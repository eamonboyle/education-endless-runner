using System.Collections.Generic;
using UnityEngine;

namespace MathRunner.Core
{
    /// <summary>
    /// Singleton that manages string localisation. Supports multiple languages
    /// with a built-in string table and falls back to English when a translation
    /// is missing. The active language is persisted in PlayerPrefs.
    /// </summary>
    public class LocalizationManager : MonoBehaviour
    {
        #region Singleton

        /// <summary>Global singleton instance.</summary>
        public static LocalizationManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadLanguagePreference();
            InitStringTable();
        }

        #endregion

        /// <summary>Supported display languages.</summary>
        public enum Language
        {
            English,
            Spanish,
            French,
            German,
            Portuguese,
            Japanese,
            Chinese,
            Korean
        }

        private const string LanguagePrefsKey = "Localization_Language";

        private Language currentLanguage = Language.English;
        private Dictionary<string, Dictionary<Language, string>> stringTable;

        /// <summary>
        /// Returns the localised string for <paramref name="key"/> in the
        /// current language. Falls back to English if no translation exists.
        /// Returns the key itself if the key is not found at all.
        /// </summary>
        /// <param name="key">The string table key.</param>
        /// <returns>Localised text.</returns>
        public string GetString(string key)
        {
            if (stringTable == null)
            {
                InitStringTable();
            }

            if (!stringTable.ContainsKey(key))
            {
                Debug.LogWarning($"LocalizationManager: Key '{key}' not found in string table.");
                return key;
            }

            Dictionary<Language, string> translations = stringTable[key];

            if (translations.ContainsKey(currentLanguage))
            {
                return translations[currentLanguage];
            }

            if (translations.ContainsKey(Language.English))
            {
                return translations[Language.English];
            }

            return key;
        }

        /// <summary>
        /// Sets the active language and persists the choice.
        /// </summary>
        /// <param name="language">The desired language.</param>
        public void SetLanguage(Language language)
        {
            currentLanguage = language;
            PlayerPrefs.SetInt(LanguagePrefsKey, (int)language);
            PlayerPrefs.Save();
        }

        /// <summary>Returns the currently active language.</summary>
        public Language GetCurrentLanguage()
        {
            return currentLanguage;
        }

        private void LoadLanguagePreference()
        {
            int stored = PlayerPrefs.GetInt(LanguagePrefsKey, (int)Language.English);
            if (stored < 0 || stored > 7) stored = (int)Language.English;
            currentLanguage = (Language)stored;
        }

        private void InitStringTable()
        {
            stringTable = new Dictionary<string, Dictionary<Language, string>>();

            // --- Menu Labels ---
            Add("menu_play", new Dictionary<Language, string>
            {
                { Language.English,    "Play" },
                { Language.Spanish,    "Jugar" },
                { Language.French,     "Jouer" },
                { Language.German,     "Spielen" },
                { Language.Portuguese, "Jogar" },
                { Language.Japanese,   "TODO: translate" },
                { Language.Chinese,    "TODO: translate" },
                { Language.Korean,     "TODO: translate" }
            });

            Add("menu_settings", new Dictionary<Language, string>
            {
                { Language.English,    "Settings" },
                { Language.Spanish,    "Ajustes" },
                { Language.French,     "Paramètres" },
                { Language.German,     "Einstellungen" },
                { Language.Portuguese, "Configurações" },
                { Language.Japanese,   "TODO: translate" },
                { Language.Chinese,    "TODO: translate" },
                { Language.Korean,     "TODO: translate" }
            });

            Add("menu_quit", new Dictionary<Language, string>
            {
                { Language.English,    "Quit" },
                { Language.Spanish,    "Salir" },
                { Language.French,     "Quitter" },
                { Language.German,     "Beenden" },
                { Language.Portuguese, "Sair" },
                { Language.Japanese,   "TODO: translate" },
                { Language.Chinese,    "TODO: translate" },
                { Language.Korean,     "TODO: translate" }
            });

            Add("menu_leaderboard", new Dictionary<Language, string>
            {
                { Language.English, "Leaderboard" },
                { Language.Spanish, "Clasificación" },
                { Language.French,  "Classement" },
                { Language.German,  "Bestenliste" },
                { Language.Portuguese, "Classificação" },
                { Language.Japanese, "TODO: translate" },
                { Language.Chinese,  "TODO: translate" },
                { Language.Korean,   "TODO: translate" }
            });

            Add("menu_mode", new Dictionary<Language, string>
            {
                { Language.English, "Mode" },
                { Language.Spanish, "Modo" },
                { Language.French, "Mode" },
                { Language.German, "Modus" },
                { Language.Portuguese, "Modo" },
                { Language.Japanese, "TODO: translate" },
                { Language.Chinese, "TODO: translate" },
                { Language.Korean, "TODO: translate" }
            });

            Add("menu_character", new Dictionary<Language, string>
            {
                { Language.English, "Character" },
                { Language.Spanish, "Personaje" },
                { Language.French, "Personnage" },
                { Language.German, "Charakter" },
                { Language.Portuguese, "Personagem" },
                { Language.Japanese, "TODO: translate" },
                { Language.Chinese, "TODO: translate" },
                { Language.Korean, "TODO: translate" }
            });

            Add("menu_stats", new Dictionary<Language, string>
            {
                { Language.English, "Stats" },
                { Language.Spanish, "Estadísticas" },
                { Language.French, "Stats" },
                { Language.German, "Statistik" },
                { Language.Portuguese, "Estatísticas" },
                { Language.Japanese, "TODO: translate" },
                { Language.Chinese, "TODO: translate" },
                { Language.Korean, "TODO: translate" }
            });

            Add("menu_challenges", new Dictionary<Language, string>
            {
                { Language.English, "Challenges" },
                { Language.Spanish, "Desafíos" },
                { Language.French, "Défis" },
                { Language.German, "Herausforderungen" },
                { Language.Portuguese, "Desafios" },
                { Language.Japanese, "TODO: translate" },
                { Language.Chinese, "TODO: translate" },
                { Language.Korean, "TODO: translate" }
            });

            Add("menu_title", new Dictionary<Language, string>
            {
                { Language.English, "Math Runner" },
                { Language.Spanish, "Math Runner" },
                { Language.French, "Math Runner" },
                { Language.German, "Math Runner" },
                { Language.Portuguese, "Math Runner" },
                { Language.Japanese, "TODO: translate" },
                { Language.Chinese, "TODO: translate" },
                { Language.Korean, "TODO: translate" }
            });

            Add("ui_loading", new Dictionary<Language, string>
            {
                { Language.English, "Loading..." },
                { Language.Spanish, "Cargando..." },
                { Language.French, "Chargement..." },
                { Language.German, "Laden..." },
                { Language.Portuguese, "Carregando..." },
                { Language.Japanese, "TODO: translate" },
                { Language.Chinese, "TODO: translate" },
                { Language.Korean, "TODO: translate" }
            });

            // --- Mode Names ---
            Add("mode_addition", new Dictionary<Language, string>
            {
                { Language.English,    "Addition" },
                { Language.Spanish,    "Suma" },
                { Language.French,     "Addition" },
                { Language.German,     "Addition" },
                { Language.Portuguese, "Adição" },
                { Language.Japanese,   "TODO: translate" },
                { Language.Chinese,    "TODO: translate" },
                { Language.Korean,     "TODO: translate" }
            });

            Add("mode_subtraction", new Dictionary<Language, string>
            {
                { Language.English,    "Subtraction" },
                { Language.Spanish,    "Resta" },
                { Language.French,     "Soustraction" },
                { Language.German,     "Subtraktion" },
                { Language.Portuguese, "Subtração" },
                { Language.Japanese,   "TODO: translate" },
                { Language.Chinese,    "TODO: translate" },
                { Language.Korean,     "TODO: translate" }
            });

            Add("mode_multiplication", new Dictionary<Language, string>
            {
                { Language.English,    "Multiplication" },
                { Language.Spanish,    "Multiplicación" },
                { Language.French,     "Multiplication" },
                { Language.German,     "Multiplikation" },
                { Language.Portuguese, "Multiplicação" },
                { Language.Japanese,   "TODO: translate" },
                { Language.Chinese,    "TODO: translate" },
                { Language.Korean,     "TODO: translate" }
            });

            Add("mode_division", new Dictionary<Language, string>
            {
                { Language.English,    "Division" },
                { Language.Spanish,    "División" },
                { Language.French,     "Division" },
                { Language.German,     "Division" },
                { Language.Portuguese, "Divisão" },
                { Language.Japanese,   "TODO: translate" },
                { Language.Chinese,    "TODO: translate" },
                { Language.Korean,     "TODO: translate" }
            });

            Add("mode_mixed", new Dictionary<Language, string>
            {
                { Language.English,    "Mixed" },
                { Language.Spanish,    "Mixto" },
                { Language.French,     "Mixte" },
                { Language.German,     "Gemischt" },
                { Language.Portuguese, "Misto" },
                { Language.Japanese,   "TODO: translate" },
                { Language.Chinese,    "TODO: translate" },
                { Language.Korean,     "TODO: translate" }
            });

            // --- UI Text ---
            Add("ui_score", new Dictionary<Language, string>
            {
                { Language.English, "Score" },
                { Language.Spanish, "Puntuación" },
                { Language.French,  "Score" },
                { Language.German,  "Punkte" },
                { Language.Portuguese, "Pontuação" },
                { Language.Japanese, "TODO: translate" },
                { Language.Chinese,  "TODO: translate" },
                { Language.Korean,   "TODO: translate" }
            });

            Add("ui_high_score", new Dictionary<Language, string>
            {
                { Language.English, "High Score" },
                { Language.Spanish, "Mejor Puntuación" },
                { Language.French,  "Meilleur Score" },
                { Language.German,  "Highscore" },
                { Language.Portuguese, "Melhor Pontuação" },
                { Language.Japanese, "TODO: translate" },
                { Language.Chinese,  "TODO: translate" },
                { Language.Korean,   "TODO: translate" }
            });

            Add("ui_game_over", new Dictionary<Language, string>
            {
                { Language.English, "Game Over" },
                { Language.Spanish, "Fin del Juego" },
                { Language.French,  "Fin de Partie" },
                { Language.German,  "Spielende" },
                { Language.Portuguese, "Fim de Jogo" },
                { Language.Japanese, "TODO: translate" },
                { Language.Chinese,  "TODO: translate" },
                { Language.Korean,   "TODO: translate" }
            });

            Add("ui_new_high_score", new Dictionary<Language, string>
            {
                { Language.English, "New High Score!" },
                { Language.Spanish, "¡Nuevo Récord!" },
                { Language.French,  "Nouveau Record !" },
                { Language.German,  "Neuer Highscore!" },
                { Language.Portuguese, "Novo Recorde!" },
                { Language.Japanese, "TODO: translate" },
                { Language.Chinese,  "TODO: translate" },
                { Language.Korean,   "TODO: translate" }
            });

            Add("ui_pause", new Dictionary<Language, string>
            {
                { Language.English, "Paused" },
                { Language.Spanish, "Pausado" },
                { Language.French,  "Pause" },
                { Language.German,  "Pausiert" },
                { Language.Portuguese, "Pausado" },
                { Language.Japanese, "TODO: translate" },
                { Language.Chinese,  "TODO: translate" },
                { Language.Korean,   "TODO: translate" }
            });

            Add("ui_resume", new Dictionary<Language, string>
            {
                { Language.English, "Resume" },
                { Language.Spanish, "Continuar" },
                { Language.French,  "Reprendre" },
                { Language.German,  "Fortsetzen" },
                { Language.Portuguese, "Continuar" },
                { Language.Japanese, "TODO: translate" },
                { Language.Chinese,  "TODO: translate" },
                { Language.Korean,   "TODO: translate" }
            });

            Add("ui_retry", new Dictionary<Language, string>
            {
                { Language.English, "Retry" },
                { Language.Spanish, "Reintentar" },
                { Language.French,  "Réessayer" },
                { Language.German,  "Erneut versuchen" },
                { Language.Portuguese, "Tentar novamente" },
                { Language.Japanese, "TODO: translate" },
                { Language.Chinese,  "TODO: translate" },
                { Language.Korean,   "TODO: translate" }
            });

            Add("ui_main_menu", new Dictionary<Language, string>
            {
                { Language.English, "Main Menu" },
                { Language.Spanish, "Menú Principal" },
                { Language.French,  "Menu Principal" },
                { Language.German,  "Hauptmenü" },
                { Language.Portuguese, "Menu Principal" },
                { Language.Japanese, "TODO: translate" },
                { Language.Chinese,  "TODO: translate" },
                { Language.Korean,   "TODO: translate" }
            });

            Add("ui_streak", new Dictionary<Language, string>
            {
                { Language.English, "Streak" },
                { Language.Spanish, "Racha" },
                { Language.French,  "Série" },
                { Language.German,  "Serie" },
                { Language.Portuguese, "Sequência" },
                { Language.Japanese, "TODO: translate" },
                { Language.Chinese,  "TODO: translate" },
                { Language.Korean,   "TODO: translate" }
            });

            Add("ui_accuracy", new Dictionary<Language, string>
            {
                { Language.English, "Accuracy" },
                { Language.Spanish, "Precisión" },
                { Language.French,  "Précision" },
                { Language.German,  "Genauigkeit" },
                { Language.Portuguese, "Precisão" },
                { Language.Japanese, "TODO: translate" },
                { Language.Chinese,  "TODO: translate" },
                { Language.Korean,   "TODO: translate" }
            });

            // --- Achievement Names ---
            Add("achievement_first_steps", new Dictionary<Language, string>
            {
                { Language.English, "First Steps" },
                { Language.Spanish, "Primeros Pasos" },
                { Language.French,  "Premiers Pas" },
                { Language.German,  "Erste Schritte" },
                { Language.Portuguese, "Primeiros Passos" },
                { Language.Japanese, "TODO: translate" },
                { Language.Chinese,  "TODO: translate" },
                { Language.Korean,   "TODO: translate" }
            });

            Add("achievement_century", new Dictionary<Language, string>
            {
                { Language.English, "Century" },
                { Language.Spanish, "Centurión" },
                { Language.French,  "Centenaire" },
                { Language.German,  "Jahrhundert" },
                { Language.Portuguese, "Centurião" },
                { Language.Japanese, "TODO: translate" },
                { Language.Chinese,  "TODO: translate" },
                { Language.Korean,   "TODO: translate" }
            });

            // --- Common Phrases ---
            Add("common_correct", new Dictionary<Language, string>
            {
                { Language.English, "Correct!" },
                { Language.Spanish, "¡Correcto!" },
                { Language.French,  "Correct !" },
                { Language.German,  "Richtig!" },
                { Language.Portuguese, "Correto!" },
                { Language.Japanese, "TODO: translate" },
                { Language.Chinese,  "TODO: translate" },
                { Language.Korean,   "TODO: translate" }
            });

            Add("common_wrong", new Dictionary<Language, string>
            {
                { Language.English, "Wrong!" },
                { Language.Spanish, "¡Incorrecto!" },
                { Language.French,  "Faux !" },
                { Language.German,  "Falsch!" },
                { Language.Portuguese, "Errado!" },
                { Language.Japanese, "TODO: translate" },
                { Language.Chinese,  "TODO: translate" },
                { Language.Korean,   "TODO: translate" }
            });

            Add("common_ready", new Dictionary<Language, string>
            {
                { Language.English, "Ready?" },
                { Language.Spanish, "¿Listo?" },
                { Language.French,  "Prêt ?" },
                { Language.German,  "Bereit?" },
                { Language.Portuguese, "Pronto?" },
                { Language.Japanese, "TODO: translate" },
                { Language.Chinese,  "TODO: translate" },
                { Language.Korean,   "TODO: translate" }
            });

            Add("common_go", new Dictionary<Language, string>
            {
                { Language.English, "GO!" },
                { Language.Spanish, "¡YA!" },
                { Language.French,  "GO !" },
                { Language.German,  "LOS!" },
                { Language.Portuguese, "VAI!" },
                { Language.Japanese, "TODO: translate" },
                { Language.Chinese,  "TODO: translate" },
                { Language.Korean,   "TODO: translate" }
            });

            Add("ui_daily_challenge", new Dictionary<Language, string>
            {
                { Language.English, "Daily Challenge" },
                { Language.Spanish, "Desafío Diario" },
                { Language.French,  "Défi Quotidien" },
                { Language.German,  "Tägliche Herausforderung" },
                { Language.Portuguese, "Desafio Diário" },
                { Language.Japanese, "TODO: translate" },
                { Language.Chinese,  "TODO: translate" },
                { Language.Korean,   "TODO: translate" }
            });
        }

        private void Add(string key, Dictionary<Language, string> translations)
        {
            stringTable[key] = translations;
        }
    }
}
