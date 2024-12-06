using UnityEngine;

public class ChaseController : MonoBehaviour
{
    [Header("Chase Settings")]
    [SerializeField] private GameObject chaseTentaclePrefab;
    [SerializeField] public float spawnDistance = 5f; // distance between player and tentacle
    [SerializeField] public float verticalOffset = -1f; // 垂直方向偏移量

    private Transform player;
    private int tentacleCount = 0; // 记录生成的触手数量

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player = collision.transform;

            // 计算生成位置
            Vector3 spawnPosition = player.position - player.right * spawnDistance;
            Debug.Log("initial spawnPosition" + spawnPosition);

            // 根据触手数量添加垂直偏移
            spawnPosition.y += tentacleCount * verticalOffset;
            Debug.Log("second spawnPosition" + spawnPosition);

            // 生成触手
            GameObject tentacle = Instantiate(chaseTentaclePrefab, spawnPosition, Quaternion.identity);

            // 设置追击逻辑
            ChaseTentacle tentacleController = tentacle.GetComponent<ChaseTentacle>();
            if (tentacleController != null)
            {
                tentacleController.Initialize(player);
            }

            // 增加触手计数
            tentacleCount++;
        }
    }
}
