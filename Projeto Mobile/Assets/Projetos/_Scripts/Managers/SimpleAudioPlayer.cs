using UnityEngine;

public class SimpleAudioPlayer : MonoBehaviour
{
    public static SimpleAudioPlayer Instance;
    private AudioSource source;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        DontDestroyOnLoad(gameObject);

        source = gameObject.AddComponent<AudioSource>();
    }

    public void Play(AudioClip clip, float volume = 1f)
    {
        source.PlayOneShot(clip, volume);
    }
}
