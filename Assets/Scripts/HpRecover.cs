using UnityEngine;

public class HpRecover : MonoBehaviour
{
    [SerializeField] private float hpRestoreAmount = 10f; // HP recover amount

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if it's player
        if (collision.CompareTag("Player"))
        {
            //Get the Ship Collider
            Ship playerShip = collision.GetComponent<Ship>();

            if (playerShip != null)
            {
                
                if (playerShip.GetDamageAmount() < 100f)
                {
                    //Redover
                    float newHP = Mathf.Min(100f, playerShip.GetDamageAmount() + hpRestoreAmount);
                    playerShip.RestoreHP(newHP);

                   
                    Destroy(gameObject);
                }
            }
        }
    }
}
