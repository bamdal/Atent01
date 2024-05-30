using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test_08_Gun : TestBase
{
    public Revolber revolber;

    public Shotgun shotGun;

    public AssaultRifle AssaultRifle;



    protected override void OnTest1(InputAction.CallbackContext context)
    {
        revolber.Test_Fire();
    }

    protected override void OnTest2(InputAction.CallbackContext context)
    {
        shotGun.Test_Fire();
    }

    protected override void OnTest3(InputAction.CallbackContext context)
    {
        AssaultRifle.Test_Fire(!context.canceled);
    }

    protected override void OnTest4(InputAction.CallbackContext context)
    {
        revolber.Reload();
    }
}
