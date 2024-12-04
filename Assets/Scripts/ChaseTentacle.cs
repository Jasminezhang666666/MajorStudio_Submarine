using UnityEngine;

public class ChaseTentacle : MonoBehaviour
{
    private Transform player; // 玩家对象的Transform
    private Ship playerShip; // Ship脚本的引用
    private float currentSpeed; // 当前速度
    private float accelerateSpeed; // 加速后的速度
    private float decelerateSpeed; // 减速后的速度
    private float changeSpeedTimer; // 用于切换速度的计时器

    [Header("Speed Settings")]
    [SerializeField] private float accelerationFactor = 1.5f; // 加速倍数
    [SerializeField] private float decelerationFactor = 0.7f; // 减速倍数
    [SerializeField] private float speedChangeInterval = 3f; // 每隔多少秒切换一次速度

    public void Initialize(Transform playerTransform)
    {
        player = playerTransform;
        playerShip = player.GetComponent<Ship>();

        if (playerShip != null)
        {
            currentSpeed = playerShip.GetCurrentSpeed();
            accelerateSpeed = currentSpeed * accelerationFactor;
            decelerateSpeed = currentSpeed * decelerationFactor;
        }

        changeSpeedTimer = speedChangeInterval;
    }

    private void Update()
    {
        if (player == null || playerShip == null) return;

        // 更新当前玩家的速度
        currentSpeed = playerShip.GetCurrentSpeed();

        // 更新速度的切换逻辑
        changeSpeedTimer -= Time.deltaTime;
        if (changeSpeedTimer <= 0)
        {
            // 切换速度状态
            if (Mathf.Approximately(currentSpeed, accelerateSpeed))
            {
                currentSpeed = decelerateSpeed; // 切换到减速状态
            }
            else if (Mathf.Approximately(currentSpeed, decelerateSpeed))
            {
                currentSpeed = playerShip.GetCurrentSpeed(); // 切换回玩家速度
            }
            else
            {
                currentSpeed = accelerateSpeed; // 切换到加速状态
            }

            // 重置计时器
            changeSpeedTimer = speedChangeInterval;
        }

        // 追击玩家
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * currentSpeed * Time.deltaTime;
    }
}
