using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;  // Singleton instance

    [Header("Shake Settings")]
    [SerializeField] private float shakeDuration = 0.2f;  // Duration of the shake
    [SerializeField] private float shakeMagnitude = 0.3f;  // Magnitude of the shake

    private Vector3 originalPosition;

    private void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this; 
        }
        else
        {
            Destroy(gameObject); 
        }
    }

    private void Start()
    {
        originalPosition = Camera.main.transform.position; 
    }

    public void ShakeCamera()
    {
        StartCoroutine(Shake());  
    }

    private System.Collections.IEnumerator Shake()
    {
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;

            Camera.main.transform.localPosition = originalPosition + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        Camera.main.transform.localPosition = originalPosition;  // Reset camera position
    }

}
