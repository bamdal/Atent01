using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test_Instantiate : TestBase
{
    public GameObject prefab;

    protected override void OnTest1(InputAction.CallbackContext context)
    {
        new GameObject(); // 비어있는 게임 오즈젝트 생성
    }
    protected override void OnTest2(InputAction.CallbackContext context)
    {
        Instantiate(prefab); // 프리팹을 이용해서 게임오브젝트 생성 로컬좌표(0,0,0)

        // 로컬 좌표 : 부모기준의 좌표 (부모가 없다면 월드가 부모)
        // 월드 좌표 : 월드의 원점(origin)을 기준으로 한 좌표
    }

    protected override void OnTest3(InputAction.CallbackContext context)
    {
        // 함수 오버로딩 : 함수의 파라메터만 다르게 해서 다양한 입력을 받을 수 있게함
        Instantiate(prefab, new Vector3(5,0,0),Quaternion.identity); // 게임 오브젝트를 (5,0,0)에 넣음, 회전 없음
    }

    protected override void OnTest4(InputAction.CallbackContext context)
    {
        Instantiate(prefab,this.transform);
    }
    protected override void OnTest5(InputAction.CallbackContext context)
    {
        
    }
}
