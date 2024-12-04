using UnityEngine;

public class StopChasing : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ����Ƿ�����ҷ�����ײ
        if (collision.CompareTag("Player"))
        {
            // �ҵ����������еĴ��ֶ���
            ChaseTentacle[] tentacles = FindObjectsOfType<ChaseTentacle>();
            foreach (var tentacle in tentacles)
            {
                // �������ٶ�����Ϊ0��ɾ��
                tentacle.StopAndDestroy();
            }
        }
    }
}
