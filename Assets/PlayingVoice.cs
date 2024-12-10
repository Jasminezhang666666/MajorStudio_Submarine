using UnityEngine;

public class PlayingVoice : MonoBehaviour
{
    [SerializeField] private AudioSource voiceAudioSource;
    private bool hasPlayed = false; 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!hasPlayed && collision.CompareTag("Player")) 
        {
            if (voiceAudioSource != null && voiceAudioSource.clip != null) 
            {
                voiceAudioSource.Play(); 
                hasPlayed = true; 
            }
            else
            {
                Debug.LogWarning("AudioSource or AudioClip is missing!");
            }
        }
    }
}
