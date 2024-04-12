using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class RankPanel : MonoBehaviour
{
    /// <summary>
    /// 이 패널이 가진 모든 탭
    /// </summary>
    Tab[] tabs;

    /// <summary>
    /// 현재 선택한 탭
    /// </summary>
    Tab selectedTab;

    Tab SelectedTab
    {
        get => selectedTab;
        set
        {
            if (value != selectedTab)
            {
                selectedTab.IsSelected = false; // 이전것은 선택 해제
                selectedTab = value;
                selectedTab.IsSelected = true;  // 새것을 선택
            }
        }
    }

    /// <summary>
    /// 이 패널이 사용하는 토글 버튼(탭의 서브패널 열고 닫기용)
    /// </summary>
    ToggleButton toggle;

    CanvasGroup canvasGroup;


    private void Awake()
    {
        tabs = GetComponentsInChildren<Tab>();
        foreach (Tab tab in tabs)
        {
            tab.OnTabSelected += (newSelected) => 
            { 
                SelectedTab = newSelected;
                toggle.ToggleOn();
            };
        }

        selectedTab = tabs[tabs.Length-1];      // 처음 열릴때의 문제 방지용 설정 초기화

        toggle = GetComponentInChildren<ToggleButton>();
        toggle.onToggleChange += (isOn) =>
        {
            if(isOn)
            {
                SelectedTab.SubPanelOpen();
            }
            else
            {
                selectedTab.SubPanelClose();
            }
        };
        canvasGroup = GetComponent<CanvasGroup>();
    }
    private void Start()
    {
        GameManager gameManager = GameManager.Instance;
        gameManager.onGameClear += Open;                // 게임 Clear시 열기
        gameManager.onGameReady += Close;               // 게임 Close시 닫기

        Close();
    }

    /// <summary>
    /// 랭크 패널을 여는 함수
    /// </summary>
    void Open()
    {
        // 게임이 Clear시 실행
        SelectedTab = tabs[0];
        canvasGroup.alpha = 1.0f;
        canvasGroup.interactable = true;
    }

    /// <summary>
    /// 랭크 패널을 닫는 함수
    /// </summary>
    void Close()
    {
        // 게임이 Ready가 되면 항상 실행
        canvasGroup.alpha = 0.0f;
        canvasGroup.interactable = false;
    }
}
