using System.Collections;
using MathRunner.UI.Toolkit;
using UnityEngine;
using UnityEngine.UIElements;

namespace MathRunner.UI.Screens
{
    /// <summary>
    /// Full-screen flash, speed vignette, and high-score celebration overlays.
    /// </summary>
    public class OverlayScreen : UIScreen
    {
        public override string ScreenId => "overlay";
        public override UILayer Layer => UILayer.Overlay;
        public override string UxmlResourcePath => null;

        private VisualElement flash;
        private VisualElement vignette;
        private Label celebration;
        private Coroutine flashRoutine;
        private Coroutine celebrationRoutine;
        private float vignetteIntensity;

        protected override void OnBind(VisualElement root)
        {
            root.pickingMode = PickingMode.Ignore;
            flash = root.Q("flash");
            vignette = root.Q("vignette");
            celebration = root.Q<Label>("celebration");
            if (flash != null) flash.pickingMode = PickingMode.Ignore;
            if (vignette != null) vignette.pickingMode = PickingMode.Ignore;
            if (celebration != null) celebration.pickingMode = PickingMode.Ignore;
        }

        public void FlashCorrect()
        {
            if (IsReducedMotion()) return;
            StartFlash("flash-correct", 0.25f);
        }

        public void FlashWrong()
        {
            if (IsReducedMotion()) return;
            StartFlash("flash-wrong", 0.3f);
        }

        private void StartFlash(string className, float duration)
        {
            if (flash == null || UIRouter.Instance == null) return;
            if (flashRoutine != null) UIRouter.Instance.StopCoroutine(flashRoutine);
            flashRoutine = UIRouter.Instance.StartCoroutine(FlashRoutine(className, duration));
        }

        private IEnumerator FlashRoutine(string className, float duration)
        {
            flash.RemoveFromClassList("flash-correct");
            flash.RemoveFromClassList("flash-wrong");
            flash.AddToClassList(className);
            yield return new WaitForSeconds(duration);
            flash.RemoveFromClassList(className);
            flashRoutine = null;
        }

        public void ShowCelebration()
        {
            if (celebration == null || UIRouter.Instance == null) return;
            if (celebrationRoutine != null) UIRouter.Instance.StopCoroutine(celebrationRoutine);
            celebrationRoutine = UIRouter.Instance.StartCoroutine(CelebrationRoutine());
        }

        private IEnumerator CelebrationRoutine()
        {
            celebration.AddToClassList("visible");
            float t = 0f;
            while (t < 2.5f)
            {
                t += Time.deltaTime;
                bool gold = Mathf.FloorToInt(t * 4f) % 2 == 0;
                celebration.style.color = gold
                    ? new StyleColor(new Color(1f, 0.84f, 0.25f))
                    : new StyleColor(Color.white);
                yield return null;
            }
            celebration.RemoveFromClassList("visible");
            celebrationRoutine = null;
        }

        public void SetVignette(float intensity)
        {
            vignetteIntensity = Mathf.Clamp01(intensity);
            if (vignette == null) return;
            float width = Mathf.Lerp(0f, 48f, vignetteIntensity);
            float alpha = Mathf.Lerp(0f, 0.55f, vignetteIntensity);
            vignette.style.borderLeftWidth = width;
            vignette.style.borderRightWidth = width;
            vignette.style.borderTopWidth = width * 0.5f;
            vignette.style.borderBottomWidth = width * 0.5f;
            vignette.style.borderLeftColor = new Color(0f, 0f, 0f, alpha);
            vignette.style.borderRightColor = new Color(0f, 0f, 0f, alpha);
            vignette.style.borderTopColor = new Color(0f, 0f, 0f, alpha);
            vignette.style.borderBottomColor = new Color(0f, 0f, 0f, alpha);
        }

        private static bool IsReducedMotion()
        {
            return ReducedMotionManager.Instance != null
                && ReducedMotionManager.Instance.IsReducedMotion();
        }
    }
}
