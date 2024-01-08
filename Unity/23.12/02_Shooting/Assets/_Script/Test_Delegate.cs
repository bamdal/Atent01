using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Test_Delegate : TestBase
{
    // Delegate - 델리게이트
    // 함수를 저장할 수 있는 변수 타입
    // 함수 체인(chain)이 가능
    // 어떤 사건이 발생했음을 알릴때 사용하면 편리

    public delegate void TestDelegate();    // 델리게이트 타입을 하나 생성  
                                            //(이 델리게이트는 파라메터가 없고 리턴 값도 없는 함수만 저장가능)
    TestDelegate aaa; // TestDelegate타입으로 함수를 저장할 수 있는 aaa라는 변수를 만듬
   





    void TestRun()
    {
        Debug.Log("TestRun");
    }
    void TestRun2()
    {
        Debug.Log("TestRun");
    }
    void TestRun3()
    {
        Debug.Log("TestRun");
    }

    public delegate int TestDelegate2(int a, int b);
    TestDelegate2 bbb;

    int TestRun4(int a, int b)
    {
        return a + b;
    }

    private void Start()
    {
        aaa = TestRun; // 이전에 등록된 함수는 모두 무시하고 TestRun만 추가
        aaa += TestRun2; // 이전에 등록된 함수 뒤에 TestRun 추가
        aaa = TestRun3 + aaa; // aaa 맨앞에 TestRun 함수 추가

        bbb = TestRun4;
    }

    protected override void OnTest1(InputAction.CallbackContext context)
    {
        aaa(); Debug.Log("TestRun");
    }
    protected override void OnTest2(InputAction.CallbackContext context)
    {
        aaa = null;
    }

    protected override void OnTest3(InputAction.CallbackContext context)
    {
        aaa?.Invoke(); // aaa가 null이 아닐때만 시행

        // nullable 타입 (null값을 가질 수 있는 타입)
    }

    //Action : 리턴값이 없는 함수를 저장할 수 있는 델리게이트
    Action ccc; //파라메터없고 리턴값 없는 함수를 저장할 수 있는 델리게이트
    Action<int> ddd; // 파라메터로 int 하나 사용하고 리턴값 없는 함수를 저장
    Action<int,int> eee; // 파라메터로 int 두개 사용하고 리턴값 없는 함수를 저장

    Func<int> f;        // 리턴 타입이 int인 함수를 저장할 수 있는 델리게이트
    Func<int,float> g;  // 파라메터가 int하나고, 리턴 타입이 float인 델리게이트 -  마지막만 리턴 앞에 몇개는 파라메터

    UnityEvent u1;

    void Test_Del()
    {
        Debug.Log("Test_D");
    }




}
