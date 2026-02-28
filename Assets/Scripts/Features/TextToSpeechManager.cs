using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Singleton that reads math questions aloud by playing pre-recorded
/// <see cref="AudioClip"/> sequences for numbers (0–100) and operators
/// (+, -, x, ÷). Falls back gracefully when clips are not assigned.
/// </summary>
public class TextToSpeechManager : MonoBehaviour
{
    #region Singleton

    /// <summary>Global singleton instance.</summary>
    public static TextToSpeechManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadSetting();
        BuildOperatorMap();
    }

    #endregion

    [SerializeField, Tooltip("Audio clips for numbers 0–100 (index = number).")]
    private AudioClip[] numberClips = new AudioClip[101];

    [SerializeField, Tooltip("Audio clip for the '+' operator.")]
    private AudioClip clipPlus;

    [SerializeField, Tooltip("Audio clip for the '-' operator.")]
    private AudioClip clipMinus;

    [SerializeField, Tooltip("Audio clip for the 'x' (multiply) operator.")]
    private AudioClip clipMultiply;

    [SerializeField, Tooltip("Audio clip for the '÷' (divide) operator.")]
    private AudioClip clipDivide;

    [SerializeField, Tooltip("Delay in seconds between spoken components.")]
    private float componentDelay = 0.3f;

    private const string PrefsKey = "Accessibility_TTS";

    private AudioSource audioSource;
    private bool ttsEnabled;
    private Dictionary<string, AudioClip> operatorMap;
    private Coroutine speakCoroutine;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    /// <summary>
    /// Parses the question text (e.g. "5 + 3") into components and plays
    /// the corresponding audio clips sequentially.
    /// </summary>
    /// <param name="questionText">A string like "12 x 4".</param>
    public void SpeakQuestion(string questionText)
    {
        if (!ttsEnabled) return;

        if (string.IsNullOrEmpty(questionText))
        {
            Debug.Log("TextToSpeechManager: Empty question text.");
            return;
        }

        if (speakCoroutine != null)
        {
            StopCoroutine(speakCoroutine);
        }

        speakCoroutine = StartCoroutine(SpeakSequence(questionText));
    }

    /// <summary>
    /// Enables or disables text-to-speech and persists the setting.
    /// </summary>
    /// <param name="enabled"><c>true</c> to enable.</param>
    public void SetEnabled(bool enabled)
    {
        ttsEnabled = enabled;
        PlayerPrefs.SetInt(PrefsKey, enabled ? 1 : 0);
        PlayerPrefs.Save();
    }

    /// <summary>Returns whether TTS is currently enabled.</summary>
    public bool IsEnabled()
    {
        return ttsEnabled;
    }

    private IEnumerator SpeakSequence(string text)
    {
        string[] parts = text.Split(new[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);

        foreach (string part in parts)
        {
            AudioClip clip = ResolveClip(part);

            if (clip != null && audioSource != null)
            {
                audioSource.PlayOneShot(clip);
                yield return new WaitForSeconds(clip.length + componentDelay);
            }
            else
            {
                Debug.Log($"TextToSpeechManager: No clip for '{part}', skipping.");
                yield return new WaitForSeconds(componentDelay);
            }
        }

        speakCoroutine = null;
    }

    private AudioClip ResolveClip(string token)
    {
        int number;
        if (int.TryParse(token, out number) && number >= 0 && number < numberClips.Length)
        {
            return numberClips[number];
        }

        if (operatorMap != null && operatorMap.ContainsKey(token))
        {
            return operatorMap[token];
        }

        return null;
    }

    private void BuildOperatorMap()
    {
        operatorMap = new Dictionary<string, AudioClip>
        {
            { "+",  clipPlus },
            { "-",  clipMinus },
            { "x",  clipMultiply },
            { "×",  clipMultiply },
            { "÷",  clipDivide },
            { "/",  clipDivide }
        };
    }

    private void LoadSetting()
    {
        ttsEnabled = PlayerPrefs.GetInt(PrefsKey, 0) == 1;
    }
}
