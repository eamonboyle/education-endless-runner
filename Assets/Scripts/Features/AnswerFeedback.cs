using UnityEngine;

/// <summary>
/// Provides visual (particle) and audio feedback when a question is answered.
/// Attach to a persistent manager GameObject and assign the particle system and
/// audio clip references in the Inspector.
/// </summary>
public class AnswerFeedback : MonoBehaviour
{
    /// <summary>Singleton for easy static access from QuestionBox.OnTriggerEnter.</summary>
    public static AnswerFeedback Instance { get; private set; }

    [Header("Correct Answer")]
    [SerializeField, Tooltip("Particle system prefab for a correct answer (green burst).")]
    private ParticleSystem correctParticlePrefab;

    [SerializeField, Tooltip("Audio clip played on a correct answer.")]
    private AudioClip correctSound;

    [Header("Incorrect Answer")]
    [SerializeField, Tooltip("Particle system prefab for an incorrect answer (red burst).")]
    private ParticleSystem incorrectParticlePrefab;

    [SerializeField, Tooltip("Audio clip played on an incorrect answer.")]
    private AudioClip incorrectSound;

    [Header("Runtime")]
    [SerializeField, Tooltip("Shared AudioSource used for feedback sounds.")]
    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    /// <summary>
    /// Play correct-answer feedback at the given world position:
    /// green particle burst + happy sound.
    /// </summary>
    public static void PlayCorrect(Vector3 position)
    {
        if (Instance == null)
        {
            Debug.LogWarning("AnswerFeedback: Instance is null. Cannot play correct feedback.");
            return;
        }

        Instance.SpawnParticles(Instance.correctParticlePrefab, position, new Color(0.2f, 0.9f, 0.2f, 1f));
        Instance.PlayClip(Instance.correctSound);
    }

    /// <summary>
    /// Play incorrect-answer feedback at the given world position:
    /// red particle burst + sad sound.
    /// </summary>
    public static void PlayIncorrect(Vector3 position)
    {
        if (Instance == null)
        {
            Debug.LogWarning("AnswerFeedback: Instance is null. Cannot play incorrect feedback.");
            return;
        }

        Instance.SpawnParticles(Instance.incorrectParticlePrefab, position, new Color(0.9f, 0.2f, 0.2f, 1f));
        Instance.PlayClip(Instance.incorrectSound);
    }

    private void SpawnParticles(ParticleSystem prefab, Vector3 position, Color fallbackColor)
    {
        if (prefab != null)
        {
            ParticleSystem ps = Instantiate(prefab, position, Quaternion.identity);
            ps.Play();
            Destroy(ps.gameObject, ps.main.duration + ps.main.startLifetime.constantMax);
        }
        else
        {
            // Spawn a runtime particle system with the fallback colour
            GameObject go = new GameObject("AnswerFeedbackParticles");
            go.transform.position = position;

            ParticleSystem ps = go.AddComponent<ParticleSystem>();
            var main = ps.main;
            main.startColor = fallbackColor;
            main.startLifetime = 0.6f;
            main.startSpeed = 4f;
            main.maxParticles = 30;
            main.duration = 0.3f;
            main.loop = false;

            var emission = ps.emission;
            emission.rateOverTime = 0f;
            emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 30) });

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Sphere;
            shape.radius = 0.3f;

            ps.Play();
            Destroy(go, main.duration + main.startLifetime.constantMax + 0.1f);
        }
    }

    private void PlayClip(AudioClip clip)
    {
        if (clip == null || audioSource == null) return;
        audioSource.PlayOneShot(clip);
    }
}
