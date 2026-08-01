using System;
using System.Text;
using MathRunner.Core;
using UnityEngine;

/// <summary>
/// Generates and decodes short alphanumeric challenge codes that encode a
/// game mode, difficulty, and random seed. When a code is active the seed
/// is used via <see cref="UnityEngine.Random.InitState"/> to produce an
/// identical question sequence for all players sharing the same code.
/// </summary>
public static class ChallengeCodeSystem
{
    private const int CodeLength = 6;
    private const string Base36Chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const string ActiveChallengeKey = "Challenge_Active";

    /// <summary>Decoded challenge parameters.</summary>
    public struct ChallengeParams
    {
        /// <summary>Game mode index (0–4).</summary>
        public int ModeIndex;

        /// <summary>Difficulty level (0–2).</summary>
        public int Difficulty;

        /// <summary>Random seed that produces the question sequence.</summary>
        public int Seed;

        /// <summary>Mode key string derived from <see cref="ModeIndex"/>.</summary>
        public string ModeKey
        {
            get
            {
                switch (ModeIndex)
                {
                    case 0: return "addition";
                    case 1: return "subtraction";
                    case 2: return "multiply";
                    case 3: return "division";
                    case 4: return "mixed";
                    default: return "addition";
                }
            }
        }
    }

    /// <summary>
    /// Generates a 6-character challenge code from the specified parameters.
    /// </summary>
    /// <param name="mode">
    /// Mode string (e.g. "addition"). Mapped to an index internally.
    /// </param>
    /// <param name="difficulty">Difficulty level (0–2).</param>
    /// <returns>A 6-character base-36 code.</returns>
    public static string GenerateCode(string mode, int difficulty)
    {
        int modeIndex = ModeStringToIndex(mode);
        int seed = UnityEngine.Random.Range(0, 1000000);

        long packed = ((long)modeIndex * 3 + difficulty) * 1000000L + seed;
        return ToBase36(packed);
    }

    /// <summary>
    /// Decodes a challenge code back into its constituent parameters.
    /// </summary>
    /// <param name="code">A 6-character code produced by <see cref="GenerateCode"/>.</param>
    /// <returns>The decoded <see cref="ChallengeParams"/>.</returns>
    public static ChallengeParams DecodeCode(string code)
    {
        if (string.IsNullOrEmpty(code))
        {
            Debug.LogWarning("ChallengeCodeSystem: Null or empty code.");
            return default;
        }

        long packed = FromBase36(code.ToUpperInvariant());

        int seed = (int)(packed % 1000000L);
        long rest = packed / 1000000L;
        int difficulty = (int)(rest % 3);
        int modeIndex = (int)(rest / 3);

        return new ChallengeParams
        {
            ModeIndex = Mathf.Clamp(modeIndex, 0, 4),
            Difficulty = Mathf.Clamp(difficulty, 0, 2),
            Seed = seed
        };
    }

    /// <summary>
    /// Activates a challenge by persisting its code. The code will be used
    /// to seed the random number generator for question generation.
    /// </summary>
    /// <param name="code">Challenge code to activate.</param>
    public static void SetActiveChallenge(string code)
    {
        PlayerPrefs.SetString(ActiveChallengeKey, code ?? "");
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Returns the currently active challenge code, or an empty string
    /// if none is active.
    /// </summary>
    public static string GetActiveChallenge()
    {
        return PlayerPrefs.GetString(ActiveChallengeKey, "");
    }

    /// <summary>
    /// Returns <c>true</c> if a challenge code is currently active.
    /// </summary>
    public static bool IsPlayingChallenge()
    {
        return !string.IsNullOrEmpty(GetActiveChallenge());
    }

    public static bool TryGetActiveChallenge(out ChallengeParams challenge)
    {
        string code = GetActiveChallenge();
        if (string.IsNullOrEmpty(code))
        {
            challenge = default;
            return false;
        }

        challenge = DecodeCode(code);
        return true;
    }

    public static bool ApplyActiveChallengeSettings()
    {
        if (!TryGetActiveChallenge(out ChallengeParams challenge)) return false;

        TimeAttackMode.SetTimeAttack(false);
        PlayerPrefs.SetInt(GameConstants.PREF_CAMPAIGN_ACTIVE, 0);
        GameState.SetQuestionType(challenge.ModeKey);
        DifficultyPresets.SetDifficulty((DifficultyLevel)challenge.Difficulty);
        return true;
    }

    public static bool ApplyActiveChallengeSeed()
    {
        if (!TryGetActiveChallenge(out ChallengeParams challenge)) return false;

        UnityEngine.Random.InitState(challenge.Seed);
        return true;
    }

    #region Base-36 Encoding

    private static string ToBase36(long value)
    {
        if (value < 0) value = -value;

        StringBuilder sb = new StringBuilder(CodeLength);
        for (int i = 0; i < CodeLength; i++)
        {
            int remainder = (int)(value % 36);
            sb.Insert(0, Base36Chars[remainder]);
            value /= 36;
        }

        return sb.ToString();
    }

    private static long FromBase36(string str)
    {
        long result = 0;
        foreach (char c in str)
        {
            int val = Base36Chars.IndexOf(c);
            if (val < 0) val = 0;
            result = result * 36 + val;
        }
        return result;
    }

    #endregion

    private static int ModeStringToIndex(string mode)
    {
        if (string.IsNullOrEmpty(mode)) return 0;

        switch (mode.ToLowerInvariant())
        {
            case "addition":       return 0;
            case "subtraction":    return 1;
            case "multiply":
            case "multiplication": return 2;
            case "division":       return 3;
            case "mixed":          return 4;
            default:               return 0;
        }
    }
}
