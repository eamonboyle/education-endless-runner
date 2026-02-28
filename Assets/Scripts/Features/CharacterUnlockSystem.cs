using System;
using System.Collections.Generic;
using UnityEngine;
using MathRunner.Core;
using MathRunner.Data;

/// <summary>
/// Static registry for unlockable player characters.
/// Each character has an unlock condition evaluated at runtime;
/// unlock state is persisted in PlayerPrefs.
/// </summary>
public static class CharacterUnlockSystem
{
    private const string UnlockPrefix = "characterUnlock_";

    private static List<UnlockableCharacter> allCharacters;

    /// <summary>
    /// Represents a single unlockable character with its requirements.
    /// </summary>
    [Serializable]
    public class UnlockableCharacter
    {
        /// <summary>Unique identifier used for PlayerPrefs persistence.</summary>
        public string Id { get; private set; }

        /// <summary>Display name shown in the character selection screen.</summary>
        public string Name { get; private set; }

        /// <summary>Short text explaining how to unlock this character.</summary>
        public string Description { get; private set; }

        /// <summary>Predicate that returns true when the unlock criteria are met.</summary>
        public Func<bool> UnlockCondition { get; private set; }

        /// <summary>Whether this character has been permanently unlocked.</summary>
        public bool IsUnlocked
        {
            get { return PlayerPrefs.GetInt(UnlockPrefix + Id, 0) == 1; }
        }

        /// <summary>
        /// Creates a new unlockable character definition.
        /// </summary>
        /// <param name="id">Unique string identifier.</param>
        /// <param name="name">Display name.</param>
        /// <param name="description">Unlock requirement description.</param>
        /// <param name="unlockCondition">Predicate returning true when criteria are met.</param>
        public UnlockableCharacter(string id, string name, string description, Func<bool> unlockCondition)
        {
            Id = id;
            Name = name;
            Description = description;
            UnlockCondition = unlockCondition;
        }

        /// <summary>
        /// Persists the unlocked state to PlayerPrefs.
        /// </summary>
        public void Unlock()
        {
            PlayerPrefs.SetInt(UnlockPrefix + Id, 1);
            PlayerPrefs.Save();
        }
    }

    /// <summary>
    /// Returns every character definition. The list is lazily initialised on first access.
    /// </summary>
    /// <returns>All defined characters.</returns>
    public static List<UnlockableCharacter> GetAllCharacters()
    {
        if (allCharacters == null)
        {
            InitCharacters();
        }
        return allCharacters;
    }

    /// <summary>
    /// Returns only the characters the player has unlocked.
    /// </summary>
    /// <returns>List of unlocked characters.</returns>
    public static List<UnlockableCharacter> GetUnlockedCharacters()
    {
        List<UnlockableCharacter> unlocked = new List<UnlockableCharacter>();
        foreach (UnlockableCharacter c in GetAllCharacters())
        {
            if (c.IsUnlocked)
            {
                unlocked.Add(c);
            }
        }
        return unlocked;
    }

    /// <summary>
    /// Evaluates every character's unlock condition. Newly-qualifying
    /// characters are unlocked and persisted.
    /// </summary>
    /// <returns>Characters unlocked during this call.</returns>
    public static List<UnlockableCharacter> CheckUnlocks()
    {
        List<UnlockableCharacter> newlyUnlocked = new List<UnlockableCharacter>();
        foreach (UnlockableCharacter c in GetAllCharacters())
        {
            if (!c.IsUnlocked && c.UnlockCondition != null && c.UnlockCondition())
            {
                c.Unlock();
                newlyUnlocked.Add(c);
            }
        }
        return newlyUnlocked;
    }

    /// <summary>
    /// Checks whether a specific character is unlocked by its identifier.
    /// </summary>
    /// <param name="id">Character identifier.</param>
    /// <returns>True if the character is unlocked.</returns>
    public static bool IsCharacterUnlocked(string id)
    {
        if (string.IsNullOrEmpty(id)) return false;
        return PlayerPrefs.GetInt(UnlockPrefix + id, 0) == 1;
    }

    private static void InitCharacters()
    {
        allCharacters = new List<UnlockableCharacter>
        {
            new UnlockableCharacter(
                "boy",
                "Boy",
                "Default character — unlocked from the start.",
                () => true
            ),

            new UnlockableCharacter(
                "girl",
                "Girl",
                "Default character — unlocked from the start.",
                () => true
            ),

            new UnlockableCharacter(
                "robot",
                "Robot",
                "Score 500 total points across all games.",
                () => GetTotalScore() >= 500
            ),

            new UnlockableCharacter(
                "ninja",
                "Ninja",
                "Achieve a streak of 10 correct answers.",
                () => PlayerStats.GetBestStreak("total") >= 10
            ),

            new UnlockableCharacter(
                "astronaut",
                "Astronaut",
                "Complete 5 campaign levels.",
                () => PlayerPrefs.GetInt(GameConstants.PREF_CAMPAIGN_PROGRESS, 0) >= 5
            ),

            new UnlockableCharacter(
                "wizard",
                "Wizard",
                "Unlock every achievement.",
                () => AreAllAchievementsUnlocked()
            ),

            new UnlockableCharacter(
                "pirate",
                "Pirate",
                "Play 100 games.",
                () => PlayerStats.GetTotalGamesPlayed() >= 100
            ),

            new UnlockableCharacter(
                "alien",
                "Alien",
                "Score 1000 in a single game.",
                () => GameState.GetScore() >= 1000
            )
        };
    }

    private static int GetTotalScore()
    {
        int total = 0;
        string[] modes = { "addition", "subtraction", "multiply", "division" };
        foreach (string mode in modes)
        {
            total += GameState.GetHighScore(mode);
        }
        return total;
    }

    private static bool AreAllAchievementsUnlocked()
    {
        List<Achievement> all = AchievementData.All;
        if (all == null || all.Count == 0) return false;

        foreach (Achievement a in all)
        {
            if (!a.IsUnlocked) return false;
        }
        return true;
    }
}
