using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    // 씬 로딩 진행도에 따라 슬라이더value변경 (100일때 1)
    // 로딩 중에는  Loading . , Loading .. , Loading ... 변화
    // 로딩이 완료되면 Loading 글자가 Loading Complete! 로 변경되고 PressText활성화
    // 로딩 진행도는 onMazeGenerated가 실행되면 70 onSpawnComplete가 실행되면 100
    // 로딩이 완료 되었을 때 아무 키보드 입력이나 마우스 클릭이 입력되면 로딩 스크린이 비활성화
    // GameManager.Instanse.GameStart(); 실행
    // 씬로딩 진행도는 목표치까지 꾸준히 증가

    TextMeshProUGUI loadingText;
    TextMeshProUGUI loadingDotText;
    TextMeshProUGUI pressText;
    Slider loadingSlider;

    PlayerInputActions playerInputActions;

    float currentProgress = 0;

    float CurrentProgress
    {
        get => currentProgress;
        set
        {
            currentProgress = Mathf.Min(targetProgress, value);
            loadingSlider.value = currentProgress;

            if (currentProgress > 0.9999f)
            {
                OnLoadingComplete();
            }
        }
    }


    float targetProgress = 0.0f;

    string[] loadingStrings = { " .", " . .", " . . .", " . . . ." };


    private void Awake()
    {
        Transform child = transform.GetChild(0);
        loadingText = child.GetComponent<TextMeshProUGUI>();
        child = transform.GetChild(1);
        loadingDotText = child.GetComponent<TextMeshProUGUI>();
        child = transform.GetChild (2);
        pressText = child.GetComponent<TextMeshProUGUI>();
        child = transform.GetChild(3);
        loadingSlider = child.GetComponent<Slider>();

        playerInputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        playerInputActions.UI.AnyKey.performed += OnAnyKey;

    }



    private void OnDisable()
    {
        playerInputActions.UI.AnyKey.performed -= OnAnyKey;
        playerInputActions.UI.Disable();
    }

    private void Update()
    {
        CurrentProgress += Time.deltaTime;
    }


    public void Initialized()
    {
        CurrentProgress = 0.0f;
        targetProgress = 0.5f;
        StartCoroutine(TextCoroutine());
    }

    IEnumerator TextCoroutine()
    {
        int index = 0;
        while (CurrentProgress < 1)
        {
            yield return new WaitForSeconds(0.2f);
            loadingDotText.text = loadingStrings[index];
            index = (++index)%loadingStrings.Length;
            
        }
    }

    public void OnLoadingProgress(float progress)
    {
        targetProgress = progress;
        Debug.Log(progress);
    }

    private void OnLoadingComplete()
    {
        loadingText.text = "Loading Complete!";
        loadingDotText.gameObject.SetActive(false);
        pressText.gameObject.SetActive(true);
        loadingSlider.value = 1;

        StopAllCoroutines();

        playerInputActions.UI.Enable();

    }

    private void OnAnyKey(InputAction.CallbackContext _)
    {
        GameManager.Instance.GameStart();
        this.gameObject.SetActive(false);
        Debug.Log("AnyKey");
    }
}
