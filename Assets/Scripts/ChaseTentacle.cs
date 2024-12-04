using UnityEngine;

public class ChaseTentacle : MonoBehaviour
{
    private Transform player; // ��Ҷ����Transform
    private Ship playerShip; // Ship�ű�������
    private float currentSpeed; // ��ǰ�ٶ�
    private float accelerateSpeed; // ���ٺ���ٶ�
    private float decelerateSpeed; // ���ٺ���ٶ�
    private float changeSpeedTimer; // �����л��ٶȵļ�ʱ��

    [Header("Speed Settings")]
    [SerializeField] private float accelerationFactor = 1.5f; // ���ٱ���
    [SerializeField] private float decelerationFactor = 0.7f; // ���ٱ���
    [SerializeField] private float speedChangeInterval = 3f; // ÿ���������л�һ���ٶ�

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

        // ���µ�ǰ��ҵ��ٶ�
        currentSpeed = playerShip.GetCurrentSpeed();

        // �����ٶȵ��л��߼�
        changeSpeedTimer -= Time.deltaTime;
        if (changeSpeedTimer <= 0)
        {
            // �л��ٶ�״̬
            if (Mathf.Approximately(currentSpeed, accelerateSpeed))
            {
                currentSpeed = decelerateSpeed; // �л�������״̬
            }
            else if (Mathf.Approximately(currentSpeed, decelerateSpeed))
            {
                currentSpeed = playerShip.GetCurrentSpeed(); // �л�������ٶ�
            }
            else
            {
                currentSpeed = accelerateSpeed; // �л�������״̬
            }

            // ���ü�ʱ��
            changeSpeedTimer = speedChangeInterval;
        }

        // ׷�����
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * currentSpeed * Time.deltaTime;
    }
}
