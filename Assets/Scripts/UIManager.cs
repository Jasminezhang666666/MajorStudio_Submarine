using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Required for Slider and Image components

public class ShipUIManager : MonoBehaviour
{
    public static ShipUIManager Instance;

    [SerializeField] private Slider healthBar; // Slider for the health bar
    [SerializeField] private Slider gasBar; // Slider for the gas bar
    [SerializeField] private TextMeshProUGUI currentSpeedText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ReloadCurrentScene();
        }
    }

    private void ReloadCurrentScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
        Debug.Log("Scene reloaded!");
    }

    public void UpdateUI(float health, float gas, float currentSpeed)
    {
        // Update the health and gas bars
        if (healthBar != null)
        {
            healthBar.value = health / 100f; // Now health is out of 100
        }

        if (gasBar != null)
        {
            gasBar.value = gas / 100f; // Now gas is out of 100
        }

        // Update speed text
        if (currentSpeedText != null)
        {
            currentSpeedText.text = $"Speed: {currentSpeed:F1}";
        }
    }
}
