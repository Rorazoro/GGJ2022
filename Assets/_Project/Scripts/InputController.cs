using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
[DisallowMultipleComponent]
public class InputController : MonoBehaviour
{
    public Vector2 Move { get; private set; }
    public float Sprint { get; private set; }
    public float Interact { get; private set; }

    public void OnMove(InputValue value)
    {
        Move = value.Get<Vector2>();
    }

    public void OnSprint(InputValue value)
    {
        Sprint = value.Get<float>();
    }

    public void OnInteract(InputValue value)
    {
        Interact = value.Get<float>();
    }
}
