using Cinemachine;
using UnityEngine;

public class ChaseController : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    public float targetOrthoSize = 13f;
    public float transitionSpeed = 2f;
    public float shakeIntensity = 1f;
    public float shakeDuration = 5f;
    public GameObject chaseTentaclePrefab; // Prefab for ChaseTentacle
    public Transform spawnPoint; // Where the tentacle spawns

    [Header("Audio Settings")]
    public AudioSource chaseAudio; // AudioSource for chase music
    public AudioSource bgmAudio;   // Independent AudioSource for BGM in the scene

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

        if (chaseAudio == null)
        {
            Debug.LogError("Chase audio is not assigned.");
        }

        if (bgmAudio == null)
        {
            Debug.LogError("BGM audio is not assigned.");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Ensure the player has this tag
        {
            StartCameraTransition();
            CameraShake.Instance.ShakeCamera(shakeIntensity, shakeDuration);
            SpawnChaseTentacle(other.transform); // Pass the player's transform
            HandleAudio(); // Handle audio changes
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

    private void SpawnChaseTentacle(Transform playerTransform)
    {
        if (chaseTentaclePrefab == null || spawnPoint == null)
        {
            Debug.LogError("ChaseTentaclePrefab or SpawnPoint is not assigned!");
            return;
        }

        // Instantiate the tentacle
        GameObject tentacle = Instantiate(chaseTentaclePrefab, spawnPoint.position, Quaternion.identity);

        // Initialize the tentacle with the player's transform
        ChaseTentacle tentacleScript = tentacle.GetComponent<ChaseTentacle>();
        if (tentacleScript != null)
        {
            tentacleScript.Initialize(playerTransform);
        }
        else
        {
            Debug.LogError("The ChaseTentacle prefab does not have a ChaseTentacle script!");
        }
    }

    private void HandleAudio()
    {
        // Play chase audio
        if (chaseAudio != null && !chaseAudio.isPlaying)
        {
            chaseAudio.Play();
        }

        // Deactivate BGM
        if (bgmAudio != null && bgmAudio.isPlaying)
        {
            bgmAudio.Stop();
        }
    }
}
