using System;
using UnityEngine;
using MathRunner.Core;

namespace MathRunner.Data
{
    /// <summary>
    /// Static player profile that aggregates identity, progression, and
    /// play-history data. All values are persisted in PlayerPrefs.
    /// </summary>
    public static class PlayerProfile
    {
        private const string UsernameKey = "profile_username";
        private const string AvatarKey = "profile_avatar";
        private const string JoinDateKey = "profile_joinDate";
        private const string TotalPlayTimeKey = "profile_totalPlayTime";

        /// <summary>
        /// Returns the player's display name. Defaults to <c>"Player"</c>
        /// if none has been set.
        /// </summary>
        /// <returns>Display name.</returns>
        public static string GetUsername()
        {
            string name = PlayerPrefs.GetString(UsernameKey, "");
            return string.IsNullOrEmpty(name) ? "Player" : name;
        }

        /// <summary>
        /// Persists a new display name. Empty or null values are ignored.
        /// </summary>
        /// <param name="username">New display name.</param>
        public static void SetUsername(string username)
        {
            if (string.IsNullOrEmpty(username)) return;
            PlayerPrefs.SetString(UsernameKey, username);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Returns the identifier of the currently selected character/avatar.
        /// Falls back to the value stored in <see cref="GameState"/>.
        /// </summary>
        /// <returns>Character identifier string.</returns>
        public static string GetAvatar()
        {
            string avatar = PlayerPrefs.GetString(AvatarKey, "");
            if (string.IsNullOrEmpty(avatar))
            {
                avatar = GameState.GetCharacter();
            }
            return string.IsNullOrEmpty(avatar) ? "boy" : avatar;
        }

        /// <summary>
        /// Persists the selected character/avatar identifier.
        /// </summary>
        /// <param name="avatarId">Character identifier.</param>
        public static void SetAvatar(string avatarId)
        {
            if (string.IsNullOrEmpty(avatarId)) return;
            PlayerPrefs.SetString(AvatarKey, avatarId);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Returns the player's total accumulated XP via <see cref="XPSystem"/>.
        /// </summary>
        /// <returns>Total XP.</returns>
        public static int GetTotalXP()
        {
            return XPSystem.GetCurrentXP();
        }

        /// <summary>
        /// Returns the player's current level via <see cref="XPSystem"/>.
        /// </summary>
        /// <returns>Current level.</returns>
        public static int GetCurrentLevel()
        {
            return XPSystem.GetCurrentLevel();
        }

        /// <summary>
        /// Determines the player's most-played math mode by comparing
        /// per-mode game counts stored in PlayerPrefs.
        /// </summary>
        /// <returns>Mode string of the most-played mode, or <c>"addition"</c> as default.</returns>
        public static string GetFavoriteMode()
        {
            string[] modes = { "addition", "subtraction", "multiply", "division" };
            string favorite = modes[0];
            int maxPlayed = 0;

            foreach (string mode in modes)
            {
                int played = PlayerStats.GetGamesPlayed(mode);
                if (played > maxPlayed)
                {
                    maxPlayed = played;
                    favorite = mode;
                }
            }

            return favorite;
        }

        /// <summary>
        /// Returns the date the player first opened the game. The join date
        /// is recorded on first access and never changes.
        /// </summary>
        /// <returns>Join date as a <see cref="DateTime"/> (UTC).</returns>
        public static DateTime GetJoinDate()
        {
            string stored = PlayerPrefs.GetString(JoinDateKey, "");
            if (string.IsNullOrEmpty(stored))
            {
                string now = DateTime.UtcNow.ToString("o");
                PlayerPrefs.SetString(JoinDateKey, now);
                PlayerPrefs.Save();
                return DateTime.UtcNow;
            }

            DateTime result;
            if (DateTime.TryParse(stored, null, System.Globalization.DateTimeStyles.RoundtripKind, out result))
            {
                return result;
            }

            return DateTime.UtcNow;
        }

        /// <summary>
        /// Returns the total play time across all sessions, in seconds.
        /// Delegates to <see cref="PlayerStats.GetTotalTimePlayed"/>.
        /// </summary>
        /// <returns>Total seconds played.</returns>
        public static float GetTotalPlayTime()
        {
            return PlayerStats.GetTotalTimePlayed();
        }
    }
}
