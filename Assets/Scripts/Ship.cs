using UnityEngine;

public class ShipController : MonoBehaviour
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
    [SerializeField] private float damageAmount; // Submarine health

    private Vector2 velocity; // Current velocity of the ship
    private Vector2 input; // Player input vector

    void Start()
    {
        currentGas = gasMaximum;
        damageAmount = 100f; // Submarine health (starts at 100%)
    }

    void Update()
    {
        HandleMovement();

        // Decrease gas over time
        currentGas -= gasDecreaseRate * Time.deltaTime;

        // Check if gas is depleted
        if (currentGas <= 0)
        {
            Debug.Log("Gas depleted! Player is dead.");
            Die();
        }
    }

    private void HandleMovement()
    {
        // Get player input
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        // Normalize input to prevent diagonal speed boost
        if (input.sqrMagnitude > 1f)
        {
            input = input.normalized;
        }

        // Apply acceleration based on input
        if (input != Vector2.zero)
        {
            velocity += input * acceleration * Time.deltaTime;
            velocity = Vector2.ClampMagnitude(velocity, maxSpeed); // Limit speed to maxSpeed
        }
        else
        {
            // Apply deceleration when no input
            velocity = Vector2.MoveTowards(velocity, Vector2.zero, deceleration * Time.deltaTime);
        }

        // Apply movement
        transform.Translate(velocity * Time.deltaTime);
    }

    private void Die()
    {
        
    }

    private void OnValidate()
    {
        // Ensure settings are valid
        if (acceleration < 0) acceleration = 0;
        if (deceleration < 0) deceleration = 0;
        if (maxSpeed < 0) maxSpeed = 0;
        if (gasMaximum < 0) gasMaximum = 0;
        if (gasDecreaseRate < 0) gasDecreaseRate = 0;
        if (damageAmount < 0) damageAmount = 0;
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
