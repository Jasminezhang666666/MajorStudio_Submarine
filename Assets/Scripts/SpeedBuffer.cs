using UnityEngine;
using TMPro;

public class SpeedBuffer : MonoBehaviour
{
    [SerializeField] private float speedBoost = 1f; // Speed boost amount
    [SerializeField] private float boostDuration = 8f; // Duration of the speed boost
    [SerializeField] private TextDisplay speedBoostText; // Reference to the TextDisplay component for the speed boost message
    [SerializeField] private AudioSource collectSound; // AudioSource for the collect sound

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Player Collide?
        if (collision.CompareTag("Player"))
        {
            Ship playerShip = collision.GetComponent<Ship>();
            if (playerShip != null)
            {
                StartCoroutine(ApplySpeedBoost(playerShip));

                // Play collect sound
                if (collectSound != null)
                {
                    collectSound.Play();
                }
                else
                {
                    Debug.LogWarning("Collect sound is not assigned.");
                }

                Destroy(gameObject); // Destroy the SpeedBuffer object after use

                // Trigger the specific TextDisplay instance for the speed boost message
                if (speedBoostText != null)
                {
                    speedBoostText.ShowMessage();
                }
            }
        }
    }

    private System.Collections.IEnumerator ApplySpeedBoost(Ship playerShip)
    {
        float originalMaxSpeed = playerShip.GetMaxSpeed(); // Get max speed
        playerShip.SetMaxSpeed(originalMaxSpeed + speedBoost); // Increase speed

        Debug.Log($"Speed boosted! New max speed: {playerShip.GetMaxSpeed()}");

        yield return new WaitForSeconds(boostDuration);

        playerShip.SetMaxSpeed(originalMaxSpeed); // Restore original speed
        Debug.Log($"Speed boost ended. Max speed restored to: {playerShip.GetMaxSpeed()}");
    }
}
