using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test_Flag : TestBase
{
    Board board;
    public int value = 0;

    private void Start()
    {


    }
#if UNITY_EDITOR
    protected override void OnTest1(InputAction.CallbackContext context)
    {
        GameManager.Instance.Test_SetFlagCount(value);
    }

    protected override void OnTest2(InputAction.CallbackContext context)
    {
        GameManager.Instance.Board.Test_OpenAllCover();

    }
#endif

}

// 보드가 초기화 될 때 mineCount가 FlagCounter에 설정된다.
// 보드에 깃발을 설치하면 FlagCounter가 감소한다.
// 보드에서 깃발 설치를 해제하면 FlagCounter가 증가한다.