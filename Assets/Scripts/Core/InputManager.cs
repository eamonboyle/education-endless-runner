using System;
using UnityEngine;

namespace MathRunner.Core
{
    /// <summary>
    /// Singleton that provides an abstraction layer over input methods.
    /// Supports swipe, tap (one-handed), and on-screen button modes.
    /// Replaces direct <c>PlayerController</c> queries in movement code.
    /// </summary>
    public class InputManager : MonoBehaviour
    {
        #region Singleton

        /// <summary>Global singleton instance.</summary>
        public static InputManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadInputMode();
        }

        #endregion

        /// <summary>Available input control schemes.</summary>
        public enum InputMode
        {
            /// <summary>Swipe left/right (default, mirrors PlayerController).</summary>
            Swipe,
            /// <summary>Tap left/right halves of screen (one-handed).</summary>
            Tap,
            /// <summary>On-screen directional buttons.</summary>
            Buttons
        }

        private const string InputModePrefsKey = "InputManager_Mode";

        /// <summary>Fired when a left movement input is detected.</summary>
        public event Action OnSwipeLeft;

        /// <summary>Fired when a right movement input is detected.</summary>
        public event Action OnSwipeRight;

        [SerializeField, Tooltip("Optional on-screen left button (Button mode).")]
        private UnityEngine.UI.Button leftButton;

        [SerializeField, Tooltip("Optional on-screen right button (Button mode).")]
        private UnityEngine.UI.Button rightButton;

        private InputMode currentMode = InputMode.Swipe;
        private bool swipeLeftThisFrame;
        private bool swipeRightThisFrame;

        // Swipe tracking
        private bool isDragging;
        private Vector2 startTouch;

        private void Start()
        {
            if (leftButton != null)
            {
                leftButton.onClick.AddListener(OnLeftButtonPressed);
            }
            if (rightButton != null)
            {
                rightButton.onClick.AddListener(OnRightButtonPressed);
            }
        }

        private void Update()
        {
            swipeLeftThisFrame = false;
            swipeRightThisFrame = false;

            switch (currentMode)
            {
                case InputMode.Swipe:
                    ProcessSwipeInput();
                    break;
                case InputMode.Tap:
                    ProcessTapInput();
                    break;
                case InputMode.Buttons:
                    break;
            }
        }

        /// <summary>Returns <c>true</c> during the frame a left swipe/input was detected.</summary>
        public bool GetSwipeLeft()
        {
            return swipeLeftThisFrame;
        }

        /// <summary>Returns <c>true</c> during the frame a right swipe/input was detected.</summary>
        public bool GetSwipeRight()
        {
            return swipeRightThisFrame;
        }

        /// <summary>Returns the currently active input mode.</summary>
        public InputMode GetInputMode()
        {
            return currentMode;
        }

        /// <summary>
        /// Sets and persists the input mode. Resets internal tracking state.
        /// </summary>
        /// <param name="mode">The desired input mode.</param>
        public void SetInputMode(InputMode mode)
        {
            currentMode = mode;
            isDragging = false;
            startTouch = Vector2.zero;
            PlayerPrefs.SetInt(InputModePrefsKey, (int)mode);
            PlayerPrefs.Save();
        }

        #region Swipe Mode

        private void ProcessSwipeInput()
        {
            // Standalone (mouse)
            if (Input.GetMouseButtonDown(0))
            {
                isDragging = true;
                startTouch = Input.mousePosition;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;
                startTouch = Vector2.zero;
            }

            // Mobile (touch)
            if (Input.touchCount > 0)
            {
                Touch t = Input.GetTouch(0);
                if (t.phase == TouchPhase.Began)
                {
                    isDragging = true;
                    startTouch = t.position;
                }
                else if (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled)
                {
                    isDragging = false;
                    startTouch = Vector2.zero;
                }
            }

            if (!isDragging) return;

            Vector2 delta = Vector2.zero;
            if (Input.touchCount > 0)
            {
                delta = Input.GetTouch(0).position - startTouch;
            }
            else if (Input.GetMouseButton(0))
            {
                delta = (Vector2)Input.mousePosition - startTouch;
            }

            if (delta.magnitude > GameConstants.SWIPE_DEADZONE)
            {
                if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                {
                    if (delta.x < 0)
                    {
                        RegisterLeft();
                    }
                    else
                    {
                        RegisterRight();
                    }
                }

                isDragging = false;
                startTouch = Vector2.zero;
            }
        }

        #endregion

        #region Tap Mode

        private void ProcessTapInput()
        {
            bool tapped = false;
            Vector2 tapPosition = Vector2.zero;

            if (Input.GetMouseButtonDown(0))
            {
                tapped = true;
                tapPosition = Input.mousePosition;
            }
            else if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                tapped = true;
                tapPosition = Input.GetTouch(0).position;
            }

            if (!tapped) return;

            float halfScreen = Screen.width * 0.5f;
            if (tapPosition.x < halfScreen)
            {
                RegisterLeft();
            }
            else
            {
                RegisterRight();
            }
        }

        #endregion

        #region Button Mode

        private void OnLeftButtonPressed()
        {
            RegisterLeft();
        }

        private void OnRightButtonPressed()
        {
            RegisterRight();
        }

        #endregion

        private void RegisterLeft()
        {
            swipeLeftThisFrame = true;
            OnSwipeLeft?.Invoke();
        }

        private void RegisterRight()
        {
            swipeRightThisFrame = true;
            OnSwipeRight?.Invoke();
        }

        private void LoadInputMode()
        {
            int stored = PlayerPrefs.GetInt(InputModePrefsKey, (int)InputMode.Swipe);
            if (stored < 0 || stored > 2) stored = (int)InputMode.Swipe;
            currentMode = (InputMode)stored;
        }
    }
}
