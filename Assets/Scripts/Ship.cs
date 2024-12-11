using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class Ship : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float acceleration = 2f;
    [SerializeField] private float deceleration = 1f;
    [SerializeField] private float currentSpeed;
    public bool CanMove { get; set; } = true; // Default to true, allowing movement

    [Header("Gas Settings")]
    [SerializeField] private float gasMaximum = 100f;
    [SerializeField] private float gasStart = 50f;
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

    [Header("UI Settings")]
    [SerializeField] private GameObject flashRedUI; // UI object for flashing red
    [SerializeField] private TextMeshProUGUI warningText; // Text for displaying warnings
    [SerializeField] private float flashSpeed = 2f; // Speed of the flash effect

    private bool isFlashingRed = false;

    private LayerMask wallLayerMask;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource introVoice; // AA
    [SerializeField] private AudioSource sound3;
    [SerializeField] private AudioSource engineAudio;// Low gas warning: 3
    [SerializeField] private AudioSource healthAlarmAudio;  // Audio for Health Alarm
    [SerializeField] private AudioSource lowGasAlarmAudio;  // Audio for Low Gas Alarm

    private bool isHealthAlarmPlaying = false;
    private bool isLowGasAlarmPlaying = false;

    private bool hasPlayedLowGasSound = false;
    public bool HasReachedEnd { get; set; } = false;

    void Start()
{
    defaultStartPosition = transform.position;

    Vector3 savedPosition = SaveManager.LoadGasStationPosition();
    respawnPosition = savedPosition != Vector3.zero ? savedPosition : defaultStartPosition;

    damageAmount = SaveManager.LoadPlayerDamage();

    transform.position = respawnPosition;
    currentGas = gasStart;

    if (Submarineanimator == null)
    {
        Submarineanimator = GetComponent<Animator>();
    }

    // Set the animator to the idle state at the start
    Submarineanimator.SetBool("IsIdle", true);

    wallLayerMask = LayerMask.GetMask("Ship_Walls");

    // Disable movement initially
    CanMove = false;

    StartCoroutine(PlayIntroVoiceAndEnableMovement());
}

private IEnumerator PlayIntroVoiceAndEnableMovement()
{
    if (introVoice != null && introVoice.clip != null)
    {
        introVoice.Play();
        yield return new WaitForSeconds(introVoice.clip.length); // Wait for the duration of the clip
    }
    else
    {
        Debug.LogWarning("IntroVoice AudioSource or clip is not assigned. Falling back to 14 seconds delay.");
        yield return new WaitForSeconds(14f); // Fallback duration
    }

    // Enable movement
    CanMove = true;

    // Set idle to false if input is detected (optional for when movement starts)
    Submarineanimator.SetBool("IsIdle", false);
}

    void Update()
    {
        HandleMovement();

        // Decrease gas over time
        currentGas -= gasDecreaseRate * Time.deltaTime;
        currentGas = Mathf.Clamp(currentGas, 0, gasMaximum);

        // Update current speed
        currentSpeed = velocity.magnitude;

        // Update UI
        ShipUIManager.Instance.UpdateUI(damageAmount, currentGas, currentSpeed);

        // Handle lights based on damage
        HandleLights();

        HandleFlashingUI();
        HandleEngineAudio();
        HandleAlarms();
    }

    private void PlayLowGasSound()
    {
        if (sound3 != null)
        {
            sound3.Play();
            Debug.Log("Playing sound 3.");
            hasPlayedLowGasSound = true; // Ensure the sound plays only once
        }
    }

 
    private void HandleMovement()
    {
        if (!CanMove)
        {
            velocity = Vector2.zero; // Stop the ship if movement is disabled
            return;
        }

        input = Vector2.zero;

        // Get input
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

        // Predict the new position
        Vector3 predictedPosition = transform.position + (Vector3)(velocity * Time.deltaTime);

        // Check if the predicted position collides with walls
        Collider2D wallCollider = Physics2D.OverlapPoint(predictedPosition, LayerMask.GetMask("Ship_Walls"));
        if (wallCollider == null)
        {
            // No collision, move the ship
            transform.position = predictedPosition;
        }
        else
        {
            // Collision detected, stop the movement
            velocity = Vector2.zero;
        }
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

                if (!isFlashingRed)
                {
                    StartCoroutine(FlashAutoLight());
                    isFlashingRed = true;
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

                if (isFlashingRed)
                {
                    StopAllCoroutines();
                    autoLight.enabled = true;
                    isFlashingRed = false;
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
    public void TakeDamage(float amount)
    {
        damageAmount -= amount;
        damageAmount = Mathf.Clamp(damageAmount, 0f, float.MaxValue);  // Ensure health doesn't go below 0

        if (damageAmount <= 0)
        {
            // Instead of directly calling Die(), we perform a check:
            HandleZeroHealth();
        }
    }

    private void HandleZeroHealth()
    {
        // Check if there are any active ChaseTentacle objects
        ChaseTentacle[] tentacles = FindObjectsOfType<ChaseTentacle>();
        if (tentacles != null && tentacles.Length > 0)
        {
            RespawnAtGasStation();
        }
        else
        {
            Die();
        }
    }

    private void RespawnAtGasStation()
    {
        Debug.Log("Player Died but in Chase Scene. Respawning at last GasStation...");

        transform.position = respawnPosition;

        // Restore health???
        damageAmount = 60f; 

        currentGas = gasMaximum;
        CanMove = true;

        ShipUIManager.Instance.UpdateUI(damageAmount, currentGas, currentSpeed);

        if (Submarineanimator != null)
        {
            Submarineanimator.SetBool("IsIdle", true);
        }
    }

    public void Die()
    {
        // If we've reached the end, do not load the death scene
        if (HasReachedEnd)
        {
            Debug.Log("Player tried to die after reaching the end. Ignoring death.");
            return;
        }

        CanMove = false;

        if (Submarineanimator != null)
        {
            Submarineanimator.SetTrigger("Submarine_crack_anim");
        }

        StartCoroutine(WaitForAnimationAndLoadScene());
    }



    private System.Collections.IEnumerator WaitForAnimationAndLoadScene()
    {
        if (Submarineanimator != null)
        {
            Submarineanimator.SetTrigger("Submarine_crack_anim");
        }

        yield return new WaitForSeconds(3f);
        UnityEngine.SceneManagement.SceneManager.LoadScene("Death");
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

    private Coroutine flashCoroutine;

    private void HandleFlashingUI()
    {
        
        string warningMessage = "";
        if (damageAmount < 40f && currentGas < 40f)
        {
            warningMessage = "WARNING WARNING WARNING";
        }
        else if (damageAmount < 40f)
        {
            warningMessage = "LOW HEALTH ALERT";
        }
        else if (currentGas < 30f)
        {
            if (!hasPlayedLowGasSound)
            {
                PlayLowGasSound();
            }
            warningMessage = "LOW GAS ALERT";
        }

        
        if (!string.IsNullOrEmpty(warningMessage))
        {
            if (warningText != null)
            {
                warningText.text = warningMessage;

               
                if (flashCoroutine == null)
                {
                    flashCoroutine = StartCoroutine(FlashText(warningText));
                }
            }

            if (flashRedUI != null)
            {
                flashRedUI.SetActive(true);
            }
        }
        else
        {
            if (warningText != null)
            {
                warningText.text = "";

            
                if (flashCoroutine != null)
                {
                    StopCoroutine(flashCoroutine);
                    flashCoroutine = null;
                    SetTextAlpha(warningText, 1f); 
                }
            }

            if (flashRedUI != null)
            {
                flashRedUI.SetActive(false);
            }
        }
    }

    
    private IEnumerator FlashText(TextMeshProUGUI text)
    {
        float alpha = 1f;
        bool fadingOut = true;

        while (true)
        {
            if (fadingOut)
            {
                alpha -= Time.deltaTime * flashSpeed; 
                if (alpha <= 0f)
                {
                    alpha = 0f;
                    fadingOut = false;
                }
            }
            else
            {
                alpha += Time.deltaTime * flashSpeed; 
                if (alpha >= 1f)
                {
                    alpha = 1f;
                    fadingOut = true;
                }
            }

            SetTextAlpha(text, alpha);
            yield return null;
        }
    }

   
    private void SetTextAlpha(TextMeshProUGUI text, float alpha)
    {
        Color color = text.color;
        color.a = alpha;
        text.color = color;
    }

    private void HandleEngineAudio()
    {
        // Play engine audio when moving forward
        if (currentSpeed > 0)
        {
            if (!engineAudio.isPlaying)
            {
                engineAudio.Play(); // Play audio only if not already playing
            }
        }
        else
        {
            if (engineAudio.isPlaying && currentSpeed <= 0)
            {
                engineAudio.Stop(); // Stop audio when not moving
            }
        }
    }
    private void HandleAlarms()
    {
        if (damageAmount < 40f && currentGas < 40f)
        {
            // Both low: Prioritize health alarm
            PlayHealthAlarm();
            StopLowGasAlarm();
        }
        else if (damageAmount < 40f)
        {
            // Only health is low
            PlayHealthAlarm();
            StopLowGasAlarm();
        }
        else if (currentGas < 40f)
        {
            // Only gas is low
            PlayLowGasAlarm();
            StopHealthAlarm();
        }
        else
        {
            // Neither is low: Stop all alarms
            StopHealthAlarm();
            StopLowGasAlarm();
        }
    }

    private void PlayHealthAlarm()
    {
        if (!isHealthAlarmPlaying && healthAlarmAudio != null)
        {
            healthAlarmAudio.Play();
            isHealthAlarmPlaying = true;
        }
    }

    private void StopHealthAlarm()
    {
        if (isHealthAlarmPlaying && healthAlarmAudio != null)
        {
            healthAlarmAudio.Stop();
            isHealthAlarmPlaying = false;
        }
    }

    private void PlayLowGasAlarm()
    {
        if (!isLowGasAlarmPlaying && lowGasAlarmAudio != null)
        {
            lowGasAlarmAudio.Play();
            isLowGasAlarmPlaying = true;
        }
    }

    private void StopLowGasAlarm()
    {
        if (isLowGasAlarmPlaying && lowGasAlarmAudio != null)
        {
            lowGasAlarmAudio.Stop();
            isLowGasAlarmPlaying = false;
        }
    }


}
