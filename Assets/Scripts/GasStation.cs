using UnityEngine;

public class GasStation : MonoBehaviour
{
    [SerializeField] private Sprite usedSprite;
    private SpriteRenderer spriteRenderer;
    private bool isUsed = false;

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

                // Save the gas station's state
                SaveManager.SaveGasStation(transform.position, ship.GetDamageAmount());
            }
        }
    }
}
