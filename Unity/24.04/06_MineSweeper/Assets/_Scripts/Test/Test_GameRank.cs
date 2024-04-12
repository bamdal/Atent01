using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test_GameRank : TestBase
{
    public int rank;
    public int actionRecord;
    public float timeRecord;
    public string testName = "테스트";

    public RankLine line1;
    public RankLine line2;


    protected override void OnTest1(InputAction.CallbackContext context)
    {
        line1.SetData(rank, actionRecord, testName);
    }

    protected override void OnTest2(InputAction.CallbackContext context)
    {
        line2.SetData(rank, timeRecord, testName);

    }

    protected override void OnTest3(InputAction.CallbackContext context)
    {
        line1.ClearLine();
        line2.ClearLine();
    }
}
