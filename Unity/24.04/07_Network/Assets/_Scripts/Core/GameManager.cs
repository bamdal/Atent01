using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetSingleton<GameManager>
{

    /// <summary>
    /// 로거(텍스트 출력용)
    /// </summary>
    Logger logger;

    /// <summary>
    /// 내 플레이어(접속 안했으면 null)
    /// </summary>
    NetPlayer player;

    /// <summary>
    /// 현재 동접자수
    /// </summary>
    NetworkVariable<int> playersInGame = new NetworkVariable<int>(0);

    /// <summary>
    /// 동접자수가 변경되었음을 알리는 델리게이트
    /// </summary>
    public Action<int> onPlayersInGameChange;

    public NetPlayer Player => player;

    /// <summary>
    /// 현재 사용자의 이름
    /// </summary>
    string userName = "디폴트";
    public string UserName
    {
        get => userName;
        set
        {
            userName = value;
            onUserNameChange?.Invoke(userName);
        }
    }

    public Action<string> onUserNameChange;

    /// <summary>
    /// 현재 사용자의 색상
    /// </summary>
    Color userColor = Color.clear;
    public Color UserColor
    {
        get => userColor;
        set
        {
            userColor = value;
            onUserColorChange?.Invoke(userColor);
        }
    }

    public Action<Color> onUserColorChange;

    NetPlayerDecorator deco;

    protected override void OnInitialize()
    {
        logger = FindAnyObjectByType<Logger>();
        // 어떤 클라이언트가 접속했을 때 실행(서버에는 항상 실행, 클라이언트는 자기것만 실행)
        NetworkManager.OnClientConnectedCallback += onClientConnect;        
        NetworkManager.OnClientDisconnectCallback += onClientDisconnect;    // 어떤 클라이언트가 접속해제할때마다 실행

        playersInGame.OnValueChanged += (_,newValue) => { onPlayersInGameChange?.Invoke(newValue); };
    }

    /// <summary>
    /// 어떤 클라이언트가 접속했을 때 처리를 하는 함수
    /// </summary>
    /// <param name="id">접속한 클라이언트 id</param>
    private void onClientConnect(ulong id)
    {
        NetworkObject netObj = NetworkManager.SpawnManager.GetPlayerNetworkObject(id);
        if (netObj.IsOwner)
        {
            player = netObj.GetComponent<NetPlayer>();
            player.gameObject.name = $"Player_{id}";
            deco = netObj.GetComponent<NetPlayerDecorator>();
            deco.SetName(UserName);

            foreach (var other in NetworkManager.SpawnManager.SpawnedObjectsList)
            {
                NetPlayer otherPlayer = other.GetComponent<NetPlayer>();
                if (otherPlayer != null && otherPlayer != player)
                {
                    
                    otherPlayer.gameObject.name = $"OtherPlayer_{other.OwnerClientId}";
                }
            }
        }
        else
        {
            NetPlayer other = netObj.GetComponent<NetPlayer>();
            if(other != null && other != player)
            {
                netObj.gameObject.name = $"OtherPlayer_{id}";
            }
        }
        if (IsServer)
        {
            playersInGame.Value++;
        }
  
    }

    /// <summary>
    /// 어떤 클라이언트가 접속 해제했을때 처리를 하는 함수
    /// </summary>
    /// <param name="id">접속해제한 클라이언트 id</param>
    private void onClientDisconnect(ulong id)
    {

        NetworkObject netObj = NetworkManager.SpawnManager.GetPlayerNetworkObject(id);
        if(netObj.IsOwner)
        {
            player = null;
        }
        if (IsServer)
        {
            playersInGame.Value--;
        }
    }


    /// <summary>
    /// 로거에 문자열을 추가하는 함수
    /// </summary>
    /// <param name="message"></param>
    public void Log(string message)
    {
        logger.Log(message);
    }
}
