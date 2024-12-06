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
    [SerializeField] public float thrustSpeedFactor = 2.0f; // ͻ���ٶȱ���
    [SerializeField] public float thrustDuration = 0.5f; // ͻ�̳���ʱ��
    [SerializeField] public float thrustCooldown = 3f; // ͻ����ȴʱ��

    [Header("Distance Settings")]
    [SerializeField] public float minDistance = 5f;
    [SerializeField] public float maxDistance = 10f;
    [SerializeField] public float thrustTriggerDistance = 8f; // ͻ�̴�������
    [SerializeField] public float maxChaseRange = 20f; // ���׷����Χ

    private bool isStopped = false; // ��Ǵ����Ƿ�ֹͣ
    private bool isThrusting = false; // �Ƿ�����ͻ��
    private float thrustTimer = 0f; // ͻ����ȴ��ʱ��
    private float thrustEndTime = 0f; // ͻ�̽���ʱ��

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

        // ��ȡ��ҵ��ٶȺ;���
        float playerSpeed = playerShip.GetCurrentSpeed();
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // ֹͣ׷���߼�
        if (distanceToPlayer > maxChaseRange)
        {
            Debug.Log("Player out of range. Stopping chase.");
            StopAndDestroy();
            return;
        }

        // ͻ����ȴ��ʱ
        if (thrustTimer > 0f)
        {
            thrustTimer -= Time.deltaTime;
        }

        // ͻ���߼�
        if (isThrusting)
        {
            if (Time.time >= thrustEndTime)
            {
                // ͻ�̽������ָ��������ٶ�
                isThrusting = false;
                currentSpeed = playerSpeed;
                Debug.Log("Thrust ended. Matching player speed.");
            }
        }
        else if (thrustTimer <= 0f && distanceToPlayer > thrustTriggerDistance)
        {
            // ����ͻ��
            StartThrust(playerSpeed);
        }

        // ���ݾ�������ٶȣ�ͻ��ʱ��������
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

        // ����ǰ�ٶ�׷�����
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * currentSpeed * Time.deltaTime;
    }

    private void StartThrust(float playerSpeed)
    {
        isThrusting = true;
        thrustTimer = thrustCooldown;
        thrustEndTime = Time.time + thrustDuration;
        currentSpeed = playerSpeed * thrustSpeedFactor; // ͻ���ٶ�
        Debug.Log("Thrusting towards player!");
    }

    // ֹͣ���ֲ�����
    public void StopAndDestroy()
    {
        isStopped = true; // ���Ϊֹͣ
        currentSpeed = 0f; // ֹͣ�ƶ�
        Destroy(gameObject); // ɾ�����ֶ���
    }
}
