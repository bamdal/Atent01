using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test_AStar_GridMap : TestBase
{
    public int width;
    public int height;

    GridMap gridMap;

    private void Start()
    {
        gridMap = new GridMap(width, height);
    }

    protected override void OnTest1(InputAction.CallbackContext context)
    {
        
    }
}
