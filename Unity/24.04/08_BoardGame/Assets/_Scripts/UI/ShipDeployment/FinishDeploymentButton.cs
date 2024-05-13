using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FinishDeploymentButton : MonoBehaviour
{
    Button button;
    UserPlayer player;

    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);

        player = GameManager.Instance.UserPlayer;
        foreach(var ship in player.Ships)
        {
            ship.onDeploy += OnShipDeployed;
        }
    }

    /// <summary>
    /// 함선의 배치 정보가 변경될 때마다 실행되는 함수
    /// </summary>
    /// <param name="isDeployed">true면 지금 함선이 배치됨 false면 지금 배치 취소됨</param>
    private void OnShipDeployed(bool isDeployed)
    {
        if(isDeployed && player.IsAllDeployed)
        {
            button.interactable = true; // 지금 배치되었고 모든 함선이 배치된 상황이면 버튼활성화
        }
        else
        {
            button.interactable= false; // 지금 함선 배치 취소되었거나 모든 함선이 배치되지 않음
        }
    }

    private void OnClick()
    {
        throw new NotImplementedException();
    }

 
}
