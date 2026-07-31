using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace MathRunner.Core
{
    /// <summary>
    /// Reusable countdown (3-2-1-GO!) supporting both legacy uGUI Text and
    /// UI Toolkit Label (via callback).
    /// </summary>
    public class CountdownHelper : MonoBehaviour
    {
        [SerializeField] private Text countdownText;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private int duration = GameConstants.COUNTDOWN_SECONDS;
        [SerializeField] private float bounceScale = 1.4f;
        [SerializeField] private float bounceUpTime = 0.15f;
        [SerializeField] private float bounceDownTime = 0.25f;

        public event Action OnCountdownComplete;

        private Coroutine activeCountdown;

        public void StartCountdown()
        {
            StartCountdown(countdownText, audioSource, duration, null);
        }

        public void StartCountdown(Action onComplete)
        {
            StartCountdown(countdownText, audioSource, duration, onComplete);
        }

        public void StartCountdown(Text text, AudioSource audio, int seconds, Action onComplete)
        {
            if (text == null)
            {
                Debug.LogWarning("[CountdownHelper] No Text component assigned. Aborting countdown.");
                return;
            }

            if (activeCountdown != null)
                StopCoroutine(activeCountdown);

            activeCountdown = StartCoroutine(CountdownRoutine(
                value => { text.gameObject.SetActive(true); text.text = value; },
                () => text.gameObject.SetActive(false),
                text.transform,
                audio,
                seconds,
                onComplete));
        }

        /// <summary>Starts a countdown that updates a Toolkit Label via callback.</summary>
        public void StartCountdown(Action<string> setText, Action onComplete, int seconds = -1, AudioSource audio = null)
        {
            if (setText == null) return;
            if (seconds < 0) seconds = duration;
            if (activeCountdown != null) StopCoroutine(activeCountdown);
            activeCountdown = StartCoroutine(CountdownRoutine(
                setText,
                () => setText(""),
                null,
                audio ?? audioSource,
                seconds,
                onComplete));
        }

        public void CancelCountdown()
        {
            if (activeCountdown != null)
            {
                StopCoroutine(activeCountdown);
                activeCountdown = null;
            }
        }

        private IEnumerator CountdownRoutine(
            Action<string> setText,
            Action hide,
            Transform bounceTarget,
            AudioSource audio,
            int seconds,
            Action onComplete)
        {
            Vector3 originalScale = bounceTarget != null ? bounceTarget.localScale : Vector3.one;
            int count = seconds;

            while (count > 0)
            {
                setText?.Invoke(count == 1 ? "GO!" : (count - 1).ToString());

                if (audio != null)
                    audio.Play();

                if (bounceTarget != null)
                    yield return StartCoroutine(BounceRoutine(bounceTarget, originalScale));
                else
                    yield return new WaitForSeconds(bounceUpTime + bounceDownTime);

                yield return new WaitForSeconds(Mathf.Max(0f, 1f - bounceUpTime - bounceDownTime));
                count--;
            }

            if (bounceTarget != null)
                bounceTarget.localScale = originalScale;
            hide?.Invoke();
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
    }
}
