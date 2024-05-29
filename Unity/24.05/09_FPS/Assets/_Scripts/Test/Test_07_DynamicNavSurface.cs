using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test_07_DynamicNavSurface : TestBase
{
    public int mazeSeed = -1;

    [Header("셀")]
    public Direction pathData;
    public CellVisualizer cellVisualizer;

    [Header("미로")]
    public MazeVisualizer backTracking;
    public int width = 5;
    public int height = 5;

    public MazeVisualizer eller;

    public MazeVisualizer wilson;

    public NavMeshSurface navMeshSurface;
    AsyncOperation navAsync;

    public MazeGenerator navGenerator;
    protected override void OnTest1(InputAction.CallbackContext context)
    {
        backTracking.Clear();

        BackTracking mazeBackTracking = new BackTracking();
        mazeBackTracking.MakeMaze(width, height, mazeSeed);
        backTracking.Draw(mazeBackTracking);


        eller.Clear();

        Eller mazeEller = new Eller();
        mazeEller.MakeMaze(width, height, seed);
        eller.Draw(mazeEller);

        wilson.Clear();

        Wilson mazeWilson = new Wilson();
        mazeWilson.MakeMaze(width, height, seed);
        wilson.Draw(mazeWilson);

    }

    protected override void OnTest2(InputAction.CallbackContext context)
    {
        StartCoroutine(UpdateSurface());
    }

    protected override void OnTest3(InputAction.CallbackContext context)
    {

        navGenerator.Generate(width, height);

    }

    IEnumerator UpdateSurface()
    {
        navAsync = navMeshSurface.UpdateNavMesh(navMeshSurface.navMeshData);
        while(!navAsync.isDone)
        {
            yield return null;
        }
        Debug.Log("Nav Surface Updated");
    }
}
