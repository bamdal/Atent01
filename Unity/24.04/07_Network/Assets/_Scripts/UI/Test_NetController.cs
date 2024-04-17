using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class Test_NetController : MonoBehaviour
{
    // Start is called before the first frame update
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
    }


}
