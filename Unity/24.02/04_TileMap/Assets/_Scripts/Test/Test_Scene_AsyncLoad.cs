using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Test_Scene_AsyncLoad : TestBase
{
    public string nextSceneName = "LoadSampleScene";
    AsyncOperation async;

    protected override void OnTest1(InputAction.CallbackContext context)
    {
        async = SceneManager.LoadSceneAsync(nextSceneName);
        async.allowSceneActivation = false; // 비동기 씬 로딩이 완료되어도 자동으로 씬 전환을 하지 않는다.
    }

    protected override void OnTest2(InputAction.CallbackContext context)
    {
        async.allowSceneActivation = true;  // 비동기 씬 로딩이 완료되면 자동으로 씬 전환을 한다.

    }

    protected override void OnTest3(InputAction.CallbackContext context)
    {
        StartCoroutine(LoadSceneCoroutine());
    }

    IEnumerator LoadSceneCoroutine()
    {
        async = SceneManager.LoadSceneAsync(nextSceneName);
        async.allowSceneActivation = false;
        while (async.progress < 0.9f)   //allowSceneActivation가 false면 0.9가 최대이다(로딩 완료 == 0.9)
        {
            Debug.Log($"async.progress : {async.progress}");
            yield return null;
        }
        Debug.Log("Loading Complete");
    }
}
