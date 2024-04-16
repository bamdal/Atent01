using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test_RankingUI : TestBase
{
    public int rank;
    public int actionRecord;
    public float timeRecord;
    public string testName = "테스트";


    protected override void OnTest1(InputAction.CallbackContext context)
    {
        GameManager.Instance.RankDataManager.Test_RankSetting();
    }

    protected override void OnTest2(InputAction.CallbackContext context)
    {
        GameManager.Instance.RankDataManager.Test_ActionUpdate(actionRecord, testName);
    }

    protected override void OnTest3(InputAction.CallbackContext context)
    {
        GameManager.Instance.RankDataManager.Test_TimeUpdate(timeRecord, testName);
    }

    protected override void OnTest4(InputAction.CallbackContext context)
    {
        GameManager.Instance.RankDataManager.Test_Save();
    }

    protected override void OnTest5(InputAction.CallbackContext context)
    {
        GameManager.Instance.RankDataManager.Test_Load();
    }
}
