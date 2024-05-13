using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomDeploymentButton : MonoBehaviour
{
    Button finishButton;

    private void Awake()
    {
        finishButton = GetComponent<Button>();
        finishButton.onClick.AddListener(() => { OnClick(); });
    }

    private void OnClick()
    {
        UserPlayer player = GameManager.Instance.UserPlayer;
        Ship[] ships = player.Ships;
        if (player.IsAllDeployed)
        {
            player.UndoAllShipDeployment(); // 만약 함선이 전부 배치되어있다면 전부 취소해서 다시 배치
        }

        player.AutoShipDeployment(true);
    }
}
