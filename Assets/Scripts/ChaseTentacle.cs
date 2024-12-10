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
    [SerializeField] public float thrustSpeedFactor = 2.0f; // 突刺速度倍数
    [SerializeField] public float thrustDuration = 0.5f; // 突刺持续时间
    [SerializeField] public float thrustCooldown = 3f; // 突刺冷却时间

    [SerializeField] public float minDistance = 4.5f; // Tentacle radius + buffer
    [SerializeField] public float maxDistance = 10f; // Keep some distance for chase
    [SerializeField] public float thrustTriggerDistance = 8f; // Trigger thrust farther away
    [SerializeField] public float maxChaseRange = 25f; // Allow for a larger chase range

    private bool isStopped = false; // 标记触手是否停止
    private bool isThrusting = false; // 是否正在突刺
    private float thrustTimer = 0f; // 突刺冷却计时器
    private float thrustEndTime = 0f; // 突刺结束时间

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

        // 获取玩家的速度和距离
        float playerSpeed = playerShip.GetCurrentSpeed();
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // 停止追击逻辑
        if (distanceToPlayer > maxChaseRange)
        {
            Debug.Log("Player out of range. Slowing down and stopping chase.");
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, Time.deltaTime * 2f); // Gradually slow down
            if (currentSpeed <= 0.1f)
            {
                StopAndDestroy();
            }
            return;
        }

        // 突刺冷却计时
        if (thrustTimer > 0f)
        {
            thrustTimer -= Time.deltaTime;
        }

        // 突刺逻辑
        if (isThrusting)
        {
            if (Time.time >= thrustEndTime)
            {
                // 突刺结束，恢复到正常速度
                isThrusting = false;
                currentSpeed = playerSpeed;
                Debug.Log("Thrust ended. Matching player speed.");
            }
        }
        else if (thrustTimer <= 0f && distanceToPlayer > thrustTriggerDistance)
        {
            // 触发突刺
            StartThrust(playerSpeed);
        }

        // 根据距离调整速度（突刺时不调整）
        if (!isThrusting)
        {
            if (distanceToPlayer < minDistance)
            {
                // Decelerate to maintain minimum distance
                currentSpeed = Mathf.MoveTowards(currentSpeed, decelerateSpeed, Time.deltaTime * 2f);
            }
            else if (distanceToPlayer > maxDistance)
            {
                // Accelerate to catch up
                currentSpeed = Mathf.MoveTowards(currentSpeed, accelerateSpeed, Time.deltaTime * 2f);
            }
            else
            {
                // Maintain player's speed
                currentSpeed = Mathf.MoveTowards(currentSpeed, playerSpeed, Time.deltaTime * 2f);
            }
        }

        // 按当前速度追击玩家
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * currentSpeed * Time.deltaTime;
    }

    private void StartThrust(float playerSpeed)
    {
        isThrusting = true;
        thrustTimer = thrustCooldown;
        thrustEndTime = Time.time + thrustDuration;
        currentSpeed = playerSpeed * thrustSpeedFactor; // 突刺速度
        Debug.Log("Thrusting towards player!");
    }

    // 停止触手并销毁
    public void StopAndDestroy()
    {
        isStopped = true; // 标记为停止
        currentSpeed = 0f; // 停止移动
        Destroy(gameObject); // 删除触手对象
    }
}
