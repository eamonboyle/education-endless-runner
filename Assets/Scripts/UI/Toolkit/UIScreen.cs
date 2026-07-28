using UnityEngine;
using UnityEngine.UIElements;

namespace MathRunner.UI.Toolkit
{
    /// <summary>
    /// Base class for a screen that lives inside a UIDocument layer.
    /// Subclasses override OnShow/OnHide and wire ClickEvents in OnBind.
    /// </summary>
    public abstract class UIScreen
    {
        public abstract string ScreenId { get; }
        public abstract UILayer Layer { get; }
        public abstract string UxmlResourcePath { get; }

        public bool IsVisible { get; private set; }
        protected VisualElement Root { get; private set; }
        protected UIDocument Document { get; private set; }

        public void Attach(UIDocument document)
        {
            Document = document;
            if (UIRoot.Instance != null)
                UIRoot.Instance.ApplyTheme(document);

            Root = document.rootVisualElement;
            // Screens with their own UXML bind in Show() after CloneTree.
            if (string.IsNullOrEmpty(UxmlResourcePath))
                OnBind(Root);
        }

        public void Show()
        {
            if (Document == null) return;

            if (!string.IsNullOrEmpty(UxmlResourcePath))
            {
                var asset = Resources.Load<VisualTreeAsset>(UxmlResourcePath);
                if (asset != null)
                {
                    Document.rootVisualElement.Clear();
                    asset.CloneTree(Document.rootVisualElement);
                    if (UIRoot.Instance != null)
                        UIRoot.Instance.ApplyTheme(Document);
                    Root = Document.rootVisualElement;
                    OnBind(Root);
                }
            }
            else
            {
                Root = Document.rootVisualElement;
            }

            if (Root != null)
                Root.style.display = DisplayStyle.Flex;

            IsVisible = true;
            OnShow();
        }

        public void Hide()
        {
            if (Root == null) return;
            OnHide();
            Root.style.display = DisplayStyle.None;
            IsVisible = false;
        }

        protected virtual void OnBind(VisualElement root) { }
        protected virtual void OnShow() { }
        protected virtual void OnHide() { }

        protected T Q<T>(string name) where T : VisualElement
        {
            return Root?.Q<T>(name);
        }

        protected VisualElement Q(string name)
        {
            return Root?.Q(name);
        }

        protected static string L(string key)
        {
            if (MathRunner.Core.LocalizationManager.Instance != null)
                return MathRunner.Core.LocalizationManager.Instance.GetString(key);
            return key;
        }
    }
}
