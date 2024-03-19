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
    // 아이템을 나누는 개수는 반드시 1~(슬롯에 들어있는 아이템수 -1) 이어야 한다.
    // OK버튼을 누르면 슬롯에서 설정된 개수만큼 아이템이 빠지고 빠진만큼 Temp슬롯에 들어간다.

    /// <summary>
    /// 아이템을 나눌 슬롯
    /// </summary>
    InvenSlot targetSlot;

    const int MinItemCount = 1;

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
            dividCount = value;
        }
    }

    /// <summary>
    /// OK버튼을 눌렀을 때 실행되는 델리게이트(uint : targetSlot의 인덱스, uint : DividCount)
    /// </summary>
    public Action<uint, uint> onOkClick;

    /// <summary>
    /// Cancel버튼을 눌렀을 때 실행되는 델리게이트
    /// </summary>
    public Action onCancel;
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
        child = transform.GetChild(4);
        slider = child.GetComponent<Slider>();
    }

    private void OnEnable()
    {
        inputActions.UI.Enable();
        inputActions.UI.Click.performed += OnClick;

    }

    private void OnDisable()
    {
        inputActions.UI.Click.performed -= OnClick;
        inputActions.UI.Disable();
    }

    private void OnClick(InputAction.CallbackContext context)
    {
        
    }

    public void Open(InvenSlot target)
    {

    }

    public void Close()
    {

    }









}
