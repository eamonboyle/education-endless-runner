using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UIElements;

namespace MathRunner.Tests
{
    [TestFixture]
    public class UxmlIntegrityTests
    {
        private static readonly Dictionary<string, string[]> RequiredElements =
            new Dictionary<string, string[]>
            {
                { "UI/Screens/hud", new[] { "lives-label", "score-label", "combo-label", "speed-label", "pause-button", "powerup-0", "question-label", "countdown-label" } },
                { "UI/Screens/overlay", new[] { "flash", "vignette", "celebration" } },
                { "UI/Screens/toast", new[] { "toast", "toast-title", "toast-body" } },
                { "UI/Screens/transition", new[] { "fade", "loading-label" } },
                { "UI/Screens/pause", new[] { "continue-button", "quit-button" } },
                { "UI/Screens/game_over", new[] { "current-score", "high-score", "restart-button", "quit-button", "history-button" } },
                { "UI/Screens/main_menu", new[] { "play-button", "mode-button", "settings-button", "stats-button" } },
                { "UI/Screens/character_select", new[] { "boy-button", "girl-button", "back-button" } },
                { "UI/Screens/mode_choice", new[] { "addition-button", "play-button", "classic-card", "easy-chip" } },
                { "UI/Screens/settings", new[] { "sound-button", "home-button", "accessibility-button" } },
                { "UI/Screens/stats", new[] { "games-played", "graph-row", "back-button" } },
                { "UI/Screens/challenges", new[] { "daily-desc", "weekly-fill", "back-button" } },
                { "UI/Screens/accessibility", new[] { "reduced-motion", "high-contrast", "back-button", "status-label" } },
                { "UI/Screens/tutorial_complete", new[] { "menu-button" } },
                { "UI/Screens/tutorial_gameover", new[] { "restart-button" } },
                { "UI/Screens/session_summary", new[] { "summary-score", "history-list", "close-button" } },
            };

        [Test]
        public void StyleSheets_ExistInResources()
        {
            Assert.IsNotNull(Resources.Load<StyleSheet>("UI/Styles/tokens"), "tokens.uss missing");
            Assert.IsNotNull(Resources.Load<StyleSheet>("UI/Styles/components"), "components.uss missing");
            Assert.IsNotNull(Resources.Load<StyleSheet>("UI/Styles/accessibility"), "accessibility.uss missing");
        }

        [Test]
        public void AllScreens_ContainRequiredElementNames()
        {
            foreach (var pair in RequiredElements)
            {
                var asset = Resources.Load<VisualTreeAsset>(pair.Key);
                Assert.IsNotNull(asset, $"Missing UXML resource: {pair.Key}");

                var root = asset.Instantiate();
                Assert.IsNotNull(root, $"Failed to instantiate {pair.Key}");

                foreach (string name in pair.Value)
                {
                    var el = root.Q(name);
                    Assert.IsNotNull(el, $"{pair.Key} is missing element '{name}'");
                }
            }
        }
    }
}
