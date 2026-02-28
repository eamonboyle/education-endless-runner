using UnityEngine;

/// <summary>
/// Provides an alternative one-handed input scheme: tapping the left half
/// of the screen moves left, tapping the right half moves right.
/// Integrates with <see cref="MathRunner.Core.InputManager"/> when
/// available, otherwise operates standalone by setting flags compatible
/// with <see cref="PlayerController"/>.
/// </summary>
public class OneHandedMode : MonoBehaviour
{
    [SerializeField, Tooltip("Optional on-screen left button.")]
    private UnityEngine.UI.Button leftButton;

    [SerializeField, Tooltip("Optional on-screen right button.")]
    private UnityEngine.UI.Button rightButton;

    private const string PrefsKey = "Accessibility_OneHanded";

    private bool oneHandedEnabled;

    /// <summary>Mirrors <see cref="PlayerController.swipeLeft"/>.</summary>
    [HideInInspector]
    public bool swipeLeft;

    /// <summary>Mirrors <see cref="PlayerController.swipeRight"/>.</summary>
    [HideInInspector]
    public bool swipeRight;

    private void Start()
    {
        oneHandedEnabled = PlayerPrefs.GetInt(PrefsKey, 0) == 1;

        if (leftButton != null)
        {
            leftButton.onClick.AddListener(() => swipeLeft = true);
        }
        if (rightButton != null)
        {
            rightButton.onClick.AddListener(() => swipeRight = true);
        }
    }

    private void Update()
    {
        swipeLeft = false;
        swipeRight = false;

        if (!oneHandedEnabled) return;

        if (MathRunner.Core.InputManager.Instance != null)
        {
            return;
        }

        bool tapped = false;
        Vector2 tapPosition = Vector2.zero;

        if (Input.GetMouseButtonDown(0))
        {
            tapped = true;
            tapPosition = Input.mousePosition;
        }
        else if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            tapped = true;
            tapPosition = Input.GetTouch(0).position;
        }

        if (!tapped) return;

        float halfScreen = Screen.width * 0.5f;
        if (tapPosition.x < halfScreen)
        {
            swipeLeft = true;
        }
        else
        {
            swipeRight = true;
        }
    }

    /// <summary>Returns whether one-handed mode is enabled.</summary>
    public bool IsEnabled()
    {
        return oneHandedEnabled;
    }

    /// <summary>
    /// Enables or disables one-handed mode and persists the setting.
    /// </summary>
    /// <param name="enabled"><c>true</c> to enable.</param>
    public void SetEnabled(bool enabled)
    {
        oneHandedEnabled = enabled;
        PlayerPrefs.SetInt(PrefsKey, enabled ? 1 : 0);
        PlayerPrefs.Save();

        if (enabled && MathRunner.Core.InputManager.Instance != null)
        {
            MathRunner.Core.InputManager.Instance.SetInputMode(
                MathRunner.Core.InputManager.InputMode.Tap);
        }
    }
}
