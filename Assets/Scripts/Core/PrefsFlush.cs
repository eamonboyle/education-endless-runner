using UnityEngine;

namespace MathRunner.Core
{
    /// <summary>
    /// Batches PlayerPrefs.Save calls so hot gameplay paths can write keys
    /// without flushing to disk every question. Call <see cref="Flush"/> on
    /// game over and when the app backgrounds.
    /// </summary>
    public static class PrefsFlush
    {
        private static bool dirty;

        /// <summary>Marks PlayerPrefs as needing a disk flush.</summary>
        public static void MarkDirty()
        {
            dirty = true;
        }

        /// <summary>Writes PlayerPrefs to disk if any writes are pending.</summary>
        public static void FlushIfDirty()
        {
            if (!dirty) return;
            Flush();
        }

        /// <summary>Forces a PlayerPrefs disk flush and clears the dirty flag.</summary>
        public static void Flush()
        {
            AnalyticsManager.FlushPendingEvents();
            PlayerPrefs.Save();
            dirty = false;
        }
    }
}
