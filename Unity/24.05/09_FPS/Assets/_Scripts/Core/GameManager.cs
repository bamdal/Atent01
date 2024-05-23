using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    CinemachineVirtualCamera followCamara;

    public CinemachineVirtualCamera FollowCamara => followCamara;

    Player player;

    public Player Player
    {
        get
        {
            if (player == null)
            {
                player = FindAnyObjectByType<Player>();
            }
            return player;
        }
    }

    protected override void OnInitialize()
    {
        player = FindAnyObjectByType<Player>();
        followCamara = GameObject.FindWithTag("FollowCamera").GetComponent<CinemachineVirtualCamera>();
    }


}
