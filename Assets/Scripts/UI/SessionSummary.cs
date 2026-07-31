using MathRunner.UI.Toolkit;
using UnityEngine;

/// <summary>
/// Bridge: session summary is a Toolkit modal.
/// </summary>
public class SessionSummary : MonoBehaviour
{
    public void ShowSummary()
    {
        UIRouter.Instance?.ShowModal("session_summary");
    }

    public static void Show()
    {
        UIRouter.Instance?.ShowModal("session_summary");
    }
}
