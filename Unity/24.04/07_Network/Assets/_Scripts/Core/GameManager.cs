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

    NetPlayer player;

    public NetPlayer Player => player;

    protected override void OnInitialize()
    {
        logger = FindAnyObjectByType<Logger>();

        // 어떤 클라이언트가 접속했을 때 실행(서버에는 항상 실행, 클라이언트는 자기것만 실행)
        NetworkManager.OnClientConnectedCallback += onClientConnect;        
        NetworkManager.OnClientDisconnectCallback += onClientDisconnect;    // 어떤 클라이언트가 접속해제할때마다 실행

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
    }

    /// <summary>
    /// 어떤 클라이언트가 접속 해제했을때 처리를 하는 함수
    /// </summary>
    /// <param name="id">접속해제한 클라이언트 id</param>
    private void onClientDisconnect(ulong id)
    {
        NetworkObject netObj = NetworkManager.SpawnManager.GetPlayerNetworkObject(id);
        if (netObj.IsOwner)
        {

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
