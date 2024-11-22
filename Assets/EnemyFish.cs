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
    [SerializeField] private float attackSpeed = 5f;  // Speed during attack phase
    [SerializeField] private float backupDuration = 3f;  // Time to move backwards after attacking

    private bool movingRight = true; 
    private FishState currentState = FishState.Normal; 
    private SpriteRenderer spriteRenderer;
    private Transform player;  

    private bool isBackingUp = false; 

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player").transform;  // Find the player object
    }

    private void Update()
    {
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
        if (!isBackingUp)
        {
            float moveDirection = movingRight ? 1 : -1;
            transform.Translate(Vector2.right * moveSpeed * moveDirection * Time.deltaTime);
        }
    }

    private void AttackBehavior()
    {
        if (player != null)
        {
            // Calculate the direction to the player
            Vector3 directionToPlayer = player.position - transform.position;

            // rotate the fish towards the player
            float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;

            // Rotate the fish to face the player 
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

            if (directionToPlayer.x < 0)  // Player is to the left
            {
                spriteRenderer.flipX = false; 
                spriteRenderer.flipY = true;
            }
            else  // Player is to the right
            {
                spriteRenderer.flipX = false;  
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
            print("Collision with Player detected");
            HitPlayer(collision.collider);
        }
        else
        {
            print("Collision with " + collision.gameObject.name + " detected");
        }
    }



    private void Flip()
    {
        movingRight = !movingRight;
        spriteRenderer.flipX = !spriteRenderer.flipX;  // This will only affect the movement, not the attack facing
    }


    private void EnterAttackState()
    {
        if (currentState != FishState.Attack)
        {
            currentState = FishState.Attack;
        }
        Debug.Log("EnemyFish has entered the Attack state!");
    }

    private void HitPlayer(Collider2D playerCollider)
    {
        print("HitPlayer");

        // Assuming player has a script with "TakeDamage" method to decrease health
        Ship playerShip = playerCollider.GetComponent<Ship>();
        if (playerShip != null)
        {
            float damageAmount = 10f;  // Define damage dealt by the fish
            playerShip.TakeDamage(damageAmount);  // Deal damage to the player

            // Check if CameraShake instance is valid
            if (CameraShake.Instance != null)
            {
                CameraShake.Instance.ShakeCamera();  // Short shake after collision
            }

            StartCoroutine(BackupAndReturnToAttack());
        }
    }


    private System.Collections.IEnumerator BackupAndReturnToAttack()
    {
        print("Backing UP");

        isBackingUp = true;

        // Backup logic
        float backupSpeed = 3f;
        Vector3 backupDirection = (transform.position - player.position).normalized;
        float backupTime = 0f;

        while (backupTime < backupDuration)
        {
            transform.Translate(backupDirection * backupSpeed * Time.deltaTime);
            backupTime += Time.deltaTime;
            yield return null;
        }

        // After backup, pause before attacking again
        yield return new WaitForSeconds(0.5f);  // Optional pause

        // After backup, return to attack mode
        isBackingUp = false;
        EnterAttackState();

        // Make sure the fish faces the player after returning to attack mode
        AttackBehavior();  // Ensure the fish faces the player correctly after backing up
    }


}
