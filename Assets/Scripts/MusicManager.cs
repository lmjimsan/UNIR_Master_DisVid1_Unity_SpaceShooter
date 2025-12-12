using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] AudioClip backgroundMusic;
    [SerializeField][Range(0f, 1f)] float volume = 0.5f;

    AudioSource audioSource;

    void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = backgroundMusic;
        audioSource.volume = volume;
        audioSource.loop = true;
        audioSource.playOnAwake = true;
        audioSource.Play();
    }
}
