using System.Collections;
using MathRunner.UI.Toolkit;
using UnityEngine;
using UnityEngine.UIElements;

namespace MathRunner.UI.Screens
{
    /// <summary>Full-screen fade + loading label for scene transitions.</summary>
    public class TransitionScreen : UIScreen
    {
        public override string ScreenId => "transition";
        public override UILayer Layer => UILayer.Transition;
        public override string UxmlResourcePath => null;

        private VisualElement fade;
        private Label loadingLabel;
        private Coroutine fadeRoutine;

        protected override void OnBind(VisualElement root)
        {
            root.pickingMode = PickingMode.Ignore;
            fade = root.Q("fade");
            loadingLabel = root.Q<Label>("loading-label");
            if (fade != null)
            {
                fade.pickingMode = PickingMode.Ignore;
                fade.style.opacity = 0f;
                fade.RemoveFromClassList("visible");
            }
            if (loadingLabel != null)
                loadingLabel.RemoveFromClassList("visible");
        }

        public void SetLoading(bool visible, string message = "Loading...")
        {
            if (loadingLabel == null) return;
            loadingLabel.text = message;
            loadingLabel.EnableInClassList("visible", visible);
            if (visible)
                FadeOut(0.15f);
            else
                FadeIn(0.25f);
        }

        public void FadeOut(float duration = 0.35f)
        {
            AnimateFade(true, duration);
        }

        public void FadeIn(float duration = 0.35f)
        {
            AnimateFade(false, duration);
        }

        private void AnimateFade(bool toBlack, float duration)
        {
            if (fade == null || UIRouter.Instance == null) return;
            if (fadeRoutine != null) UIRouter.Instance.StopCoroutine(fadeRoutine);
            fadeRoutine = UIRouter.Instance.StartCoroutine(FadeRoutine(toBlack, duration));
        }

        private IEnumerator FadeRoutine(bool toBlack, float duration)
        {
            fade.pickingMode = toBlack ? PickingMode.Position : PickingMode.Ignore;
            float start = fade.resolvedStyle.opacity;
            float end = toBlack ? 1f : 0f;
            float t = 0f;
            fade.EnableInClassList("visible", toBlack);
            while (t < duration)
            {
                t += Time.unscaledDeltaTime;
                float a = Mathf.Lerp(start, end, Mathf.Clamp01(t / duration));
                fade.style.opacity = a;
                yield return null;
            }
            fade.style.opacity = end;
            if (!toBlack) fade.RemoveFromClassList("visible");
            fade.pickingMode = PickingMode.Ignore;
            fadeRoutine = null;
        }
    }
}
