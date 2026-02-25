using System;

namespace MathRunner.Core
{
    /// <summary>Selectable player character.</summary>
    public enum CharacterType
    {
        Boy,
        Girl
    }

    /// <summary>Math operation mode.</summary>
    public enum MathMode
    {
        Addition,
        Subtraction,
        Multiplication,
        Division,
        Mixed
    }

    /// <summary>Game difficulty tier.</summary>
    public enum DifficultyLevel
    {
        Easy,
        Medium,
        Hard
    }

    /// <summary>High-level game phase for state management.</summary>
    public enum GamePhase
    {
        Menu,
        CharacterSelect,
        ModeSelect,
        Countdown,
        Playing,
        Paused,
        GameOver,
        Tutorial
    }

    /// <summary>Extension helpers for <see cref="CharacterType"/>.</summary>
    public static class CharacterTypeExtensions
    {
        /// <summary>
        /// Converts the enum value to the lowercase string used by PlayerPrefs
        /// (e.g. <c>"boy"</c>, <c>"girl"</c>).
        /// </summary>
        public static string ToPlayerPrefsString(this CharacterType type)
        {
            switch (type)
            {
                case CharacterType.Boy:  return "boy";
                case CharacterType.Girl: return "girl";
                default: throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        /// <summary>
        /// Parses a PlayerPrefs string back to a <see cref="CharacterType"/>.
        /// Defaults to <see cref="CharacterType.Boy"/> for unrecognised values.
        /// </summary>
        public static CharacterType FromPlayerPrefsString(string value)
        {
            if (string.IsNullOrEmpty(value))
                return CharacterType.Boy;

            switch (value.ToLowerInvariant())
            {
                case "girl": return CharacterType.Girl;
                default:     return CharacterType.Boy;
            }
        }
    }

    /// <summary>Extension helpers for <see cref="MathMode"/>.</summary>
    public static class MathModeExtensions
    {
        /// <summary>
        /// Returns a human-readable display name for the mode
        /// (e.g. <c>"Multiplication"</c>).
        /// </summary>
        public static string DisplayName(this MathMode mode)
        {
            switch (mode)
            {
                case MathMode.Addition:       return "Addition";
                case MathMode.Subtraction:    return "Subtraction";
                case MathMode.Multiplication: return "Multiplication";
                case MathMode.Division:       return "Division";
                case MathMode.Mixed:          return "Mixed";
                default: throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }
        }

        /// <summary>
        /// Returns the mathematical symbol for the mode
        /// (e.g. <c>"+"</c>, <c>"÷"</c>). Mixed returns <c>"?"</c>.
        /// </summary>
        public static string Symbol(this MathMode mode)
        {
            switch (mode)
            {
                case MathMode.Addition:       return "+";
                case MathMode.Subtraction:    return "-";
                case MathMode.Multiplication: return "x";
                case MathMode.Division:       return "÷";
                case MathMode.Mixed:          return "?";
                default: throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }
        }

        /// <summary>
        /// Converts the enum value to the lowercase string stored in PlayerPrefs
        /// (matches the existing keys: <c>"addition"</c>, <c>"subtraction"</c>,
        /// <c>"multiply"</c>, <c>"division"</c>, <c>"mixed"</c>).
        /// </summary>
        public static string ToPlayerPrefsString(this MathMode mode)
        {
            switch (mode)
            {
                case MathMode.Addition:       return "addition";
                case MathMode.Subtraction:    return "subtraction";
                case MathMode.Multiplication: return "multiply";
                case MathMode.Division:       return "division";
                case MathMode.Mixed:          return "mixed";
                default: throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }
        }

        /// <summary>
        /// Parses a PlayerPrefs string back to a <see cref="MathMode"/>.
        /// Defaults to <see cref="MathMode.Addition"/> for unrecognised values.
        /// </summary>
        public static MathMode FromPlayerPrefsString(string value)
        {
            if (string.IsNullOrEmpty(value))
                return MathMode.Addition;

            switch (value.ToLowerInvariant())
            {
                case "subtraction":    return MathMode.Subtraction;
                case "multiply":       return MathMode.Multiplication;
                case "multiplication": return MathMode.Multiplication;
                case "division":       return MathMode.Division;
                case "mixed":          return MathMode.Mixed;
                default:               return MathMode.Addition;
            }
        }
    }
}
