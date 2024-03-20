using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ItemDividerUI : MonoBehaviour
{
    // 2개 이상이 들어있는 슬롯을 쉬프트클릭을 하면 열린다
    // DividCounterInputField, PlusButton, MinusButton, DividerSlider를 이용해서 아이템을 나눌 개수를 설정 할 수 있다
    // DividCounterInputField와 DividerSlider는 서로 값이 연동된다
    // 아이템을 나누는 개수는 반드시 1~(슬롯에 들어있는 아이템수 -1) 이어야 한다.
    // OK버튼을 누르면 슬롯에서 설정된 개수만큼 아이템이 빠지고 빠진만큼 Temp슬롯에 들어간다.

    /// <summary>
    /// 아이템을 나눌 슬롯
    /// </summary>
    InvenSlot targetSlot;

    /// <summary>
    /// 아이템을 나눌때의 최소 개수를 나타내는 상수
    /// </summary>
    const uint MinItemCount = 1;

    /// <summary>
    /// 아이템을 나눌 때의 최대 개수(targetSlot이 없으면 MinItemCount)
    /// </summary>
    uint MaxItemCount => targetSlot.IsEmpty ? MinItemCount : targetSlot.ItemCount - 1;

    /// <summary>
    /// 아이템을 나눌 개수
    /// </summary>
    uint dividCount = MinItemCount;

    /// <summary>
    /// 아이템 개수 변경시 처리할 일을 하는 프로퍼티
    /// </summary>
    uint DividCount
    {
        get => dividCount;
        set
        {
            dividCount = Math.Clamp(value,MinItemCount,MaxItemCount);
            inputField.text = dividCount.ToString("N0");
            slider.value = dividCount;
        }
    }

    /// <summary>
    /// OK버튼을 눌렀을 때 실행되는 델리게이트(uint : targetSlot의 인덱스, uint : DividCount)
    /// </summary>
    public Action<uint, uint> onOkClick;

    /// <summary>
    /// Cancel버튼을 눌렀을 때 실행되는 델리게이트
    /// </summary>
    public Action onCancelClick;
    /// <summary>
    /// 입력용 인풋 액션
    /// </summary>
    PlayerInputActions inputActions;

    // 컴포넌트들
    Image icon;
    TMP_InputField inputField;
    Slider slider;

    private void Awake()
    {
        inputActions = new PlayerInputActions();

        Transform child = transform.GetChild(0);
        icon = child.GetComponent<Image>();
        child = transform.GetChild(1);
        inputField = child.GetComponent<TMP_InputField>();
        inputField.onValueChanged.AddListener((text) =>
        {
            if (uint.TryParse(text, out uint value))
            {
                DividCount = value;
            }
            else
            {
                // 음수 값이 들어올때 1로 고정
                DividCount = MinItemCount;
                Debug.Log("파싱 실패");
            }
        });

        child = transform.GetChild(2);
        Button plus = child.GetComponent<Button>();
        plus.onClick.AddListener(() =>
        {
            DividCount++;
        });

        child = transform.GetChild(3);
        Button minus = child.GetComponent<Button>();
        minus.onClick.AddListener(() =>
        {
            DividCount--;
        });

        child = transform.GetChild(4);
        slider = child.GetComponent<Slider>();
        slider.onValueChanged.AddListener((value) =>
        {
            DividCount = (uint)value;
        });

        child = transform.GetChild(5);
        Button ok = child.GetComponent<Button>();
        ok.onClick.AddListener(() =>
        {
            onOkClick?.Invoke(targetSlot.Index,DividCount);
            Close();
        });

        child = transform.GetChild(6);
        Button cancel = child.GetComponent<Button>();
        cancel.onClick.AddListener(() =>
        {
            onCancelClick?.Invoke();
            Close();
        });

    }


    private void OnEnable()
    {
        inputActions.UI.Enable();
        inputActions.UI.Click.performed += OnClick;
        inputActions.UI.Wheel.performed += OnWheel;

    }

    private void OnDisable()
    {
        inputActions.UI.Wheel.performed -= OnWheel;
        inputActions.UI.Click.performed -= OnClick;
        inputActions.UI.Disable();
    }




    /// <summary>
    /// 아이템분리창을 여는 함수
    /// </summary>
    /// <param name="target">아이템을 나눌 대상 슬롯</param>
    /// <returns>true면 여는데 성공, false면 여는데 실패</returns>
    public bool Open(InvenSlot target)
    {
        bool result = false;
        if (!target.IsEmpty && target.ItemCount > MinItemCount) // 슬롯이 비어있지 않고 아이템 개수가 1보다 많다
        {
            targetSlot = target;
            icon.sprite = targetSlot.ItemData.itemIcon; // 컴포넌트 초기설정
            slider.minValue = MinItemCount;
            slider.maxValue = MaxItemCount;
            DividCount = targetSlot.ItemCount / 2;
            

            result = true;
            gameObject.SetActive(true);
        }
        return result;
  
     
    }

    /// <summary>
    /// 아이템 분리창을 닫는 함수
    /// </summary>
    public void Close()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 클릭하면 실행되는 함수(UI밖을 클릭했는지 구별하기 위함)
    /// </summary>
    /// <param name="context"></param>
    private void OnClick(InputAction.CallbackContext context)
    {
        if (!MousePointInRect())  // UI 영역 밖을 클릭했으면
        {
            Close();    // 닫기
        }
    }



    /// <summary>
    /// 마우스 휠입력으로 아이템 분리창 값 수정
    /// </summary>
    /// <param name="context"></param>
    private void OnWheel(InputAction.CallbackContext context)
    {
        if(MousePointInRect())
        {
            if (context.ReadValue<float>() > 0)
            {
                DividCount++;   // 휠을 위로 올리면 증가
            }
            else
            {
                DividCount--;   // 휠을 아래로 내리면 감소
            }
        }
    }

    /// <summary>
    /// 마우스 포인터가 UI rect 안에 있는지 확인하는 함수
    /// </summary>
    /// <returns>true면 안에있고 false면 밖에있다</returns>
    bool MousePointInRect()
    {
        Vector2 screenPos = Mouse.current.position.ReadValue();
        Vector2 diff = screenPos - (Vector2)transform.position; // 이 UI의 피봇에서 마우스 포인터가 얼마나 떨어졌는지 계산

        RectTransform rectTransform = (RectTransform)transform;
        return rectTransform.rect.Contains(diff); 

    }

}
