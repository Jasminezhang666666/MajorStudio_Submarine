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

    private bool isStopped = false; // ��Ǵ����Ƿ�ֹͣ

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
        if (isStopped) return; // �����ֹͣ�������ƶ�

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
        // ����ǰ�ٶ�׷�����
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * currentSpeed * Time.deltaTime;
    }

    // ֹͣ���ֲ�����
    public void StopAndDestroy()
    {
        isStopped = true; // ���Ϊֹͣ
        currentSpeed = 0f; // ֹͣ�ƶ�
        Destroy(gameObject); // ɾ�����ֶ���
    }
}
