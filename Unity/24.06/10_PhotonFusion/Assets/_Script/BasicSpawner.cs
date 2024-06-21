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
    /// �� ������Ʈ�� ����� ��Ʈ��ũ ����
    /// </summary>
    private NetworkRunner myRunner = null;

    /// <summary>
    /// �÷��̾� ������Ʈ ������
    /// </summary>
    [SerializeField]
    private NetworkPrefabRef playerPrefab;


    /// <summary>
    /// ������ ���� ��ųʸ�
    /// </summary>
    private Dictionary<PlayerRef, NetworkObject> spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();

    /// <summary>
    /// ��ǲ �ý���
    /// </summary>
    PlayerInputActions inputActions;

    /// <summary>
    /// �÷��̾� �̵� ����
    /// </summary>
    Vector3 inputDirection = Vector3.zero;

    bool isShootPress = false;
    
    private void Awake()
    {
        inputActions = new PlayerInputActions();
    }

    /// <summary>
    /// ���� ������ ���ų� �����ϴ� �Լ�
    /// </summary>
    /// <param name="mode">���ӿ� �����ϴ� ���(Host or Client)</param>
    async void StartGame(GameMode mode) // async : �񵿱� �޼������� �˸�(���ο� await�� ����)
    {
        myRunner = this.gameObject.AddComponent<NetworkRunner>(); // ��Ʈ��ũ ���� ������Ʈ �߰�
        myRunner.ProvideInput = true;                             // ���� �Է��� ������ ���̶�� ����

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
    /// GUI�� �׸��� �Լ�
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
    /// �÷��̾ �������� �� ����Ǵ� �Լ�
    /// </summary>
    /// <param name="runner">�ڱ� �ڽ��� ����</param>
    /// <param name="player">������ �÷��̾�</param>
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) 
    {
        if (runner.IsServer)    // ���������� ����
        {
            // ���� �� ��ġ ���ϱ�
            Vector3 spawnPosition = new Vector3(player.RawEncoded % runner.Config.Simulation.PlayerCount,0,0);

            // �÷��̾� ������Ʈ ����(�׹�° �Ķ����(player) : �� ������Ʈ�� �Է��� �� �� �ִ� �÷��̾ ���� ����(Owner ���� ����))
            NetworkObject netPlayer = runner.Spawn(playerPrefab, spawnPosition,Quaternion.identity,player);
            
            // 
            spawnedCharacters.Add(player, netPlayer);
        }
    }

    /// <summary>
    /// �÷��̾ ������ ���������� ����Ǵ� �Լ�
    /// </summary>
    /// <param name="runner">�ڱ� �ڽ��� ����</param>
    /// <param name="player">������ �÷��̾�</param>
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) 
    {
        if (spawnedCharacters.TryGetValue(player, out NetworkObject networkObject)) //spawnedCharacters�� player�� ������ ����
        {
            runner.Despawn(networkObject);      // runner���� ����(���� ������Ʈ ������ �Բ� ó��)
            spawnedCharacters.Remove(player);   // ��ųʸ����� ����
        }
    }

    /// <summary>
    /// ������� �Էµ����͸� �����ϴ� �Լ�
    /// </summary>
    /// <param name="runner">��Ʈ��ũ ����</param>
    /// <param name="input">�����͸� �޾ư� ����</param>
    public void OnInput(NetworkRunner runner, NetworkInput input) 
    {
        NetworkInputData data = new NetworkInputData(); // �츮�� ���� ������ Ÿ���� new�ϱ�

        /*        if (Input.GetKey(KeyCode.W))    // �� ���� WŰ�� ������ �ִ��� Ȯ�� (����� true�� WŰ�� ���� ���´�)
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


        input.Set(data);    // ������ �Է��� ���������� ����

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
