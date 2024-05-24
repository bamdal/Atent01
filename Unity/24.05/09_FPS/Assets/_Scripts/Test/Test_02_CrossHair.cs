using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Networking;
public class Test_02_CrossHair : TestBase
{
    public Crosshair crossHair;
    public float amount = 5.0f;
    protected override void OnTestLClick(InputAction.CallbackContext context)
    {
        crossHair.Expend(amount);
    }

    protected override void OnTest1(InputAction.CallbackContext context)
    {
        StartCoroutine(GetData());
    }

    readonly string url = "http://www.naver.com";

    IEnumerator GetData()
    {
        UnityWebRequest www = UnityWebRequest.Get(url); // 웹페이지를 가져오는 요청만들기
        yield return www.SendWebRequest();              // 위에서 만든 요청을 전송하고 경과 기다리기(전송결과 돌아올때까지 대기)

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log(www.downloadHandler.text);
        }


    }
}
