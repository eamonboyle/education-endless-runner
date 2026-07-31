using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace MathRunner.Core
{
    /// <summary>
    /// Reusable countdown component (3-2-1-GO!) with a scale-bounce animation.
    /// Replaces the duplicated countdown coroutines in StartCountdown, Pause,
    /// EndScreen, and Pause.
    /// </summary>
    public class CountdownHelper : MonoBehaviour
    {
        #region Inspector Fields

        [Tooltip("UI Text component that displays the countdown numbers.")]
        [SerializeField] private Text countdownText;

        [Tooltip("Optional AudioSource that plays a tick sound each beat.")]
        [SerializeField] private AudioSource audioSource;

        [Tooltip("Total duration in beats (e.g. 4 means 3-2-1-GO!).")]
        [SerializeField] private int duration = GameConstants.COUNTDOWN_SECONDS;

        [Tooltip("Peak scale multiplier applied during the bounce animation.")]
        [SerializeField] private float bounceScale = 1.4f;

        [Tooltip("Time in seconds for the scale-up portion of the bounce.")]
        [SerializeField] private float bounceUpTime = 0.15f;

        [Tooltip("Time in seconds for the scale-down portion of the bounce.")]
        [SerializeField] private float bounceDownTime = 0.25f;

        #endregion

        /// <summary>
        /// Invoked when the countdown finishes (after the "GO!" beat completes).
        /// </summary>
        public event Action OnCountdownComplete;

        private Coroutine activeCountdown;

        #region Public API

        /// <summary>
        /// Starts the countdown using the inspector-configured text, audio, and duration.
        /// </summary>
        public void StartCountdown()
        {
            StartCountdown(countdownText, audioSource, duration, null);
        }

        /// <summary>
        /// Starts the countdown with an explicit completion callback.
        /// </summary>
        /// <param name="onComplete">Called when the countdown ends.</param>
        public void StartCountdown(Action onComplete)
        {
            StartCountdown(countdownText, audioSource, duration, onComplete);
        }

        /// <summary>
        /// Starts the countdown with fully overridden parameters.
        /// </summary>
        /// <param name="text">The UI Text component to display numbers on.</param>
        /// <param name="audio">Optional AudioSource for tick sounds (may be null).</param>
        /// <param name="seconds">Total beats (e.g. 4 for 3-2-1-GO!).</param>
        /// <param name="onComplete">Called when the countdown ends (may be null).</param>
        public void StartCountdown(Text text, AudioSource audio, int seconds, Action onComplete)
        {
            if (text == null)
            {
                Debug.LogWarning("[CountdownHelper] No Text component assigned. Aborting countdown.");
                return;
            }

            if (activeCountdown != null)
            {
                StopCoroutine(activeCountdown);
            }

            activeCountdown = StartCoroutine(CountdownRoutine(text, audio, seconds, onComplete));
        }

        /// <summary>
        /// Cancels any running countdown immediately.
        /// </summary>
        public void CancelCountdown()
        {
            if (activeCountdown != null)
            {
                StopCoroutine(activeCountdown);
                activeCountdown = null;
            }
        }

        #endregion

        #region Coroutines

        private IEnumerator CountdownRoutine(Text text, AudioSource audio, int seconds, Action onComplete)
        {
            text.gameObject.SetActive(true);
            Vector3 originalScale = text.transform.localScale;
            int count = seconds;

            while (count > 0)
            {
                text.text = (count == 1) ? "GO!" : (count - 1).ToString();

                if (audio != null)
                {
                    audio.Play();
                }

                yield return StartCoroutine(BounceRoutine(text.transform, originalScale));

                yield return new WaitForSeconds(
                    Mathf.Max(0f, 1f - bounceUpTime - bounceDownTime));

                count--;
            }

            text.transform.localScale = originalScale;
            activeCountdown = null;

            OnCountdownComplete?.Invoke();
            onComplete?.Invoke();
        }

        private IEnumerator BounceRoutine(Transform target, Vector3 originalScale)
        {
            Vector3 peakScale = originalScale * bounceScale;
            float elapsed = 0f;

            while (elapsed < bounceUpTime)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / bounceUpTime);
                target.localScale = Vector3.Lerp(originalScale, peakScale, t);
                yield return null;
            }

            target.localScale = peakScale;
            elapsed = 0f;

            while (elapsed < bounceDownTime)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / bounceDownTime);
                target.localScale = Vector3.Lerp(peakScale, originalScale, t);
                yield return null;
            }

            target.localScale = originalScale;
        }

        #endregion
    }
}
