using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test_ImageNumber : TestBase
{
    [Range(-99.0f, 999.0f)]
    public int number = 0;

    public bool Start =false;

    public ImageNumber imageNumber;

    private void Update()
    {
        if (Start)
        {
            imageNumber.Number = number;
        }
    }

    protected override void OnTest1(InputAction.CallbackContext context)
    {
        imageNumber.Number = number;
    }

    protected override void OnTest2(InputAction.CallbackContext context)
    {
        imageNumber.Number = Random.Range(-100, 1000);
    }
}
