using System.Collections;
using MathRunner.UI.Toolkit;
using UnityEngine;
using UnityEngine.UIElements;

namespace MathRunner.UI.Screens
{
    /// <summary>Sliding toast notifications for achievements and unlocks.</summary>
    public class ToastScreen : UIScreen
    {
        public override string ScreenId => "toast";
        public override UILayer Layer => UILayer.Toast;
        public override string UxmlResourcePath => null;

        private VisualElement toast;
        private Label titleLabel;
        private Label bodyLabel;
        private Coroutine hideRoutine;

        protected override void OnBind(VisualElement root)
        {
            root.pickingMode = PickingMode.Ignore;
            toast = root.Q("toast");
            titleLabel = root.Q<Label>("toast-title");
            bodyLabel = root.Q<Label>("toast-body");
            if (toast != null) toast.pickingMode = PickingMode.Ignore;
        }

        public void ShowMessage(string title, string body, float duration = 2.5f)
        {
            if (toast == null || UIRouter.Instance == null) return;
            if (titleLabel != null) titleLabel.text = title ?? "";
            if (bodyLabel != null) bodyLabel.text = body ?? "";
            toast.AddToClassList("visible");

            if (hideRoutine != null) UIRouter.Instance.StopCoroutine(hideRoutine);
            hideRoutine = UIRouter.Instance.StartCoroutine(HideAfter(duration));
        }

        private IEnumerator HideAfter(float duration)
        {
            yield return new WaitForSeconds(duration);
            toast?.RemoveFromClassList("visible");
            hideRoutine = null;
        }
    }
}
