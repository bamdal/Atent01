using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test_SaveLoad : TestBase
{
    public int score;

    Player player;


    public RankPanel panel;
#if UNITY_EDITOR

    private void Start()
    {
        player = GameManager.Instance.Player;
        panel.Test_LoadRankPanel();
        score = 100;
    }
    protected override void OnTest1(InputAction.CallbackContext context)
    {
        panel.Test_DefaultRankPanel();
        panel.Test_SaveRankData();

    }
    protected override void OnTest2(InputAction.CallbackContext context)
    {
        panel.Test_LoadRankPanel();
    }

    protected override void OnTest3(InputAction.CallbackContext context)
    {
        panel.Test_UpdateRankPanel(score);
    }

    protected override void OnTest4(InputAction.CallbackContext context)
    {
        player.Test_SetScore(score);
    }

    protected override void OnTest5(InputAction.CallbackContext context)
    {
        player.Test_Die();

    }
#endif
}
