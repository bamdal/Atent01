using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test_09_GunChange : TestBase
{
    public GunType gunType = GunType.Revolver;

    Player player;

    private void Start()
    {
        player = GameManager.Instance.Player;
    }
    protected override void OnTest1(InputAction.CallbackContext context)
    {
        player.GunChage(gunType);
    }


}
