using UnityEngine;

public class StopChasing : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Stop all tentacles
            ChaseTentacle[] tentacles = FindObjectsOfType<ChaseTentacle>();
            foreach (var tentacle in tentacles)
            {
                tentacle.StopAndDestroy();
            }

            // Stop and disable all audio sources
            StopAndDisableAllGameAudio();
        }
    }

    private void StopAndDisableAllGameAudio()
    {
        // Find all AudioSources in the scene
        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();
        foreach (var audioSource in allAudioSources)
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }

            // Disable the AudioSource to prevent re-triggering
            audioSource.enabled = false;
        }

        Debug.Log("All game sounds have been stopped and audio sources disabled.");
    }
}
