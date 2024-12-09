using UnityEngine;
using TMPro;

public class SpeedBuffer : MonoBehaviour
{
    [SerializeField] private float speedBoost = 2f; // Speed boost amount
    [SerializeField] private float boostDuration = 5f; // Duration of the speed boost
    [SerializeField] private TextDisplay speedBoostText; // Reference to the TextDisplay component for the speed boost message

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Player Collide?
        if (collision.CompareTag("Player"))
        {
            Ship playerShip = collision.GetComponent<Ship>();
            if (playerShip != null)
            {
                StartCoroutine(ApplySpeedBoost(playerShip));
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
