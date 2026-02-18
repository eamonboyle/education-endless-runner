public readonly struct SwipeInputState
{
    public bool Tap { get; }
    public bool SwipeLeft { get; }
    public bool SwipeRight { get; }
    public bool SwipeUp { get; }
    public bool SwipeDown { get; }

    public SwipeInputState(bool tap, bool swipeLeft, bool swipeRight, bool swipeUp, bool swipeDown)
    {
        Tap = tap;
        SwipeLeft = swipeLeft;
        SwipeRight = swipeRight;
        SwipeUp = swipeUp;
        SwipeDown = swipeDown;
    }
}
