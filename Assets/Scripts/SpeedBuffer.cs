using UnityEngine;

public class SpeedBuffer : MonoBehaviour
{
    [SerializeField] private float speedBoost = 2f; 
    [SerializeField] private float boostDuration = 5f; 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Player Collide?
        if (collision.CompareTag("Player"))
        {
            Ship playerShip = collision.GetComponent<Ship>();
            if (playerShip != null)
            {
                StartCoroutine(ApplySpeedBoost(playerShip));
                Destroy(gameObject);
            }
        }
    }

    private System.Collections.IEnumerator ApplySpeedBoost(Ship playerShip)
    {
        float originalMaxSpeed = playerShip.GetMaxSpeed(); //get max speed
        playerShip.SetMaxSpeed(originalMaxSpeed + speedBoost); // increase speed

        Debug.Log($"Speed boosted! New max speed: {playerShip.GetMaxSpeed()}");

        yield return new WaitForSeconds(boostDuration);

        playerShip.SetMaxSpeed(originalMaxSpeed); // back to original speed
        Debug.Log($"Speed boost ended. Max speed restored to: {playerShip.GetMaxSpeed()}");
    }
}
