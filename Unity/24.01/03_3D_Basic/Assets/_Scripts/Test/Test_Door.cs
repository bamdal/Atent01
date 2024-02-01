using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test_Door : TestBase
{

    public TextMeshPro text;
    public DoorBase door;
    protected override void OnTest1(InputAction.CallbackContext context)
    {
        Vector3 cameraForward = Camera.main.transform.forward;
        float angle = Vector3.SignedAngle(text.transform.forward, cameraForward, Vector3.up);
        Debug.Log(angle);
    }
}
