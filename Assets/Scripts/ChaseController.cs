using UnityEngine;

public class ChaseController : MonoBehaviour
{
    [Header("Chase Settings")]
    [SerializeField] private GameObject chaseTentaclePrefab; // ׷�����ֵ�Prefab
    [SerializeField] private float spawnDistance = 5f; // ������������ҵľ���

    private Transform player; // ��Ҷ����Transform

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ����Ƿ���Player������ײ
        if (collision.CompareTag("Player"))
        {
            player = collision.transform;

            // �����������ɴ���
            Vector3 spawnPosition = player.position - player.right * spawnDistance;
            GameObject tentacle = Instantiate(chaseTentaclePrefab, spawnPosition, Quaternion.identity);

            // Ϊ�������׷���߼�
            ChaseTentacle tentacleController = tentacle.GetComponent<ChaseTentacle>();
            if (tentacleController != null)
            {
                tentacleController.Initialize(player);
            }
        }
    }
}
