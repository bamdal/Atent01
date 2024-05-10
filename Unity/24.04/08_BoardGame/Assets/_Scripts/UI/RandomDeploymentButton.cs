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
        PlayerBase player = GameManager.Instance.UserPlayer;
        Ship[] ships = player.Ships;
        foreach (Ship ship in ships)
        {
            if (!ship.IsDeployed)
            {
                player.AutoShipDeployment(true);
                break;
            }
        }
    }
}
