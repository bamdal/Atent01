using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{

    PlayerInputActions inputActions;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
    }

    private void Start()
    {
        GameManager.Instance.GameState = GameState.Title;   // 통일성 유지를 위해 넣은 것(실직적으로 하는일 없음)
        GameManager.Instance.TurnController.TurnManagerStop();

    }

    private void OnEnable()
    {
        inputActions.Title.Enable();
        inputActions.Title.Anything.performed += OnAnyThing;

    }



    private void OnDisable()
    {
        inputActions.Title.Anything.performed -= OnAnyThing;
        inputActions.Title.Disable();

    }
    private void OnAnyThing(InputAction.CallbackContext context)
    {
        SceneManager.LoadScene(1);
    }
}
