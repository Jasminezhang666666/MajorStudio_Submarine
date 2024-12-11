using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

public class EndingTrigger : MonoBehaviour
{
    [Header("Audio References")]
    [Tooltip("The initial audio source played when the trigger is entered.")]
    public AudioSource TriggerVoiceOver;
    [Tooltip("The audio source played after zooming STARTS.")]
    public AudioSource GradualCreepy;
    [Tooltip("The audio source played after zooming is done.")]
    public AudioSource VeryCreepy;

    [Header("Camera Settings")]
    [Tooltip("The Cinemachine Virtual Camera to zoom out and reposition.")]
    public CinemachineVirtualCamera virtualCamera;
    [Tooltip("The target orthographic size to reach.")]
    public float targetOrthoSize = 50f;
    [Tooltip("The duration of the zoom out.")]
    public float zoomDuration = 8f;

    [Header("Camera Movement")]
    [Tooltip("If true, the camera will stop following the player.")]
    public bool stopFollowing = true;
    [Tooltip("How far to move the camera on the X axis (negative to move left).")]
    public float cameraMoveX = -10f;
    [Tooltip("How far to move the camera on the Y axis.")]
    public float cameraMoveY = 0f;

    [Header("Timing")]
    [Tooltip("Time before starting the zoom after initial audio plays.")]
    public float initialWaitTime = 3.5f;
    [Tooltip("Time to wait after playing the second audio before loading the 'End' scene.")]
    public float finalWaitTime = 3f;

    [Header("Scene Settings")]
    [Tooltip("Name of the scene to load at the end.")]
    public string endSceneName = "End";

    private bool hasTriggered = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!hasTriggered && other.CompareTag("Player"))
        {
            hasTriggered = true;
            StartCoroutine(EndSequenceRoutine());
        }
    }

    private IEnumerator EndSequenceRoutine()
    {
        if (TriggerVoiceOver != null)
        {
            TriggerVoiceOver.Play();
        }

        // Wait for initial audio duration
        yield return new WaitForSeconds(initialWaitTime);

        if (GradualCreepy != null)
        {
            GradualCreepy.Play();
        }

        // Stop camera from following the player
        if (stopFollowing && virtualCamera != null)
        {
            virtualCamera.Follow = null;
            virtualCamera.LookAt = null;
        }

        // Zoom out and move the camera
        if (virtualCamera != null)
        {
            float startSize = virtualCamera.m_Lens.OrthographicSize;
            float endSize = targetOrthoSize;

            Vector3 startPos = virtualCamera.transform.position;
            Vector3 endPos = startPos + new Vector3(cameraMoveX, cameraMoveY, 0f);

            float elapsed = 0f;

            while (elapsed < zoomDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / zoomDuration);

                // Lerp camera size
                virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(startSize, endSize, t);

                // Lerp camera position
                virtualCamera.transform.position = Vector3.Lerp(startPos, endPos, t);

                yield return null;
            }

            // Ensure final values are set
            virtualCamera.m_Lens.OrthographicSize = endSize;
            virtualCamera.transform.position = endPos;
        }

        // Play the second audio after zooming
        if (VeryCreepy != null)
        {
            VeryCreepy.Play();
        }

        // Wait before loading the end scene
        yield return new WaitForSeconds(finalWaitTime);

        SceneManager.LoadScene(endSceneName);
    }
}
