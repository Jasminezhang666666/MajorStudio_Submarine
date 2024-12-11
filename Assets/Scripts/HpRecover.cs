using UnityEngine;
using TMPro;

public class HpRecover : MonoBehaviour
{
    [SerializeField] private float hpRestoreAmount = 10f; // HP recover amount
    [SerializeField] private TextDisplay hpRecoverText; // Reference to the specific TextDisplay component
    [SerializeField] private AudioSource collectSound; // AudioSource for the collect sound

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Ship playerShip = collision.GetComponent<Ship>();

            if (playerShip != null && playerShip.GetDamageAmount() < 100f)
            {
                float newHP = Mathf.Min(100f, playerShip.GetDamageAmount() + hpRestoreAmount);
                playerShip.RestoreHP(newHP);

                // Play collect sound
                if (collectSound != null)
                {
                    collectSound.Play();
                }
                else
                {
                    Debug.LogWarning("Collect sound is not assigned.");
                }

                // Trigger the specific TextDisplay instance
                if (hpRecoverText != null)
                {
                    hpRecoverText.ShowMessage();
                }

                Destroy(gameObject); // Destroy the HP recovery object
            }
        }
    }
}
