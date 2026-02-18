using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using InputSystemTouchPhase = UnityEngine.InputSystem.TouchPhase;
#endif

public class InputSystemInputService : IInputService
{
#if ENABLE_INPUT_SYSTEM
    private readonly float swipeDeadZone;

    private bool isDragging;
    private Vector2 startTouch;
    private Vector2 swipeDelta;

    public InputSystemInputService(float swipeDeadZone)
    {
        this.swipeDeadZone = swipeDeadZone;
    }

    public SwipeInputState ReadInput()
    {
        bool tap = false;
        bool swipeLeft = false;
        bool swipeRight = false;
        bool swipeUp = false;
        bool swipeDown = false;

        bool hasTouchscreen = Touchscreen.current != null;
        bool hasMouse = Mouse.current != null;

        if (hasMouse && Mouse.current.leftButton.wasPressedThisFrame)
        {
            tap = true;
            isDragging = true;
            startTouch = Mouse.current.position.ReadValue();
        }
        else if (hasMouse && Mouse.current.leftButton.wasReleasedThisFrame)
        {
            Reset();
        }

        if (hasTouchscreen)
        {
            var touch = Touchscreen.current.primaryTouch;
            InputSystemTouchPhase phase = touch.phase.ReadValue();

            if (phase == InputSystemTouchPhase.Began)
            {
                tap = true;
                isDragging = true;
                startTouch = touch.position.ReadValue();
            }
            else if (phase == InputSystemTouchPhase.Ended || phase == InputSystemTouchPhase.Canceled)
            {
                Reset();
            }
        }

        swipeDelta = Vector2.zero;
        if (isDragging)
        {
            if (hasTouchscreen && Touchscreen.current.primaryTouch.press.isPressed)
            {
                swipeDelta = Touchscreen.current.primaryTouch.position.ReadValue() - startTouch;
            }
            else if (hasMouse && Mouse.current.leftButton.isPressed)
            {
                swipeDelta = Mouse.current.position.ReadValue() - startTouch;
            }
        }

        if (swipeDelta.magnitude > swipeDeadZone)
        {
            float x = swipeDelta.x;
            float y = swipeDelta.y;

            if (Mathf.Abs(x) > Mathf.Abs(y))
            {
                swipeLeft = x < 0;
                swipeRight = x > 0;
            }
            else
            {
                swipeDown = y < 0;
                swipeUp = y > 0;
            }

            Reset();
        }

        return new SwipeInputState(tap, swipeLeft, swipeRight, swipeUp, swipeDown);
    }

    private void Reset()
    {
        isDragging = false;
        startTouch = Vector2.zero;
        swipeDelta = Vector2.zero;
    }
#else
    private readonly LegacyInputService fallback;

    public InputSystemInputService(float swipeDeadZone)
    {
        fallback = new LegacyInputService(swipeDeadZone);
    }

    public SwipeInputState ReadInput()
    {
        return fallback.ReadInput();
    }
#endif
}
