using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test_Chat : TestBase
{
    public Logger logger;

    private void Start()
    {
        logger = FindAnyObjectByType<Logger>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        inputActions.Test.Enter.performed += OnEnter;
    }

    protected override void OnDisable()
    {
        inputActions.Test.Enter.performed -= OnEnter;
        base.OnDisable();
    }


    private void OnEnter(InputAction.CallbackContext context)
    {
        logger.InputFieldFocusOn();
    }

    protected override void OnTest1(InputAction.CallbackContext context)
    {
        Color color = Color.red;
        string colorText = ColorUtility.ToHtmlStringRGBA(color);
        logger.Log($"<#{colorText}>색 적용 </color> 색 꺼짐");
    }
}
