using UnityEngine;
using UnityEngine.Rendering.Universal; // ����Light2D�������ռ�

public class Ship : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float acceleration = 2f;
    [SerializeField] private float deceleration = 1f;
    [SerializeField] private float currentSpeed;

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
    [SerializeField] private Light2D directionalLight; // 2D�����
    [SerializeField] private Light2D autoLight; // 2D�Զ���
    [SerializeField] private float lowIntensity = 0.2f; // ��Ѫ������40%ʱ��Դ������
    [SerializeField] private float normalIntensity = 1f;
    [SerializeField] private float directionalnormalIntensity = 12f;// ����״̬�µĹ�Դ����
    [SerializeField] private Color dangerColor = Color.red; // Ѫ����ʱ�Ĺ�Դ��ɫ
    [SerializeField] private Color normalColor = Color.white; // ����״̬�µĹ�Դ��ɫ
    private bool isFlashing = false; // ��ֹ�ظ�������˸Э��

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

        transform.Translate(velocity * Time.deltaTime);
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
            // �ָ���Դ������״̬
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
                    StopAllCoroutines(); // ֹͣ����Э�̣�ֹͣ��˸
                    autoLight.enabled = true; // ȷ����Դ�ָ�����״̬
                    isFlashing = false; // ������˸״̬
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
                autoLight.enabled = !autoLight.enabled; // �л�����״̬
            }
            yield return new WaitForSeconds(0.5f); // ÿ��0.5���л�
        }

        if (autoLight != null)
        {
            autoLight.enabled = true; // �ָ�Ϊ����״̬
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
    public float GetCurrentSpeed() => currentSpeed;
}
