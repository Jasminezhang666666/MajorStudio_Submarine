using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Ship : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float acceleration = 2f;
    [SerializeField] private float deceleration = 1f;
    [SerializeField] private float currentSpeed;
    public bool CanMove { get; set; } = true; // Default to true, allowing movement

    [Header("Gas Settings")]
    [SerializeField] private float gasMaximum = 60f;
    [SerializeField] private float gasDecreaseRate = 0.2f;
    private float currentGas;

    [Header("Damage Settings")]
    [SerializeField] private float damageAmount = 100f;

    private Vector2 velocity;
    private Vector2 input;

    private Vector3 defaultStartPosition;
    private Vector3 respawnPosition;

    [Header("Animation Settings")]
    [SerializeField] private Animator Submarineanimator;

    [Header("Light Settings")]
    [SerializeField] private Light2D directionalLight;
    [SerializeField] private Light2D autoLight;
    [SerializeField] private float lowIntensity = 0.2f;
    [SerializeField] private float normalIntensity = 1f;
    [SerializeField] private float directionalnormalIntensity = 12f;
    [SerializeField] private Color dangerColor = Color.red;
    [SerializeField] private Color normalColor = Color.white;
    private bool isFlashing = false;

    private Bounds combinedBounds;

    void Start()
    {
        defaultStartPosition = transform.position;

        Vector3 savedPosition = SaveManager.LoadGasStationPosition();
        respawnPosition = savedPosition != Vector3.zero ? savedPosition : defaultStartPosition;

        damageAmount = SaveManager.LoadPlayerDamage();

        transform.position = respawnPosition;
        currentGas = gasMaximum;

        if (Submarineanimator == null)
        {
            Submarineanimator = GetComponent<Animator>();
        }

    }

    void Update()
    {
        HandleMovement();

        // Decrease gas over time
        currentGas -= gasDecreaseRate * Time.deltaTime;
        currentGas = Mathf.Clamp(currentGas, 0, gasMaximum);

        // Check if gas is depleted
        if (currentGas <= 0)
        {
            Debug.Log("Gas depleted! Player is dead.");
            Die();
        }

        // Update current speed
        currentSpeed = velocity.magnitude;

        // Update UI
        ShipUIManager.Instance.UpdateUI(damageAmount, currentGas, currentSpeed);

        // Handle lights based on damage
        HandleLights();
    }

    private void HandleMovement()
    {
        if (!CanMove)
        {
            velocity = Vector2.zero; // Stop the ship if movement is disabled
            return;
        }

        input = Vector2.zero;

        if (Input.GetKey(KeyCode.W)) input.y = 1f;
        if (Input.GetKey(KeyCode.S)) input.y = -1f;
        if (Input.GetKey(KeyCode.A)) input.x = -1f;
        if (Input.GetKey(KeyCode.D)) input.x = 1f;

        if (input.sqrMagnitude > 1f) input = input.normalized;

        if (input != Vector2.zero)
        {
            Submarineanimator.SetBool("IsIdle", false);
        }
        else
        {
            Submarineanimator.SetBool("IsIdle", true);
        }

        if (input != Vector2.zero)
        {
            velocity += input * acceleration * Time.deltaTime;
            velocity = Vector2.ClampMagnitude(velocity, maxSpeed);
        }
        else
        {
            velocity = Vector2.MoveTowards(velocity, Vector2.zero, deceleration * Time.deltaTime);
        }

        Vector3 newPosition = transform.position + (Vector3)(velocity * Time.deltaTime);
        newPosition = ClampPositionWithinBounds(newPosition);
        transform.position = newPosition;
    }
    private Vector3 ClampPositionWithinBounds(Vector3 targetPosition)
    {
        if (combinedBounds.size != Vector3.zero)
        {
            targetPosition.x = Mathf.Clamp(targetPosition.x, combinedBounds.min.x, combinedBounds.max.x);
            targetPosition.y = Mathf.Clamp(targetPosition.y, combinedBounds.min.y, combinedBounds.max.y);
        }
        return targetPosition;
    }

    private void HandleLights()
    {
        if (damageAmount < 40f)
        {
            if (directionalLight != null)
            {
                directionalLight.intensity = lowIntensity;
                directionalLight.color = dangerColor;
            }

            if (autoLight != null)
            {
                autoLight.intensity = lowIntensity;
                autoLight.color = dangerColor;

                if (!isFlashing)
                {
                    StartCoroutine(FlashAutoLight());
                    isFlashing = true;
                }
            }
        }
        else
        {
            if (directionalLight != null)
            {
                directionalLight.intensity = directionalnormalIntensity;
                directionalLight.color = normalColor;
            }

            if (autoLight != null)
            {
                autoLight.intensity = normalIntensity;
                autoLight.color = normalColor;

                if (isFlashing)
                {
                    StopAllCoroutines();
                    autoLight.enabled = true;
                    isFlashing = false;
                }
            }
        }
    }

    private System.Collections.IEnumerator FlashAutoLight()
    {
        while (damageAmount < 40f)
        {
            if (autoLight != null)
            {
                autoLight.enabled = !autoLight.enabled;
            }
            yield return new WaitForSeconds(0.5f);
        }

        if (autoLight != null)
        {
            autoLight.enabled = true;
        }
    }

    private void Die()
    {
        respawnPosition = SaveManager.LoadGasStationPosition();
        if (respawnPosition == Vector3.zero) respawnPosition = defaultStartPosition;

        transform.position = respawnPosition;
        currentGas = gasMaximum;
        damageAmount = SaveManager.LoadPlayerDamage();
    }

    public void TakeDamage(float amount)
    {
        damageAmount -= amount;
        damageAmount = Mathf.Clamp(damageAmount, 0f, float.MaxValue);  // Ensure health doesn't go below 0

        if (damageAmount <= 0)
        {
            Die(); 
        }
    }

    public void RefuelGas()
    {
        currentGas = gasMaximum;
    }

    public void RestoreHP(float newDamageAmount)
    {
        damageAmount = newDamageAmount;
        Debug.Log($"HP Restored to: {damageAmount}");
    }

    public float GetMaxSpeed() => maxSpeed;

    public void SetMaxSpeed(float newMaxSpeed)
    {
        maxSpeed = newMaxSpeed;
    }

    public float GetCurrentGas() => currentGas;
    public float GetDamageAmount() => damageAmount;

    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }
}
