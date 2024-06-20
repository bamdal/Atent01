using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test_16_Goal : TestBase
{
    public Goal goal;
    // Start is called before the first frame update

#if UNITY_EDITOR

    void Start()
    {
        
    }

    // Update is called once per frame
    protected override void OnTest1(InputAction.CallbackContext context)
    {

        Vector2Int result = goal.TestSetRandomPosition(GameManager.Instance.MazeWidth, GameManager.Instance.MazeHeight);
        Debug.Log(result);
    }

    protected override void OnTest2(InputAction.CallbackContext context)
    {
        goal.SetRandomPosition(GameManager.Instance.MazeWidth, GameManager.Instance.MazeHeight);
        GameManager.Instance.onGameEnd += (isClear) => { Debug.Log($"GameClear : {isClear}"); };
    }

    protected override void OnTest3(InputAction.CallbackContext context)
    {
        /*        int size = GameManager.Instance.MazeHeight * GameManager.Instance.MazeWidth;
                int[] count = new int[10000];

                for (int i = 0; i < 10000; i++)
                {
                    Vector2Int result = goal.TestSetRandomPosition(GameManager.Instance.MazeWidth, GameManager.Instance.MazeHeight);
                    if (result.x == 1 && result.y == 1)
                    {
                        Debug.LogError("Not Valid");
                    }

                    count[i]++;

                }
                Debug.Log("Check complete");

                for (int i = 0; i < size; i++)
                {

                    Debug.Log($"{i} : {count[i]}");
                }*/
        GameManager.Instance.GameStart();
    }

    protected override void OnTest4(InputAction.CallbackContext context)
    {
        GameManager.Instance.GameClear();
    }
#endif
}
