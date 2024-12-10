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
    [SerializeField] private float attackSpeed = 5f;
    [SerializeField] private float waitDurationAfterHit = 2f;
    [SerializeField] private float backupDuration = 1f;
    [SerializeField] private float wiggleMoveDistance = 0.5f;
    [SerializeField] private float wiggleCycleTime = 0.5f;

    [Header("Animation")]
    [SerializeField] private Animator animator; // Animator component

    private bool movingRight = true;
    private FishState currentState = FishState.Normal;
    private SpriteRenderer spriteRenderer;
    private Transform player;

    private bool isWiggling = false;
    private bool isBackingUp = false;

    private int hitCount = 0;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
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
        if (isBackingUp || isWiggling || currentState == FishState.Attack) return;

        float moveDirection = movingRight ? 1 : -1;
        transform.Translate(Vector2.right * moveSpeed * moveDirection * Time.deltaTime);

        animator.SetTrigger("Idle"); // Trigger Idle animation while moving horizontally
    }

    private void AttackBehavior()
    {
        if (player != null)
        {
            animator.SetTrigger("Attack"); // Trigger Attack animation

            Vector3 directionToPlayer = (player.position - transform.position).normalized;

            float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

            if ((directionToPlayer.x < 0 && transform.localScale.x > 0) || (directionToPlayer.x > 0 && transform.localScale.x < 0))
            {
                Vector3 newScale = transform.localScale;
                newScale.x = Mathf.Abs(newScale.x) * (directionToPlayer.x > 0 ? 1 : -1);
                transform.localScale = newScale;
            }

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
        if (isBackingUp || isWiggling) return;

        print("HitPlayer");

        Ship playerShip = playerCollider.GetComponent<Ship>();
        if (playerShip != null)
        {
            float damageAmount = 5f;
            playerShip.TakeDamage(damageAmount);

            if (CameraShake.Instance != null)
            {
                CameraShake.Instance.ShakeCamera(1.5f, 0.5f);
            }

            hitCount++;
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
        isWiggling = true;

        float elapsedWiggleTime = 0f;
        while (elapsedWiggleTime < waitDurationAfterHit)
        {
            Vector3 wiggleBackward = -transform.right * wiggleMoveDistance;
            transform.position += wiggleBackward * Time.deltaTime;

            yield return new WaitForSeconds(wiggleCycleTime / 2);

            if (player != null)
            {
                Vector3 directionToPlayer = (player.position - transform.position).normalized;
                transform.position += directionToPlayer * wiggleMoveDistance * Time.deltaTime;
            }

            yield return new WaitForSeconds(wiggleCycleTime / 2);

            elapsedWiggleTime += wiggleCycleTime;
        }

        isWiggling = false;

        StartCoroutine(BackupAndReturnToAttack());
    }

    private System.Collections.IEnumerator BackupAndReturnToNormal()
    {
        isBackingUp = true;

        Vector3 backupDirection = movingRight ? -transform.right : transform.right;
        float backupTime = 0f;

        while (backupTime < backupDuration)
        {
            transform.position += backupDirection * moveSpeed * Time.deltaTime;
            backupTime += Time.deltaTime;
            yield return null;
        }

        if ((movingRight && transform.localScale.x < 0) || (!movingRight && transform.localScale.x > 0))
        {
            Flip();
        }

        isBackingUp = false;
        currentState = FishState.Normal;
        MoveHorizontally();
    }

    private System.Collections.IEnumerator BackupAndReturnToAttack()
    {
        isBackingUp = true;

        Vector3 backupDirection = movingRight ? -transform.right : transform.right;
        float backupTime = 0f;

        while (backupTime < backupDuration)
        {
            transform.position += backupDirection * moveSpeed * Time.deltaTime;
            backupTime += Time.deltaTime;
            yield return null;
        }

        isBackingUp = false;
        EnterAttackState();
    }
}