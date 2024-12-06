using UnityEngine;
using TMPro;
using UnityEngine.Rendering.Universal;

public class GasStation : MonoBehaviour
{
    [SerializeField] private Sprite usedSprite;
    [SerializeField] private TextDisplay gasRefillText; // Reference to the specific TextDisplay component
    private SpriteRenderer spriteRenderer;
    private bool isUsed = false;
    public Light2D gasLight;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isUsed && collision.CompareTag("Player"))
        {
            Ship ship = collision.GetComponent<Ship>();
            if (ship != null)
            {
                ship.RefuelGas();
                Debug.Log("Gas refilled!");
                isUsed = true;

                // Change sprite to indicate the gas station is used
                spriteRenderer.sprite = usedSprite;
                gasLight.enabled = !gasLight.enabled;

                // Save the gas station's state
                SaveManager.SaveGasStation(transform.position, ship.GetDamageAmount());

                // Trigger the specific TextDisplay instance for the gas refill message
                if (gasRefillText != null)
                {
                    gasRefillText.ShowMessage();
                }
            }
        }
    }
}
