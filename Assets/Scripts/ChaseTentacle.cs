using UnityEngine;

public class ChaseTentacle : MonoBehaviour
{
    private Transform player;
    private Ship playerShip;
    private float currentSpeed;
    private float accelerateSpeed;
    private float decelerateSpeed;

    [Header("Speed Settings")]
    [SerializeField] public float accelerationFactor = 1.5f;
    [SerializeField] public float decelerationFactor = 0.7f;
    [SerializeField] public float thrustSpeedFactor = 2.0f; 
    [SerializeField] public float thrustDuration = 0.5f; 
    [SerializeField] public float thrustCooldown = 3f; 

    [Header("Distance Settings")]
    [SerializeField] public float minDistance = 4.5f;
    [SerializeField] public float maxDistance = 10f;
    [SerializeField] public float thrustTriggerDistance = 8f;
    [SerializeField] public float maxChaseRange = 25f;

    private bool isStopped = false;
    private bool isThrusting = false;
    private float thrustTimer = 0f;
    private float thrustEndTime = 0f;

    public void Initialize(Transform playerTransform)
    {
        if (playerTransform == null)
        {
            Debug.LogError("Player transform is null!");
            return;
        }

        player = playerTransform;
        playerShip = player.GetComponent<Ship>();

        if (playerShip == null)
        {
            Debug.LogError("Player does not have a Ship component!");
            return;
        }

        currentSpeed = playerShip.GetCurrentSpeed();
        accelerateSpeed = currentSpeed * accelerationFactor;
        decelerateSpeed = currentSpeed * decelerationFactor;
    }

    private void Update()
    {
        if (isStopped || player == null || playerShip == null) return;

        float playerSpeed = playerShip.GetCurrentSpeed();
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer > maxChaseRange)
        {
            Debug.Log("Player out of range. Slowing down and stopping chase.");
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, Time.deltaTime * 2f);
            if (currentSpeed <= 0.1f)
            {
                StopAndDestroy();
            }
            return;
        }

        if (thrustTimer > 0f)
        {
            thrustTimer -= Time.deltaTime;
        }

        if (isThrusting)
        {
            if (Time.time >= thrustEndTime)
            {
                isThrusting = false;
                currentSpeed = playerSpeed;
                Debug.Log("Thrust ended. Matching player speed.");
            }
        }
        else if (thrustTimer <= 0f && distanceToPlayer > thrustTriggerDistance)
        {
            StartThrust(playerSpeed);
        }

        if (!isThrusting)
        {
            if (distanceToPlayer < minDistance)
            {
                currentSpeed = Mathf.MoveTowards(currentSpeed, decelerateSpeed, Time.deltaTime * 2f);
            }
            else if (distanceToPlayer > maxDistance)
            {
                currentSpeed = Mathf.MoveTowards(currentSpeed, accelerateSpeed, Time.deltaTime * 2f);
            }
            else
            {
                currentSpeed = Mathf.MoveTowards(currentSpeed, playerSpeed, Time.deltaTime * 2f);
            }
        }

        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * currentSpeed * Time.deltaTime;
    }

    private void StartThrust(float playerSpeed)
    {
        isThrusting = true;
        thrustTimer = thrustCooldown;
        thrustEndTime = Time.time + thrustDuration;
        currentSpeed = playerSpeed * thrustSpeedFactor;
        Debug.Log("Thrusting towards player!");
    }

    public void StopAndDestroy()
    {
        isStopped = true;
        currentSpeed = 0f;
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Ship ship = collision.GetComponent<Ship>();
            if (ship != null)
            {
                Debug.Log("Tentacle collided with the player. Triggering Die function.");
                ship.Die();
            }
        }
    }
}
