using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test_Counter : TestBase
{
    [Range(-99.0f, 999.0f)]
    public int number = 0;

    public bool Start = false;

    public GameManager.GameState gameState;

#if UNITY_EDITOR
    private void Update()
    {
        if (Start)
        {
            GameManager.Instance.Test_SetFlagCount(number);
        }
    }

    protected override void OnTest1(InputAction.CallbackContext context)
    {
        GameManager.Instance.Test_SetFlagCount(number);
    }

    protected override void OnTest2(InputAction.CallbackContext context)
    {
        GameManager.Instance.Test_SetFlagCount(Random.Range(-100, 1000));
    }

    protected override void OnTest3(InputAction.CallbackContext context)
    {
        GameManager.Instance.Test_StateChange(gameState);
    }
#endif
}
