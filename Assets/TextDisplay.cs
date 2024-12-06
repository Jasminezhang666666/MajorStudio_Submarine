using UnityEngine;
using TMPro;

public class TextDisplay : MonoBehaviour
{
    [SerializeField] private float displayDuration = 2f; // How long to display the text
    private TextMeshProUGUI textComponent;

    private void Awake()
    {
        textComponent = GetComponent<TextMeshProUGUI>();

        if (textComponent == null)
        {
            Debug.LogError("TextDisplay: TextMeshProUGUI component is missing!");
        }

        // Ensure the text is hidden on start
        gameObject.SetActive(false);
    }

    public void ShowMessage()
    {
        // Only start coroutine if the GameObject is active
        if (!gameObject.activeInHierarchy)
        {
            gameObject.SetActive(true);
        }

        StopAllCoroutines(); // Stop any ongoing coroutine to reset the timer
        StartCoroutine(DisplayMessage());
    }

    private System.Collections.IEnumerator DisplayMessage()
    {
        // Make sure the object is active to allow the coroutine
        yield return new WaitForSeconds(displayDuration); // Wait for the specified duration
        gameObject.SetActive(false); // Hide the message after the duration
    }
}
