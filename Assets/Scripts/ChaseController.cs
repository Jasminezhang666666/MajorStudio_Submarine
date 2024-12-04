using UnityEngine;

public class ChaseController : MonoBehaviour
{
    [Header("Chase Settings")]
    [SerializeField] private GameObject chaseTentaclePrefab; // 追击触手的Prefab
    [SerializeField] private float spawnDistance = 5f; // 触手生成与玩家的距离

    private Transform player; // 玩家对象的Transform

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 检查是否与Player发生碰撞
        if (collision.CompareTag("Player"))
        {
            player = collision.transform;

            // 在玩家身后生成触手
            Vector3 spawnPosition = player.position - player.right * spawnDistance;
            GameObject tentacle = Instantiate(chaseTentaclePrefab, spawnPosition, Quaternion.identity);

            // 为触手添加追击逻辑
            ChaseTentacle tentacleController = tentacle.GetComponent<ChaseTentacle>();
            if (tentacleController != null)
            {
                tentacleController.Initialize(player);
            }
        }
    }
}
