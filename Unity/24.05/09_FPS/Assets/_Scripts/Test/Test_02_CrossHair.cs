using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test_02_CrossHair : TestBase
{
    public Crosshair crossHair;
    public float amount = 5.0f;
    protected override void OnTestLClick(InputAction.CallbackContext context)
    {
        crossHair.Expend(amount);
    }
}
