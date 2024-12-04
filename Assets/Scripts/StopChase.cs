using UnityEngine;

public class StopChasing : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 检测是否与玩家发生碰撞
        if (collision.CompareTag("Player"))
        {
            // 找到场景中所有的触手对象
            ChaseTentacle[] tentacles = FindObjectsOfType<ChaseTentacle>();
            foreach (var tentacle in tentacles)
            {
                // 将触手速度设置为0并删除
                tentacle.StopAndDestroy();
            }
        }
    }
}
