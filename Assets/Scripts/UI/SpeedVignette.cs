using MathRunner.UI.Toolkit;
using UnityEngine;

/// <summary>
/// Bridge: speed-based edge darkening via Toolkit OverlayScreen vignette.
/// </summary>
public class SpeedVignette : MonoBehaviour
{
    [SerializeField] private float minSpeed = 8f;
    [SerializeField] private float maxSpeed = 20f;

    private void Update()
    {
        if (UIRouter.Instance?.Overlay == null) return;
        if (!GameState.IsRunning())
        {
            UIRouter.Instance.Overlay.SetVignette(0f);
            return;
        }

        float speed = GameState.GetCharacterSpeed();
        float t = Mathf.InverseLerp(minSpeed, maxSpeed, speed);
        UIRouter.Instance.Overlay.SetVignette(t);
    }
}
