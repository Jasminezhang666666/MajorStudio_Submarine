using UnityEngine;

public class ChaseController : MonoBehaviour
{
    [Header("Chase Settings")]
    [SerializeField] private GameObject chaseTentaclePrefab;
    [SerializeField] public float spawnDistance = 5f; // distance between player and tentacle
    [SerializeField] public float verticalOffset = -1f; // ��ֱ����ƫ����

    private Transform player;
    private int tentacleCount = 0; // ��¼���ɵĴ�������

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player = collision.transform;

            // ��������λ��
            Vector3 spawnPosition = player.position - player.right * spawnDistance;
            Debug.Log("initial spawnPosition" + spawnPosition);

            // ���ݴ���������Ӵ�ֱƫ��
            spawnPosition.y += tentacleCount * verticalOffset;
            Debug.Log("second spawnPosition" + spawnPosition);

            // ���ɴ���
            GameObject tentacle = Instantiate(chaseTentaclePrefab, spawnPosition, Quaternion.identity);

            // ����׷���߼�
            ChaseTentacle tentacleController = tentacle.GetComponent<ChaseTentacle>();
            if (tentacleController != null)
            {
                tentacleController.Initialize(player);
            }

            // ���Ӵ��ּ���
            tentacleCount++;
        }
    }
}
