using System;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    [Range(0f, 1f)]
    public float volume = 1f;
    [Range(0.1f, 3f)]
    public float pitch = 1f;
    public bool loop = false;
}

[System.Serializable]
public class SceneMusic
{
    public string sceneName;
    public string musicName;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Sounds")]
    public Sound[] musicSounds;
    public Sound[] sfxSounds;

    //this we can add the name of the music we add from the sounds section and map it into a scene
    //but you have to be very specific on the scene so you cant do Main Menu if your scene is MainMenu you have to be exact
    [Header("Scene Music Settings")]
    public SceneMusic[] sceneMusicMap;
    public string defaultMusic = "Menu Background Music";
    public bool playMusicOnUnknownScenes = true;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        HandleSceneMusic(SceneManager.GetActiveScene().name);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Loaded Scene: {scene.name}");
        HandleSceneMusic(scene.name);
    }

    private void HandleSceneMusic(string sceneName)
    {
        musicSource.Stop();

        SceneMusic sceneMusic = Array.Find(sceneMusicMap, sm => sm.sceneName == sceneName);

        if (sceneMusic != null && !string.IsNullOrEmpty(sceneMusic.musicName))
        {
            PlayMusic(sceneMusic.musicName);
        }
        else if (playMusicOnUnknownScenes && !string.IsNullOrEmpty(defaultMusic))
        {
            PlayMusic(defaultMusic);
        }
    }

    public void PlayMusic(string name)
    {
        Sound s = Array.Find(musicSounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning($"Sound: {name} not found!");
            return;
        }
        //these settings allow you to customize each music clip you add to the inspector
        musicSource.clip = s.clip;
        musicSource.volume = s.volume;
        musicSource.pitch = s.pitch;
        musicSource.loop = s.loop;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void PauseMusic()
    {
        musicSource.Pause();
    }

    public void UnpauseMusic()
    {
        musicSource.UnPause();
    }

    public void PlaySFX(string name)
    {
        Sound s = Array.Find(sfxSounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning($"SFX: {name} not found!");
            return;
        }
        //these settings allow you to customize each SFX clip you add to the inspector
        sfxSource.volume = s.volume;
        sfxSource.pitch = s.pitch;
        sfxSource.PlayOneShot(s.clip);
    }

    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip == null || sfxSource == null)
            return;

        sfxSource.PlayOneShot(clip, volume);
    }

    public void ToggleMusic()
    {
        musicSource.mute = !musicSource.mute;
    }

    public void ToggleSFX()
    {
        sfxSource.mute = !sfxSource.mute;
    }

    public void SetMusicVolume(float volume)
    {
        musicSource.volume = Mathf.Clamp01(volume);
    }

    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = Mathf.Clamp01(volume);
    }
    //These are all set sfx that you can set the name on the inspector plus the sound and it will play it
    public void PlayButtonClick()
    {
        PlaySFX("Button Click");
    }

    public void PlayJumpSound()
    {
        PlaySFX("Jump");
    }

    public void PlayWalkSound()
    {
        PlaySFX("Walk");
    }

    public void PlayAttackSound()
    {
        PlaySFX("Attack");
    }

    public void PlayPickupSound()
    {
        PlaySFX("Pickup");
    }

    public void PlayDashSound()
    {
        PlaySFX("Dash");
    }

    public void PlayLandingSound()
    {
        PlaySFX("Landing");
    }

    public void AddSceneMusic(string sceneName, string musicName)
    {
        SceneMusic existing = Array.Find(sceneMusicMap, sm => sm.sceneName == sceneName);
        if (existing != null)
        {
            existing.musicName = musicName;
        }
    }
}