using MathRunner.UI.Toolkit;
using UnityEngine;

/// <summary>
/// Deprecated: HUD is owned by UI Toolkit <see cref="MathRunner.UI.Screens.HudScreen"/>.
/// Kept so any lingering scene references do not throw MissingScript errors.
/// </summary>
public class InGameHUD : MonoBehaviour
{
    private void OnEnable()
    {
        // Ensure Toolkit HUD is shown when this legacy component is present.
        if (GameState.IsRunning())
            UIRouter.Instance?.ShowHud();
    }
}
