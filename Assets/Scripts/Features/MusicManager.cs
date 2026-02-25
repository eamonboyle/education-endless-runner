using System.Collections;
using UnityEngine;

/// <summary>
/// Singleton MonoBehaviour (DontDestroyOnLoad) that manages background music
/// playback. Supports crossfading between tracks, tempo scaling based on
/// character speed, and volume control tied to <see cref="SettingState"/>.
/// </summary>
public class MusicManager : MonoBehaviour
{
    #region Singleton

    /// <summary>Global singleton instance.</summary>
    public static MusicManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        InitAudioSources();
    }

    #endregion

    [Header("Music Clips")]
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip gameplayMusic;
    [SerializeField] private AudioClip gameOverMusic;
    [SerializeField] private AudioClip bossMusic;

    [Header("Settings")]
    [SerializeField, Tooltip("Duration of the crossfade transition in seconds.")]
    private float crossfadeDuration = 1.0f;

    [SerializeField, Tooltip("Minimum pitch applied to gameplay music.")]
    private float minPitch = 0.9f;

    [SerializeField, Tooltip("Maximum pitch applied to gameplay music.")]
    private float maxPitch = 1.3f;

    [SerializeField, Tooltip("Character speed at which maximum pitch is reached.")]
    private float maxSpeedForPitch = 80f;

    private AudioSource sourceA;
    private AudioSource sourceB;
    private AudioSource activeSource;
    private Coroutine crossfadeCoroutine;
    private float volume = 1.0f;
    private bool isPlayingGameplay;

    private void InitAudioSources()
    {
        sourceA = gameObject.AddComponent<AudioSource>();
        sourceB = gameObject.AddComponent<AudioSource>();

        sourceA.loop = true;
        sourceB.loop = true;
        sourceA.playOnAwake = false;
        sourceB.playOnAwake = false;

        activeSource = sourceA;
    }

    private void Update()
    {
        if (!isPlayingGameplay) return;
        if (activeSource == null || !activeSource.isPlaying) return;

        float speed = GameState.GetCharacterSpeed();
        float t = Mathf.InverseLerp(MathRunner.Core.GameConstants.DEFAULT_SPEED, maxSpeedForPitch, speed);
        activeSource.pitch = Mathf.Lerp(minPitch, maxPitch, t);
    }

    /// <summary>Crossfades to the menu music track.</summary>
    public void PlayMenu()
    {
        isPlayingGameplay = false;
        CrossfadeTo(menuMusic);
    }

    /// <summary>Crossfades to the gameplay music track.</summary>
    public void PlayGameplay()
    {
        isPlayingGameplay = true;
        CrossfadeTo(gameplayMusic);
    }

    /// <summary>Crossfades to the game-over music track.</summary>
    public void PlayGameOver()
    {
        isPlayingGameplay = false;
        CrossfadeTo(gameOverMusic);
    }

    /// <summary>Crossfades to the boss encounter music track.</summary>
    public void PlayBoss()
    {
        isPlayingGameplay = false;
        CrossfadeTo(bossMusic);
    }

    /// <summary>Stops all music playback immediately.</summary>
    public void StopAll()
    {
        if (crossfadeCoroutine != null)
        {
            StopCoroutine(crossfadeCoroutine);
            crossfadeCoroutine = null;
        }

        isPlayingGameplay = false;

        if (sourceA != null) { sourceA.Stop(); sourceA.pitch = 1f; }
        if (sourceB != null) { sourceB.Stop(); sourceB.pitch = 1f; }
    }

    /// <summary>
    /// Sets the master music volume. The value is clamped between 0 and 1.
    /// Volume is further gated by <see cref="SettingState.GetSound()"/>.
    /// </summary>
    /// <param name="vol">Volume level (0–1).</param>
    public void SetVolume(float vol)
    {
        volume = Mathf.Clamp01(vol);
        ApplyVolume();
    }

    private void CrossfadeTo(AudioClip clip)
    {
        if (clip == null) return;

        if (activeSource != null && activeSource.clip == clip && activeSource.isPlaying)
        {
            return;
        }

        if (crossfadeCoroutine != null)
        {
            StopCoroutine(crossfadeCoroutine);
        }

        crossfadeCoroutine = StartCoroutine(CrossfadeCoroutine(clip));
    }

    private IEnumerator CrossfadeCoroutine(AudioClip newClip)
    {
        AudioSource fadingOut = activeSource;
        AudioSource fadingIn = (activeSource == sourceA) ? sourceB : sourceA;

        fadingIn.clip = newClip;
        fadingIn.pitch = 1f;
        fadingIn.volume = 0f;
        fadingIn.Play();

        float effectiveVolume = GetEffectiveVolume();
        float elapsed = 0f;

        while (elapsed < crossfadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / crossfadeDuration);

            if (fadingOut != null)
            {
                fadingOut.volume = Mathf.Lerp(effectiveVolume, 0f, t);
            }
            fadingIn.volume = Mathf.Lerp(0f, effectiveVolume, t);

            yield return null;
        }

        if (fadingOut != null)
        {
            fadingOut.Stop();
            fadingOut.clip = null;
            fadingOut.volume = 0f;
        }

        fadingIn.volume = effectiveVolume;
        activeSource = fadingIn;
        crossfadeCoroutine = null;
    }

    private float GetEffectiveVolume()
    {
        bool soundOn = SettingState.GetSound();
        return soundOn ? volume : 0f;
    }

    private void ApplyVolume()
    {
        float effective = GetEffectiveVolume();
        if (activeSource != null && activeSource.isPlaying)
        {
            activeSource.volume = effective;
        }
    }
}
