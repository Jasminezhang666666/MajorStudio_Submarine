using UnityEngine;

public class ChaseTentacle : MonoBehaviour
{
    private Transform player;
    private Ship playerShip;
    private float currentSpeed;
    private float accelerateSpeed;
    private float decelerateSpeed;

    [Header("Speed Settings")]
    [SerializeField] private float accelerationFactor = 1.5f;
    [SerializeField] private float decelerationFactor = 0.7f;

    [Header("Distance Settings")]
    [SerializeField] private float minDistance = 5f;
    [SerializeField] private float maxDistance = 10f;

    private bool isStopped = false; // 标记触手是否停止

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
    }

    private void Update()
    {
        if (isStopped) return; // 如果已停止，不再移动

        if (player == null || playerShip == null) return;

        // current Speed
        float playerSpeed = playerShip.GetCurrentSpeed();

        // Distance of player between the tentacle
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Speed
        if (distanceToPlayer < minDistance)
        {
            Debug.Log("Deacelerate");
            currentSpeed = Mathf.Lerp(currentSpeed, decelerateSpeed, Time.deltaTime);
        }
        else if (distanceToPlayer > maxDistance)
        {
            Debug.Log("Accelerate");
            currentSpeed = Mathf.Lerp(currentSpeed, accelerateSpeed, Time.deltaTime);
        }
        else
        {
            Debug.Log("Keep up with the player");
            currentSpeed = Mathf.Lerp(currentSpeed, playerSpeed, Time.deltaTime);
        }
        Debug.Log("Distance: " + currentSpeed);
        // 按当前速度追击玩家
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * currentSpeed * Time.deltaTime;
    }

    // 停止触手并销毁
    public void StopAndDestroy()
    {
        isStopped = true; // 标记为停止
        currentSpeed = 0f; // 停止移动
        Destroy(gameObject); // 删除触手对象
    }
}
