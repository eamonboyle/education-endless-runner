using MathRunner.UI.Toolkit;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MathRunner.UI.Toolkit
{
    /// <summary>
    /// Plain C# navigation service replacing Inspector-wired SceneSwitcher calls.
    /// </summary>
    public static class NavigationService
    {
        public static void GoToMainMenu()
        {
            UIRouter.Instance?.HideHud();
            UIRouter.Instance?.HideModal();
            if (GameManager.instance != null)
                GameManager.instance.LoadMainMenu();
            else
                Debug.LogError("NavigationService: GameManager.instance is null.");
        }

        public static void GoToModeSelect()
        {
            UIRouter.Instance?.HideHud();
            if (GameManager.instance != null)
                GameManager.instance.LoadModeSelect();
        }

        public static void GoToCharacterSelect()
        {
            UIRouter.Instance?.HideHud();
            if (GameManager.instance != null)
                GameManager.instance.LoadCharacterSelection();
        }

        public static void GoToSettings()
        {
            UIRouter.Instance?.HideHud();
            if (GameManager.instance != null)
                GameManager.instance.LoadSettings();
        }

        public static void GoToTutorial()
        {
            UIRouter.Instance?.HideModal();
            if (GameManager.instance != null)
                GameManager.instance.LoadTutorial();
        }

        public static void GoToGame()
        {
            UIRouter.Instance?.HideModal();
            GameSession.BeginRun();
        }

        public static void ShowMainMenuModal()
        {
            UIRouter.Instance?.HideHud();
            UIRouter.Instance?.ShowModal("main_menu");
        }

        public static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (UIRouter.Instance == null || !UIRouter.Instance.IsReady) return;

            switch (scene.buildIndex)
            {
                case (int)SceneIndexes.MAIN_MENU:
                    DisableSceneCanvases();
                    UIRouter.Instance.ShowModal("main_menu");
                    break;
                case (int)SceneIndexes.CHARACTER_SELECT:
                    DisableSceneCanvases();
                    UIRouter.Instance.ShowModal("character_select");
                    break;
                case (int)SceneIndexes.MODE_CHOICE:
                    DisableSceneCanvases();
                    UIRouter.Instance.ShowModal("mode_choice");
                    break;
                case (int)SceneIndexes.SETTINGS:
                    DisableSceneCanvases();
                    UIRouter.Instance.ShowModal("settings");
                    break;
                case (int)SceneIndexes.GAME:
                case (int)SceneIndexes.TUTORIAL:
                    UIRouter.Instance.HideModal();
                    DisableSceneCanvases();
                    break;
            }

            ApplyAccessibilityFromPrefs();
        }

        private static void DisableSceneCanvases()
        {
            var canvases = Object.FindObjectsByType<Canvas>();
            foreach (var canvas in canvases)
            {
                if (canvas == null) continue;
                if (canvas.renderMode == RenderMode.WorldSpace) continue;
                canvas.enabled = false;
            }
        }

        private static void ApplyAccessibilityFromPrefs()
        {
            bool hc = AccessibilityManager.Instance != null && AccessibilityManager.Instance.HighContrastMode;
            bool rm = ReducedMotionManager.Instance != null && ReducedMotionManager.Instance.IsReducedMotion();
            bool dyslexia = PlayerPrefs.GetInt("Accessibility_DyslexiaFont", 0) == 1;
            float scale = AccessibilityManager.Instance != null ? AccessibilityManager.Instance.GetTextScale() : 1f;
            UIRoot.Instance?.ApplyAccessibilityClasses(hc, rm, dyslexia, scale);
        }
    }
}
