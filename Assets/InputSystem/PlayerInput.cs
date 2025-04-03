using UnityEngine;
using UnityEngine.InputSystem;

public delegate void PlayerInputEvent();

public class PlayerInput : MonoBehaviour
{
    public Vector2 MoveInput { get; private set; } = Vector2.zero;
    InputSystem_Actions _Input = null;

    // Initialize with empty delegate to prevent null reference
    public PlayerInputEvent OnClick = delegate { };

    private void OnEnable()
    {
        _Input = new();
        _Input.Player.Enable();

        _Input.Player.Click.started += HandleClick;
    }

    private void HandleClick(InputAction.CallbackContext context)
    {
        OnClick?.Invoke(); // Safe invocation
    }

    private void OnDisable()
    {
        _Input.Player.Click.started -= HandleClick;

        _Input.Player.Disable();
    }

}
