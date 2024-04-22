using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class Test_NetController : MonoBehaviour
{
    // Start is called before the first frame update
    TextMeshProUGUI playerInGame;
    TextMeshProUGUI userName;

    const string BlankUserName = "□□□□□□□□";
    const string BlankPlayerInGame ="-";
    void Start()
    {
        Transform child = transform.GetChild(0);
        Button startHost = child.GetComponent<Button>();
        startHost.onClick.AddListener(() => 
        {
            if (NetworkManager.Singleton.StartHost())   // 호스트로 시작 시도
            {
                Debug.Log("호스트로 시작");

            }
            else
            {
                Debug.Log("호스트로 시작 실패");
            }
        });

        child = transform.GetChild(1);
        Button startClient = child.GetComponent<Button>();
        startClient.onClick.AddListener(() =>
        {
            if (NetworkManager.Singleton.StartClient())   // 클라이언트로 시작 시도
            {
                Debug.Log("클라이언트로 연결");

            }
            else
            {
                Debug.Log("클라이언트로 연결 실패");
            }
        });

        child = transform.GetChild(2);
        Button disconnect = child.GetComponent<Button>();
        disconnect.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.Shutdown();    // 내연결 끊기
        });

        child = transform.GetChild(3);
        child = child.GetChild(1);
        playerInGame = child.GetComponent<TextMeshProUGUI>();
        GameManager gameManager = GameManager.Instance;
        gameManager.onPlayersInGameChange += (count) => { playerInGame.text = count.ToString(); };  // 동접자수 변경되었으면 UI업데이트

        child = transform.GetChild(4);
        child = child.GetChild(1);
        userName = child.GetComponent<TextMeshProUGUI>();
        gameManager.onUserNameChange += (text) => { userName.text = text;  };

        // 플레이어 연결 종료시 
        gameManager.onPlayerDisconnected += () =>
        {
            userName.text = BlankUserName;
            playerInGame.text = BlankPlayerInGame;
        };
    }

}
