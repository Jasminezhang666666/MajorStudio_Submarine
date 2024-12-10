using Cinemachine;
using UnityEngine;

public class ChaseController : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    public float targetOrthoSize = 13f;
    public float transitionSpeed = 2f;
    public float shakeIntensity = 1f;
    public float shakeDuration = 5f; // Example duration
    private bool isTransitioning = false;

    private float initialOrthoSize;

    private void Start()
    {
        if (virtualCamera == null)
        {
            Debug.LogError("Virtual Camera is not assigned to ChaseController.");
            return;
        }

        initialOrthoSize = virtualCamera.m_Lens.OrthographicSize;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Ensure the player has this tag
        {
            StartCameraTransition();
            CameraShake.Instance.ShakeCamera(shakeIntensity, shakeDuration);
        }
    }

    private void Update()
    {
        if (isTransitioning)
        {
            SmoothOrthoSizeTransition();
        }
    }

    private void StartCameraTransition()
    {
        isTransitioning = true;
    }

    private void SmoothOrthoSizeTransition()
    {
        float currentSize = virtualCamera.m_Lens.OrthographicSize;
        if (Mathf.Abs(currentSize - targetOrthoSize) > 0.01f)
        {
            virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(currentSize, targetOrthoSize, Time.deltaTime * transitionSpeed);
        }
        else
        {
            virtualCamera.m_Lens.OrthographicSize = targetOrthoSize;
            isTransitioning = false;
        }
    }
}
