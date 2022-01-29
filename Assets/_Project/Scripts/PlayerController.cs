using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(InputController))]
[RequireComponent(typeof(CharacterController))]
[DisallowMultipleComponent]
public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 5;
    public float sprintSpeed = 8;
    public float raycastRange = 4;

    private CharacterController _characterController;
    private InputController _input;

    private void Update()
    {
        Move();
        //CastRay();
    }

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _input = GetComponent<InputController>();
    }

    private void Move()
    {
        Vector2 move = _input.Move;
        float sprint = _input.Sprint;

        Vector3 inputDirection = new Vector3(move.x, 0, 0);

        float speed = sprint > 0 ? sprintSpeed : walkSpeed;
    
        _characterController.Move(inputDirection.normalized * (speed * Time.deltaTime));
    }

    // private void CastRay()
    // {
    //     Vector3 origin = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
    //     Vector3 direction = transform.TransformDirection(Vector3.forward);
    //
    //     bool hit = Physics.Raycast(origin, direction, out var hitInfo, raycastRange);
    //     Debug.DrawRay(origin, direction * raycastRange, Color.red);
    //     if (!hit) return;
    //
    //     GameObject hitObject = hitInfo.transform.gameObject;
    //     float interact = _input.Interact;
    //
    //     if (interact > 0)
    //     {
    //         if (hitObject.TryGetComponent(typeof(IInteractable), out Component comp))
    //         {
    //             ((IInteractable)comp).Interact(gameObject);
    //         }
    //     }
    // }
}