using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Library")]
    [SerializeField] private AudioLibrary library;

    [Header("Sources")]
    [SerializeField] private int sfxPoolSize = 5;

    private AudioSource musicSource;
    private List<AudioSource> sfxSources = new();

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Music source
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;

        // SFX pool
        for (int i = 0; i < sfxPoolSize; i++)
        {
            var src = gameObject.AddComponent<AudioSource>();
            sfxSources.Add(src);
        }
    }

    public void PlayMusic(AudioID id)
    {
        var data = library.Get(id);
        musicSource.clip = data.clip;
        musicSource.volume = data.volume;
        musicSource.Play();
    }

    public void PlaySFX(AudioID id)
    {
        var data = library.Get(id);
        var src = GetFreeSFXSource();
        src.clip = data.clip;
        src.volume = data.volume;
        src.Play();
    }

    private AudioSource GetFreeSFXSource()
    {
        foreach (var src in sfxSources)
            if (!src.isPlaying)
                return src;

        return sfxSources[0]; // fallback seguro
    }
}
