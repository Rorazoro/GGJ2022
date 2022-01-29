using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
[DisallowMultipleComponent]
public class InputController : MonoBehaviour
{
    public Vector2 Move { get; private set; }
    public bool Jump { get; private set; }
    public bool FlipWorld { get; private set; }

    public void OnMove(InputAction.CallbackContext context)
    {
        Move = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        Jump = context.ReadValueAsButton();
    }
    
    public void OnFlipWorld(InputAction.CallbackContext context)
    {
        FlipWorld = context.ReadValueAsButton();
    }
}
