using Cinemachine;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    private CinemachineVirtualCamera virtualCamera;
    private CinemachineBasicMultiChannelPerlin perlin;

    private float shakeDuration;
    private float shakeIntensity;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject); 
        }

        virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
        if (virtualCamera == null)
        {
            Debug.LogError("Cinemachine Virtual Camera not found in the scene.");
            return;
        }

        // Get the Perlin Noise component from the Cinemachine Virtual Camera
        perlin = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        if (perlin == null)
        {
            Debug.LogError("Cinemachine Basic Multi Channel Perlin component not found.");
            return;
        }
    }

    public void ShakeCamera(float intensity, float duration)
    {
        if (perlin != null)
        {
            perlin.m_AmplitudeGain = intensity; 
            shakeDuration = duration; 
            shakeIntensity = intensity; 
        }
        else
        {
            Debug.LogError("Cinemachine Perlin component is missing. Cannot shake camera.");
        }
    }

    private void Update()
    {
        if (shakeDuration > 0)
        {
            shakeDuration -= Time.deltaTime; 
            if (shakeDuration <= 0f)
            {
                perlin.m_AmplitudeGain = 0f; // Stop the shake when duration is over
            }
        }
    }
}
