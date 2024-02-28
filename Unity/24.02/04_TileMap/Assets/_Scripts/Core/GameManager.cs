using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    Player player;

    public Player Player
    {
        get
        {
            if(player == null)
            {
                OnInitialize();
            }
            return player;
        }
    }

    WolrdManager wolrdManager;

    public WolrdManager Wolrd => wolrdManager;

    protected override void OnPreInitialize()
    {
        base.OnPreInitialize();
        wolrdManager = GetComponent<WolrdManager>();
    }

    protected override void OnInitialize()
    {
        base.OnInitialize();
        player = FindAnyObjectByType<Player>();
    }
}
