using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test_06_Maze : TestBase
{

    public int mazeSeed = -1;

    [Header("셀")]
    public Direction pathData;
    public CellVisualizer cellVisualizer;

    [Header("미로")]
    public MazeVisualizer backTracking;
    public int width = 5;
    public int height = 5;
    protected override void OnTest1(InputAction.CallbackContext context)
    {
        cellVisualizer.RefreshWall((byte)pathData);
    }

    protected override void OnTest2(InputAction.CallbackContext context)
    {
        Debug.Log(cellVisualizer.GetPath());
    }

    protected override void OnTest3(InputAction.CallbackContext context)
    {

        backTracking.Clear();

        BackTracking maze = new BackTracking();
        maze.MakeMaze(width, height, mazeSeed);
        backTracking.Draw(maze);

    }
}
