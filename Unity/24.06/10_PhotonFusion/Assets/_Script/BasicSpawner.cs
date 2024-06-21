using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class BasicSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    /// <summary>
    /// 이 오브젝트가 만드는 네트워크 러너
    /// </summary>
    private NetworkRunner myRunner = null;

    /// <summary>
    /// 플레이어 오브젝트 프리펩
    /// </summary>
    [SerializeField]
    private NetworkPrefabRef playerPrefab;


    /// <summary>
    /// 접속자 정보 딕셔너리
    /// </summary>
    private Dictionary<PlayerRef, NetworkObject> spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();

    /// <summary>
    /// 인풋 시스템
    /// </summary>
    PlayerInputActions inputActions;

    /// <summary>
    /// 플레이어 이동 방향
    /// </summary>
    Vector3 inputDirection = Vector3.zero;

    bool isShootPress = false;
    
    private void Awake()
    {
        inputActions = new PlayerInputActions();
    }

    /// <summary>
    /// 게임 세션을 열거나 접속하는 함수
    /// </summary>
    /// <param name="mode">게임에 접속하는 방식(Host or Client)</param>
    async void StartGame(GameMode mode) // async : 비동기 메서드임을 알림(내부에 await가 있음)
    {
        myRunner = this.gameObject.AddComponent<NetworkRunner>(); // 네트워크 러너 컴포넌트 추가
        myRunner.ProvideInput = true;                             // 유저 입력을 제공할 것이라고 설정

        SceneRef scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
        NetworkSceneInfo sceneInfo = new NetworkSceneInfo();
        if (scene.IsValid)
        {
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
        }

        await myRunner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = "TestRoom",
            Scene = scene,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
        InputEnable();
    }

    private void OnDisable()
    {
        InputDisable(); 
    }

    void InputEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnMove;
        inputActions.Player.Shoot.performed += OnShootPress;
        inputActions.Player.Shoot.canceled += OnShootRelease;
    }



    void InputDisable()
    {
        inputActions.Player.Shoot.canceled -= OnShootRelease;
        inputActions.Player.Shoot.performed -= OnShootPress;
        inputActions.Player.Move.canceled-= OnMove;
        inputActions.Player.Move.performed -= OnMove;
        inputActions.Player.Disable();
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        Vector2 read = context.ReadValue<Vector2>();
        inputDirection.Set(read.x, 0, read.y);

    }

    private void OnShootPress(InputAction.CallbackContext context)
    {
        isShootPress = true;
    }

    private void OnShootRelease(InputAction.CallbackContext context)
    {
        isShootPress = false;
    }


    /// <summary>
    /// GUI를 그리는 함수
    /// </summary>
    void OnGUI()
    {
        if(myRunner == null)
        {
            if (GUI.Button(new Rect(0, 0, 200, 40), "Host"))
            {
                StartGame(GameMode.Host);
            }

            if (GUI.Button(new Rect(0, 50, 200, 40), "Client"))
            {
                StartGame(GameMode.Client);
            }
        }

    }

    /// <summary>
    /// 플레이어가 접속했을 때 실행되는 함수
    /// </summary>
    /// <param name="runner">자기 자신의 러너</param>
    /// <param name="player">접속한 플레이어</param>
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) 
    {
        if (runner.IsServer)    // 서버에서만 실행
        {
            // 스폰 될 위치 구하기
            Vector3 spawnPosition = new Vector3(player.RawEncoded % runner.Config.Simulation.PlayerCount,0,0);

            // 플레이어 오브젝트 생성(네번째 파라메터(player) : 이 오브젝트에 입력을 줄 수 있는 플레이어에 대한 참조(Owner 같은 느낌))
            NetworkObject netPlayer = runner.Spawn(playerPrefab, spawnPosition,Quaternion.identity,player);
            
            // 
            spawnedCharacters.Add(player, netPlayer);
        }
    }

    /// <summary>
    /// 플레이어가 접속을 해제했을때 실행되는 함수
    /// </summary>
    /// <param name="runner">자기 자신의 러너</param>
    /// <param name="player">접속한 플레이어</param>
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) 
    {
        if (spawnedCharacters.TryGetValue(player, out NetworkObject networkObject)) //spawnedCharacters에 player가 있으면 실행
        {
            runner.Despawn(networkObject);      // runner에서 디스폰(게임 오브젝트 삭제도 함께 처리)
            spawnedCharacters.Remove(player);   // 딕셔너리에서 제거
        }
    }

    /// <summary>
    /// 사용자의 입력데이터를 수집하는 함수
    /// </summary>
    /// <param name="runner">네트워크 러너</param>
    /// <param name="input">데이터를 받아갈 변수</param>
    public void OnInput(NetworkRunner runner, NetworkInput input) 
    {
        NetworkInputData data = new NetworkInputData(); // 우리가 만든 데이터 타입을 new하기

        /*        if (Input.GetKey(KeyCode.W))    // 이 순간 W키가 눌려져 있는지 확인 (결과가 true면 W키가 눌린 상태다)
                {
                    data.direction += Vector3.forward;
                }

                if (Input.GetKey(KeyCode.A))
                {
                    data.direction += Vector3.left;
                }

                if (Input.GetKey(KeyCode.S))
                {
                    data.direction += Vector3.back;
                }

                if (Input.GetKey(KeyCode.D))
                {
                    data.direction += Vector3.right;
                }
        */

        /*        if(Keyboard.current.wKey.isPressed)
                {
                    data.direction += Vector3.forward;
                }

                if (Keyboard.current.aKey.isPressed)
                {
                    data.direction += Vector3.left;
                }

                if (Keyboard.current.sKey.isPressed)
                {
                    data.direction += Vector3.back;
                }

                if (Keyboard.current.dKey.isPressed)
                {
                    data.direction += Vector3.right;
                }
        */

        data.direction = inputDirection;
        data.buttons.Set(NetworkInputData.MouseButtonLeft, isShootPress);


        input.Set(data);    // 결정된 입력을 서버쪽으로 전달

    }


    public void OnConnectedToServer(NetworkRunner runner){ }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason){ }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token){ }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data){ }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason){ }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken){ }


    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input){ }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player){ }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player){ }



    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress){ }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data){ }

    public void OnSceneLoadDone(NetworkRunner runner){ }

    public void OnSceneLoadStart(NetworkRunner runner){ }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList){ }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason){ }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message){ }
}
