using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DetailInfoUI : MonoBehaviour
{
    Image icon;

    TextMeshProUGUI itemName;
    TextMeshProUGUI price;
    TextMeshProUGUI description;

    CanvasGroup canvasGroup;

    /// <summary>
    /// 알파값이 변하는 속도
    /// </summary>
    public float alphaChangeSpeed = 10.0f;

    RectTransform rectTransform;

    /// <summary>
    /// 일시 정지 모드(true면 일시정지, false면 사용중)
    /// </summary>
    bool isPause = false;

    /// <summary>
    /// 일시 정지 모드를 확인하고 설정하는 프로퍼티
    /// </summary>
    public bool IsPause
    {
        get => isPause;
        set
        {
            isPause = value;
            if (isPause)
            {
                Close();    // 일시 정지가 되면 열려있던 상세 정보창도 닫는다
            }
        }
    }
    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0.0f;

        Transform child = transform.GetChild(0);
        icon = child.GetComponent<Image>();
        child = transform.GetChild(1);
        itemName = child.GetComponent<TextMeshProUGUI>();
        child = transform.GetChild(2);
        price = child.GetComponent<TextMeshProUGUI>();
        child = transform.GetChild(4);
        description = child.GetComponent<TextMeshProUGUI>();
        rectTransform = GetComponent<RectTransform>();
    }


    /// <summary>
    /// 상세 정보창열기 함수
    /// </summary>
    /// <param name="itemData">표시할 아이템 데이터</param>
    public void Open(ItemData itemData)
    {
        // 컴포넌트들 채우기
        // 알파 변경시작(0->1)
        if (!IsPause && itemData != null)
        {
            // 컴포넌트들 채우기
            transform.position = Vector2.zero;
            icon.sprite = itemData.itemIcon;
            itemName.text = itemData.itemName;
            price.text = itemData.price.ToString("N0");
            description.text = itemData.itemDescription;
            canvasGroup.alpha = 0.000001f; // 알파가 0보다 커야 MovePosition이 작동하므로 살짝 올려줌
            MovePosition(Mouse.current.position.ReadValue());   // 보이기전 커서 위치로 상세 정보창 옮기기

            // 알파 변경시작 0->1
            StopAllCoroutines();
            StartCoroutine(FadeIn());
        }

    }


    /// <summary>
    /// 상세 정보창 닫는 함수
    /// </summary>
    public void Close()
    {
        // 알파 변경 시작(1->0)
        StopAllCoroutines();
        StartCoroutine(FadeOut());
    }

    /// <summary>
    /// 상세정보창을 움직이는 함수
    /// </summary>
    /// <param name="screenPos">스크린 좌표</param>
    public void MovePosition(Vector2 screenPos)
    {
        // Screen.width;   // 화면의 가로 해상도
        // Screen.height;  // 화면의 세로 해상도

        // 디테일 인포창을 screenPos로 이동시킨다.
        // 단 디테일 인포창이 화면밖으로 벗어날경우 보이게 화면 이동

        if(canvasGroup.alpha > 0.0f)    // 알파가 보이는 상황인지 확인
        {
            int overX = Mathf.Max(0, (int)(rectTransform.sizeDelta.x + screenPos.x) - Screen.width); // 얼마나 넘치는지 계산
            int overY = Mathf.Max(0, (int)(rectTransform.sizeDelta.y + screenPos.y) - Screen.height); // 얼마나 넘치는지 계산
            
            screenPos.x -= overX;
            screenPos.y -= overY;
            rectTransform.position = screenPos;
        }


    }

    /// <summary>
    /// 알파 0->1 코루틴
    /// </summary>
    /// <returns></returns>
    IEnumerator FadeIn()
    {
        while (canvasGroup.alpha < 1.0f)
        {
            canvasGroup.alpha += Time.deltaTime * alphaChangeSpeed;
            yield return null;
        }
        canvasGroup.alpha = 1.0f;
    }

    /// <summary>
    /// 알파 1->0 코루틴
    /// </summary>
    /// <returns></returns>
    IEnumerator FadeOut()
    {
        while (canvasGroup.alpha > 0.0f)
        {
            canvasGroup.alpha += Time.deltaTime * -alphaChangeSpeed;
            yield return null;
        }
        canvasGroup.alpha = 0.0f;
    }
}
