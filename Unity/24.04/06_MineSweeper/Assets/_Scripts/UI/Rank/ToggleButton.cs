using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleButton : MonoBehaviour
{
    /// <summary>
    /// 토글 버튼이 켜진 상태일 때 보일 이미지
    /// </summary>
    public Sprite onSprite;

    /// <summary>
    /// 토글 버튼이 닫힌 상태일 때 보일 이미지
    /// </summary>
    public Sprite offSprite;

    /// <summary>
    /// 토글 버튼이 눌려질 때마다 알리는 델리게이트(true면 on, false면 off)
    /// </summary>
    public Action<bool> onToggleChange;

    /// <summary>
    /// 현재 토글의 on/off 상태 ( true면 on, false면 off)
    /// </summary>
    bool isOn = false;

    // UI컴포넌트들
    Button toggle;
    Image buttonImage;

    private void Awake()
    {
        toggle = GetComponent<Button>();
        toggle.onClick.AddListener(() => SetToggleState(!isOn));

        buttonImage = GetComponent<Image>();
    }

    private void Start()
    {
        isOn = true;
        SetToggleState(isOn);
    }

    /// <summary>
    /// 토글 상태를 변경하는 함수
    /// </summary>
    /// <param name="on">새 토글 상태, true면 on, false면 off</param>
    public void SetToggleState(bool on)
    {
        isOn= on;
        if (isOn)
        {
            buttonImage.sprite = onSprite;
        }
        else
        {
            buttonImage.sprite= offSprite;
        }
        onToggleChange?.Invoke(isOn);
    }

    /// <summary>
    /// 토글을 켜는 함수
    /// </summary>
    public void ToggleOn()
    {
        // 외부에서 사용도기 위한 목적(탭이 선택되었을 때 서브패널은 항상 열리기 때문에)
        SetToggleState(true);
    }

}