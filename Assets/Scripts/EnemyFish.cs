using UnityEngine;

public class EnemyFish : MonoBehaviour
{
    private enum FishState
    {
        Normal,
        Attack
    }

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float attackSpeed = 5f; // Speed during attack phase
    [SerializeField] private float waitDurationAfterHit = 2f; // Time to wait after hitting player
    [SerializeField] private float backupDuration = 1f; // Time to move backwards after waiting
    [SerializeField] private float wiggleMoveDistance = 0.5f; // Distance to wiggle back and forth
    [SerializeField] private float wiggleCycleTime = 0.5f; // Time for one back-and-forth cycle

    private bool movingRight = true;
    private FishState currentState = FishState.Normal;
    private SpriteRenderer spriteRenderer;
    private Transform player;

    private bool isWiggling = false;
    private bool isBackingUp = false;

    private int hitCount = 0; // Track number of hits

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

    }


    private void Update()
    {
        if (isWiggling || isBackingUp) return;

        switch (currentState)
        {
            case FishState.Normal:
                MoveHorizontally();
                break;
            case FishState.Attack:
                AttackBehavior();
                break;
        }
    }


    private void MoveHorizontally()
    {
        // Skip movement if backing up, wiggling, or in attack state
        if (isBackingUp || isWiggling || currentState == FishState.Attack) return;

        float moveDirection = movingRight ? 1 : -1;
        transform.Translate(Vector2.right * moveSpeed * moveDirection * Time.deltaTime);
    }


    private void AttackBehavior()
    {
        if (player != null)
        {
            Vector3 directionToPlayer = (player.position - transform.position).normalized;

            // Rotate the fish to face the player
            float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

            // Flip based on the direction to the player only if the direction changes
            if ((directionToPlayer.x < 0 && transform.localScale.x > 0) || (directionToPlayer.x > 0 && transform.localScale.x < 0))
            {
                Vector3 newScale = transform.localScale;
                newScale.x = Mathf.Abs(newScale.x) * (directionToPlayer.x > 0 ? 1 : -1);
                transform.localScale = newScale;
            }

            // Move towards the player
            transform.position = Vector2.MoveTowards(transform.position, player.position, attackSpeed * Time.deltaTime);
        }
    }


    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (currentState == FishState.Normal)
        {
            if (collider.CompareTag("Enemy_Walls"))
            {
                Flip();
            }
            else if (collider.CompareTag("Light"))
            {
                EnterAttackState();
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            HitPlayer(collision.collider);
        }
    }

    private void Flip()
    {
        movingRight = !movingRight;

        // Flip the fish by changing its scale on the X axis
        Vector3 newScale = transform.localScale;
        newScale.x = Mathf.Abs(newScale.x) * (movingRight ? 1 : -1);
        transform.localScale = newScale;
    }


    private void EnterAttackState()
    {
        if (currentState != FishState.Attack)
        {
            currentState = FishState.Attack;
        }

        // Rotate to face the player after backup
        if (player != null)
        {
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }

        Debug.Log("EnemyFish has entered the Attack state!");
    }

    private void HitPlayer(Collider2D playerCollider)
    {
        // Ensure no new action is taken if already backing up or wiggling
        if (isBackingUp || isWiggling) return;

        print("HitPlayer");

        // Assuming player has a script with "TakeDamage" method to decrease health
        Ship playerShip = playerCollider.GetComponent<Ship>();
        if (playerShip != null)
        {
            float damageAmount = 5f;
            playerShip.TakeDamage(damageAmount); // Deal damage to the player

            if (CameraShake.Instance != null)
            {
                CameraShake.Instance.ShakeCamera(1.5f, 0.5f);
            }

            hitCount++; // Increment hit count
            if (hitCount < 2)
            {
                StartCoroutine(WiggleAndBackup());
            }
            else
            {
                StartCoroutine(BackupAndReturnToNormal());
            }
        }
    }

    private System.Collections.IEnumerator WiggleAndBackup()
    {
        print("Wiggling After Hit");

        isWiggling = true;

        float elapsedWiggleTime = 0f;
        while (elapsedWiggleTime < waitDurationAfterHit)
        {
            // Move backward
            Vector3 wiggleBackward = -transform.right * wiggleMoveDistance;
            transform.position += wiggleBackward * Time.deltaTime;

            // Wait for half the wiggle cycle time
            yield return new WaitForSeconds(wiggleCycleTime / 2);

            // Move toward the player
            if (player != null)
            {
                Vector3 directionToPlayer = (player.position - transform.position).normalized;
                transform.position += directionToPlayer * wiggleMoveDistance * Time.deltaTime;
            }

            // Wait for the remaining half wiggle cycle time
            yield return new WaitForSeconds(wiggleCycleTime / 2);

            elapsedWiggleTime += wiggleCycleTime;
        }

        isWiggling = false;

        StartCoroutine(BackupAndReturnToAttack());
    }

    private System.Collections.IEnumerator BackupAndReturnToNormal()
    {
        print("Backing UP to Normal Movement");

        isBackingUp = true;

        Vector3 backupDirection = movingRight ? -transform.right : transform.right;  // Use the current direction for backup
        float backupTime = 0f;

        while (backupTime < backupDuration)
        {
            transform.position += backupDirection * moveSpeed * Time.deltaTime;
            backupTime += Time.deltaTime;
            yield return null;
        }

        print("Finished Backing UP, Returning to Normal Movement");

        // After backup, check if the fish should flip based on the `movingRight` state
        if ((movingRight && transform.localScale.x < 0) || (!movingRight && transform.localScale.x > 0))
        {
            Flip();  // Flip to ensure correct direction
        }

        isBackingUp = false;
        currentState = FishState.Normal;  // Return to normal state
        MoveHorizontally();  // Start horizontal movement
    }

    private System.Collections.IEnumerator BackupAndReturnToAttack()
    {
        print("Backing UP to Attack");

        isBackingUp = true;

        Vector3 backupDirection = movingRight ? -transform.right : transform.right;  // Use the current direction for backup
        float backupTime = 0f;

        while (backupTime < backupDuration)
        {
            transform.position += backupDirection * moveSpeed * Time.deltaTime;
            backupTime += Time.deltaTime;
            yield return null;
        }

        print("Finished Backing UP, Returning to Attack State");

        // After backup, resume attack without flipping direction
        isBackingUp = false;
        EnterAttackState();
    }


}
