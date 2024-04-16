using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{

    PlayerInputActions inputActions;

    Vector3 direction;
    public float speed = 5.0f;

    CharacterController characterController;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
        characterController = GetComponent<CharacterController>();
    }
    private void OnEnable()
    {
        inputActions.Enable();
        inputActions.Player.Move.performed += OnMove;

    }

    private void OnDisable()
    {
        inputActions.Player.Move.performed -= OnMove;
        inputActions.Disable();
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        Vector2 move = context.ReadValue<Vector2>();
        direction.x = move.x;
        direction.z = move.y;
        if (context.canceled)
        {

            direction = Vector3.zero;
        }
    }

    void Start()
    {
        characterController.Move(direction);
    }

    // Update is called once per frame
    void Update()
    {
        characterController.Move(Time.deltaTime* speed * direction);
    }
}
