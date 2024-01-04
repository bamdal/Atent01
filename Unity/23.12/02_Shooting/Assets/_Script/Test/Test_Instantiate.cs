using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test_Instantiate : TestBase
{
    public GameObject prefab;

    private void Start()
    {

    }

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
        StartCoroutine(TestCorution()); // 정지 안해도 되는경우

        StopAllCoroutines(); // 실행중인 모든 코루틴 정지

        // 정지시킬 필요가 있을시
        IEnumerator cor = TestCorution();
        StartCoroutine(cor);
        StopCoroutine(cor);
    }

    // 코루틴의 가장 큰 특징
    // 코루틴이 실행 되었을 떄 이전 yield return 다음부터 이어서 시작한다.

    IEnumerator TestCorution()
    {
        Debug.Log("시작");
        /*        yield return null;                      // 다음 프레임까지 대기
                yield return new WaitForEndOfFrame();   // 프레임이 끝날때까지 대기*/
        yield return new WaitForSeconds(1.5f);
        Debug.Log("종료");
    }

}
