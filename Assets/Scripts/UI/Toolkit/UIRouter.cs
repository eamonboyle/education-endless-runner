using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using MathRunner.UI.Screens;

namespace MathRunner.UI.Toolkit
{
    /// <summary>
    /// Central screen switcher for UI Toolkit. Replaces GameObject.Find canvas
    /// toggling. Persistent HUD / overlay / toast / transition layers stay mounted;
    /// modal layer swaps UXML screens via ShowModal / HideModal.
    /// </summary>
    public class UIRouter : MonoBehaviour
    {
        public static UIRouter Instance { get; private set; }

        private readonly Dictionary<string, UIScreen> screens = new Dictionary<string, UIScreen>();
        private UIScreen activeModal;
        private HudScreen hudScreen;
        private OverlayScreen overlayScreen;
        private ToastScreen toastScreen;
        private TransitionScreen transitionScreen;
        private bool toolkitReady;

        /// <summary>True once UIRoot and layered screens are mounted.</summary>
        public bool IsReady => toolkitReady;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            EnsureRoot();
            RegisterScreens();
            MountPersistentLayers();
            toolkitReady = true;
        }

        private void EnsureRoot()
        {
            if (UIRoot.Instance == null)
            {
                var go = new GameObject("[UIRoot]");
                go.AddComponent<UIRoot>();
            }
        }

        private void RegisterScreens()
        {
            hudScreen = new HudScreen();
            overlayScreen = new OverlayScreen();
            toastScreen = new ToastScreen();
            transitionScreen = new TransitionScreen();

            Register(hudScreen);
            Register(overlayScreen);
            Register(toastScreen);
            Register(transitionScreen);
            Register(new PauseScreen());
            Register(new GameOverScreen());
            Register(new MainMenuScreen());
            Register(new CharacterSelectScreen());
            Register(new ModeChoiceScreen());
            Register(new SettingsScreen());
            Register(new StatsScreen());
            Register(new ChallengesScreen());
            Register(new AccessibilityScreen());
            Register(new TutorialCompleteScreen());
            Register(new TutorialGameOverScreen());
            Register(new SessionSummaryScreen());
        }

        private void Register(UIScreen screen)
        {
            screens[screen.ScreenId] = screen;
        }

        private void MountPersistentLayers()
        {
            var root = UIRoot.Instance;
            if (root == null) return;

            hudScreen.Attach(root.HudDocument);
            overlayScreen.Attach(root.OverlayDocument);
            toastScreen.Attach(root.ToastDocument);
            transitionScreen.Attach(root.TransitionDocument);

            // HUD starts hidden until gameplay; overlay/toast/transition stay active.
            hudScreen.Hide();
            ClearModal();
        }

        public void ShowHud()
        {
            hudScreen?.Show();
            ClearModal();
        }

        public void HideHud()
        {
            hudScreen?.Hide();
        }

        public void ShowModal(string screenId)
        {
            if (!screens.TryGetValue(screenId, out var screen))
            {
                Debug.LogWarning($"[UIRouter] Unknown screen: {screenId}");
                return;
            }

            var root = UIRoot.Instance;
            if (root == null) return;

            if (activeModal != null && activeModal != screen)
                activeModal.Hide();

            screen.Attach(root.ModalDocument);
            screen.Show();
            activeModal = screen;
        }

        public void HideModal()
        {
            if (activeModal != null)
            {
                activeModal.Hide();
                activeModal = null;
            }
            ClearModal();
        }

        private void ClearModal()
        {
            var root = UIRoot.Instance?.ModalDocument?.rootVisualElement;
            if (root != null)
            {
                root.Clear();
                root.style.display = DisplayStyle.None;
            }
        }

        public bool IsModalVisible(string screenId)
        {
            return activeModal != null && activeModal.ScreenId == screenId && activeModal.IsVisible;
        }

        public HudScreen Hud => hudScreen;
        public OverlayScreen Overlay => overlayScreen;
        public ToastScreen Toast => toastScreen;
        public TransitionScreen Transition => transitionScreen;

        public void ShowToast(string title, string body, float duration = 2.5f)
        {
            toastScreen?.ShowMessage(title, body, duration);
        }

        public void FlashCorrect()
        {
            overlayScreen?.FlashCorrect();
        }

        public void FlashWrong()
        {
            overlayScreen?.FlashWrong();
        }

        public void ShowHighScoreCelebration()
        {
            overlayScreen?.ShowCelebration();
        }

        public void SetLoading(bool visible, string message = "Loading...")
        {
            transitionScreen?.SetLoading(visible, message);
        }

        public void FadeIn(float duration = 0.35f)
        {
            transitionScreen?.FadeIn(duration);
        }

        public void FadeOut(float duration = 0.35f)
        {
            transitionScreen?.FadeOut(duration);
        }
    }
}
