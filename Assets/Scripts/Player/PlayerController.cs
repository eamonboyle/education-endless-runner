using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum InputBackend
    {
        Auto,
        Legacy,
        InputSystem
    }

    public bool tap, swipeLeft, swipeRight, swipeUp, swipeDown;

    [SerializeField] private InputBackend inputBackend = InputBackend.Auto;
    [SerializeField] private float swipeDeadZone = 50f;

    private IInputService inputService;

    private void Awake()
    {
        inputService = CreateInputService();
    }

    private void Update()
    {
        SwipeInputState input = inputService.ReadInput();
        tap = input.Tap;
        swipeLeft = input.SwipeLeft;
        swipeRight = input.SwipeRight;
        swipeUp = input.SwipeUp;
        swipeDown = input.SwipeDown;
    }

    private IInputService CreateInputService()
    {
        switch (inputBackend)
        {
            case InputBackend.Legacy:
                return new LegacyInputService(swipeDeadZone);
            case InputBackend.InputSystem:
                return new InputSystemInputService(swipeDeadZone);
            case InputBackend.Auto:
            default:
#if ENABLE_INPUT_SYSTEM
                return new InputSystemInputService(swipeDeadZone);
#else
                return new LegacyInputService(swipeDeadZone);
#endif
        }
    }
}
