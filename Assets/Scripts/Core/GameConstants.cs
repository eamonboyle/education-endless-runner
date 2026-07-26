using UnityEngine;

namespace MathRunner.Core
{
    /// <summary>
    /// Central repository of game-wide constants. Eliminates magic numbers
    /// scattered across the codebase and provides a single source of truth
    /// for tuning values and PlayerPrefs keys.
    /// </summary>
    public static class GameConstants
    {
        #region Lane Positions

        /// <summary>X position of the left lane.</summary>
        public const float LEFT_LANE = -1.586f;

        /// <summary>X position of the center lane.</summary>
        public const float CENTER_LANE = 0f;

        /// <summary>X position of the right lane.</summary>
        public const float RIGHT_LANE = 1.586f;

        #endregion

        #region Gameplay

        /// <summary>Y offset at which question boxes are spawned.</summary>
        public const float BOX_HEIGHT = 1.3f;

        /// <summary>Z distance between consecutive question rows.</summary>
        public const float QUESTION_SPACING = 50.0f;

        /// <summary>Initial forward speed of the player character (units/sec).</summary>
        /// <remarks>
        /// Retuned from 40 after fixing PlayerMovement's self-referencing lerp,
        /// which previously applied only ~10% of the configured speed.
        /// </remarks>
        public const float DEFAULT_SPEED = 4.0f;

        /// <summary>Minimum swipe magnitude (in pixels) to register as a directional swipe.</summary>
        public const float SWIPE_DEADZONE = 50f;

        /// <summary>Number of seconds the pre-game countdown lasts (including the "GO!" beat).</summary>
        public const int COUNTDOWN_SECONDS = 4;

        #endregion

        #region Level Generation

        /// <summary>Length of a single floor tile along the Z axis.</summary>
        public const float FLOOR_WIDTH = 36.0f;

        /// <summary>Z offset applied when placing the next floor tile.</summary>
        public const float FLOOR_OFFSET = 18.44f;

        #endregion

        #region Player

        /// <summary>Z position where the player character is instantiated.</summary>
        public const float PLAYER_SPAWN_Z = 6.3f;

        /// <summary>Z offset behind the player where dust particles are spawned.</summary>
        public const float DUST_OFFSET_Z = -0.5f;

        #endregion

        #region PlayerPrefs Keys

        /// <summary>Key for the selected character name (string: "boy" / "girl").</summary>
        public const string PREF_CHARACTER = "character";

        /// <summary>Key for the selected math mode (string: "addition", "subtraction", etc.).</summary>
        public const string PREF_MODE = "mode";

        /// <summary>Key for the sound-enabled flag (int: 0 or 1).</summary>
        public const string PREF_SOUND = "sound";

        /// <summary>Key for the graphics quality setting (string).</summary>
        public const string PREF_GRAPHICS = "graphics";

        /// <summary>
        /// Prefix for per-mode high-score keys.
        /// Append the mode name to form the full key, e.g. "highScore_addition".
        /// </summary>
        public const string PREF_HIGH_SCORE_PREFIX = "highScore_";

        /// <summary>Key indicating whether the player has completed the first-load tutorial (int: 0 or 1).</summary>
        public const string PREF_FIRST_LOAD = "firstLoad";

        /// <summary>Key for total number of games played (int).</summary>
        public const string PREF_GAMES_PLAYED = "gamesPlayed";

        /// <summary>Key for the remaining continue-ad count in the current session (int).</summary>
        public const string PREF_AD_COUNT = "adCount";

        /// <summary>Key for the last saved score (int).</summary>
        public const string PREF_SCORE = "score";

        /// <summary>Prefix for per-mode games-played keys. Append mode name.</summary>
        public const string PREF_GAMES_PLAYED_PREFIX = "gamesPlayed_";

        /// <summary>Prefix for total questions answered per mode. Append mode name.</summary>
        public const string PREF_TOTAL_QUESTIONS_PREFIX = "totalQuestions_";

        /// <summary>Prefix for correct answers per mode. Append mode name.</summary>
        public const string PREF_CORRECT_ANSWERS_PREFIX = "correctAnswers_";

        /// <summary>Prefix for wrong answers per mode. Append mode name.</summary>
        public const string PREF_WRONG_ANSWERS_PREFIX = "wrongAnswers_";

        /// <summary>Prefix for best streak per mode. Append mode name.</summary>
        public const string PREF_BEST_STREAK_PREFIX = "bestStreak_";

        /// <summary>Key for total time played in seconds (float).</summary>
        public const string PREF_TOTAL_TIME_PLAYED = "totalTimePlayed";

        /// <summary>Key for the current streak within a session (int).</summary>
        public const string PREF_CURRENT_STREAK = "currentStreak";

        /// <summary>Prefix for achievement unlock state. Append achievement id.</summary>
        public const string PREF_ACHIEVEMENT_PREFIX = "achievement_";

        /// <summary>Prefix for daily challenge data. Append date string.</summary>
        public const string PREF_DAILY_CHALLENGE_PREFIX = "dailyChallenge_";

        /// <summary>Key for the daily challenge reward claimed flag. Append date string.</summary>
        public const string PREF_DAILY_REWARD_PREFIX = "dailyReward_";

        /// <summary>Key for the selected difficulty level (int: 0=Easy, 1=Medium, 2=Hard).</summary>
        public const string PREF_DIFFICULTY = "difficulty";

        /// <summary>Key for the time-attack mode flag (int: 0 or 1).</summary>
        public const string PREF_TIME_ATTACK = "timeAttack";

        /// <summary>Prefix for campaign level star ratings. Append level number.</summary>
        public const string PREF_CAMPAIGN_STARS_PREFIX = "campaignStars_";

        /// <summary>Key for the highest unlocked campaign level (int).</summary>
        public const string PREF_CAMPAIGN_PROGRESS = "campaignProgress";

        #endregion
    }
}
