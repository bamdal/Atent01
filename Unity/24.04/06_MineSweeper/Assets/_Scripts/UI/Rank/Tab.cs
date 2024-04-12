using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tab : MonoBehaviour
{
    // 이 탭이 선택되었을 RankPanel에게 선택되면 서브 패널을 연다
    // 선택이 해제되면 서브패널을 닫는다

    /// <summary>
    /// 선택되지 않았을 때의 색상
    /// </summary>
    readonly Color UnselectedColor = new Color(0.44f, 0.44f, 0.44f);

    /// <summary>
    /// 선택되었을때의 색상
    /// </summary>
    readonly Color SelectedColor = Color.white;

    bool isSelected = false;

    public bool IsSelected
    {
        get => isSelected;
        set
        {
            // 선택되면 버튼의 색상변경 선택 안되면 반투명
            // 선택되면 서브 패널 열고 선택 안되면 서브패털을 닫는다
            if(isSelected != value)
            {
                isSelected = value;
                if(isSelected)
                {
                    SubPanelOpen(); // 선택된 경우
                    OnTabSelected?.Invoke(this);    // 선택됨을 알림
                }
                else
                {
                    SubPanelClose();    // 선택되지 않은 경우
                }
            }
        }
    }
    
    /// <summary>
    /// 선택됨을 알리는 델리게이트
    /// </summary>
    public Action<Tab> OnTabSelected;

    // 컴포넌트들
    Image tabImage;

    TabSubPanel subPanel;

    public CanvasGroup canvasGroup;

    private void Awake()
    {
        tabImage = GetComponent<Image>();
        Button button = GetComponent<Button>();
        button.onClick.AddListener(() =>
        {
            IsSelected = true;  // 버튼이 눌려지면 IsSelected를 true로 한다
        });
        subPanel = GetComponentInChildren<TabSubPanel>(true);
        canvasGroup = GetComponent<CanvasGroup>();
    }

    /// <summary>
    /// 서브패널을 여는 함수
    /// </summary>
    public void SubPanelOpen()
    {
        tabImage.color = SelectedColor;
        subPanel.gameObject.SetActive(true);
    }

    /// <summary>
    /// 서브패너를 닫는 함수
    /// </summary>
    public void SubPanelClose()
    {
        tabImage.color = UnselectedColor;
        subPanel.gameObject.SetActive(false);

    }
}
