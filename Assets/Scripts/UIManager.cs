using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ShipUIManager : MonoBehaviour
{
    public static ShipUIManager Instance;

    [SerializeField] private TextMeshProUGUI damageText;
    [SerializeField] private TextMeshProUGUI gasText;

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
        // Check if the R key is pressed to reload the scene
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

    public void UpdateUI(float damage, float gas)
    {
        damageText.text = $"Damage: {damage:F1}%";
        gasText.text = $"Gas: {gas:F1}";
    }
}
