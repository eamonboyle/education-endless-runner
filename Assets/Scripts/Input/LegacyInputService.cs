using UnityEngine;

public class LegacyInputService : IInputService
{
    private readonly float swipeDeadZone;

    private bool isDragging;
    private Vector2 startTouch;
    private Vector2 swipeDelta;

    public LegacyInputService(float swipeDeadZone)
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

        if (Input.GetMouseButtonDown(0))
        {
            tap = true;
            isDragging = true;
            startTouch = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            Reset();
        }

        if (Input.touches.Length != 0)
        {
            Touch touch = Input.touches[0];

            if (touch.phase == TouchPhase.Began)
            {
                tap = true;
                isDragging = true;
                startTouch = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                Reset();
            }
        }

        swipeDelta = Vector2.zero;
        if (isDragging)
        {
            if (Input.touches.Length != 0)
            {
                swipeDelta = Input.touches[0].position - startTouch;
            }
            else if (Input.GetMouseButton(0))
            {
                swipeDelta = (Vector2)Input.mousePosition - startTouch;
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
}
