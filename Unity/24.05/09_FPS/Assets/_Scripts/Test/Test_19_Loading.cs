using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test_19_Loading : TestBase
{
    public LoadingScreen loadingScreen;

    private void Start()
    {
        loadingScreen = FindAnyObjectByType<LoadingScreen>();
    }


    protected override void OnTest1(InputAction.CallbackContext context)
    {
       
    }

    protected override void OnTest2(InputAction.CallbackContext context)
    {
  
    }
}
