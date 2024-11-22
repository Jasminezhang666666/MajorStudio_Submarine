using UnityEngine;

public class Ship : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float acceleration = 2f;
    [SerializeField] private float deceleration = 1f;

    [Header("Gas Settings")]
    [SerializeField] private float gasMaximum = 40f;
    [SerializeField] private float gasDecreaseRate = 0.2f; // Rate at which gas decreases per second
    private float currentGas; // Current gas level

    [Header("Damage Settings")]
    [SerializeField] private float damageAmount = 100f; // Submarine health

    private Vector2 velocity; // Current velocity of the ship
    private Vector2 input; // Player input vector

    private Vector3 defaultStartPosition; 
    private Vector3 respawnPosition; 

    void Start()
    {
        defaultStartPosition = transform.position;

        // Try to load the last saved gas station position; default to starting position if not found
        Vector3 savedPosition = SaveManager.LoadGasStationPosition();
        respawnPosition = savedPosition != Vector3.zero ? savedPosition : defaultStartPosition;

        damageAmount = SaveManager.LoadPlayerDamage();

        transform.position = respawnPosition;
        currentGas = gasMaximum;
    }

    void Update()
    {
        HandleMovement();

        // Decrease gas over time
        currentGas -= gasDecreaseRate * Time.deltaTime;
        currentGas = Mathf.Clamp(currentGas, 0, gasMaximum);

        // Check if gas is depleted
        if (currentGas <= 0) // || damageAmount <=0
        {
            Debug.Log("Gas depleted! Player is dead.");
            Die();
        }

        // Update UI
        ShipUIManager.Instance.UpdateUI(damageAmount, currentGas);
    }

    private void HandleMovement()
    {
        input = Vector2.zero; 

        if (Input.GetKey(KeyCode.W))
        {
            input.y = 1f; 
        }
        if (Input.GetKey(KeyCode.S))
        {
            input.y = -1f; 
        }
        if (Input.GetKey(KeyCode.A))
        {
            input.x = -1f; 
        }
        if (Input.GetKey(KeyCode.D))
        {
            input.x = 1f; 
        }

        // Normalize input
        if (input.sqrMagnitude > 1f)
        {
            input = input.normalized;
        }

        // acceleration
        if (input != Vector2.zero)
        {
            velocity += input * acceleration * Time.deltaTime;
            velocity = Vector2.ClampMagnitude(velocity, maxSpeed); // Limit speed to maxSpeed
        }
        else
        {
            // deceleration 
            velocity = Vector2.MoveTowards(velocity, Vector2.zero, deceleration * Time.deltaTime);
        }

        transform.Translate(velocity * Time.deltaTime);
    }

    private void Die()
    {
        respawnPosition = SaveManager.LoadGasStationPosition();
        if (respawnPosition == Vector3.zero) respawnPosition = defaultStartPosition;

        transform.position = respawnPosition; // Respawn at the saved gas station position or default start position
        currentGas = gasMaximum; // Refill the gas to maximum
        damageAmount = SaveManager.LoadPlayerDamage(); // Reload the damage amount
    }

    public void TakeDamage(float amount)
    {
        damageAmount -= amount;
        if (damageAmount <= 0)
        {
            Debug.Log("Submarine Destroyed!");
            Die();
        }
    }

    public void RefuelGas()
    {
        currentGas = gasMaximum;
    }

    public float GetCurrentGas() => currentGas;
    public float GetDamageAmount() => damageAmount;
}
