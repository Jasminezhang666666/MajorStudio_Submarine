using UnityEngine;

public class ChaseController : MonoBehaviour
{
    [Header("Chase Settings")]
    [SerializeField] private GameObject chaseTentaclePrefab; 
    [SerializeField] private float spawnDistance = 5f; // distance between player and tentacle

    private Transform player; 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.CompareTag("Player"))
        {
            player = collision.transform;

            //spawn
            Vector3 spawnPosition = player.position - player.right * spawnDistance;
            GameObject tentacle = Instantiate(chaseTentaclePrefab, spawnPosition, Quaternion.identity);

            //chasing
            ChaseTentacle tentacleController = tentacle.GetComponent<ChaseTentacle>();
            if (tentacleController != null)
            {
                tentacleController.Initialize(player);
            }
        }
    }
}
