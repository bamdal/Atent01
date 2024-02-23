using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test_AStar : TestBase
{
    TestData testData1 = new TestData(1,2.0f,"a");
    TestData testData2 = new TestData(1,2.0f,"b");
    TestData testData3 = new TestData(2,2.0f,"c");
    TestData testData4 = new TestData(1,2.0f,"d");
    protected override void OnTest1(InputAction.CallbackContext context)
    {
        // TestData 정렬 확인하기
        List<TestData> list = new List<TestData>(4);
        list.Add(testData1);
        list.Add(testData4);
        list.Add(testData3);
        list.Add(testData2);
        list.Sort();
        if(testData1 ==  testData2)
        {
            Debug.Log("같다");
        }  
        
        if(testData1 !=  testData3)
        {
            Debug.Log("다르다");
        }
    }
    protected override void OnTest2(InputAction.CallbackContext context)
    {
        GridMap grid = new GridMap(5, 5);

    }
}
