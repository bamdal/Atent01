using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : NetworkBehaviour
{
    public float moveSpeed = 5.0f;

    NetworkCharacterController cc;

    Vector3 forward = Vector3.forward;

    [SerializeField]
    Ball prefabBall;

    [SerializeField]
    PhysicBall prefabPhysicBall;

    [Networked]
    TickTimer delay { get; set; }

    [Networked]
    public bool spawnedProjectile { get; set; }

    /// <summary>
    /// Networked로 성정된 변수의 변화를 감지하는 클래스
    /// </summary>
    private ChangeDetector changeDetector;

    /// <summary>
    /// player의 몸색 머티리얼
    /// </summary>
    public Material bodyMaterial;

    PlayerInputActions inputActions;



    TMP_Text messageText;

    private void Awake()
    {
        cc = GetComponent<NetworkCharacterController>();
        Transform child = transform.GetChild(0);
        bodyMaterial = child.GetComponent<Renderer>()?.material;
        

        inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        inputActions.Enable();
        inputActions.Player.Chat.performed += OnChat;

    }

    private void OnDisable()
    {
        inputActions.Player.Chat.performed -= OnChat;
        inputActions.Disable();
    }


    /// <summary>
    /// 네트워크 틱별로 계속실행되는 함수
    /// </summary>
    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))    // 서버쪽에서 입력 정보 받아오기 
        {
            //data.direction.Normalize();             // 유닛벡터로 만들기

            cc.Move(Runner.DeltaTime * moveSpeed * data.direction); // 초당 moveSpeed의 속도로 data.direction방향으로 이동

            if(data.direction.sqrMagnitude > 0 )
            {
                forward = data.direction;   // 회전 도중에 forward방향으로 공이 나가는 걸 방지해 상하좌우 대각선만 나가게함
            }
        }
        
        if (HasStateAuthority && delay.ExpiredOrNotRunning(Runner))  // 서버(호스트)인지 체크 && 딜레이가 설정이 되있거나 0.5f 이내
        {
            if (data.buttons.IsSet(NetworkInputData.MouseButtonLeft))   // 마우스 왼쪽 버튼이 눌렸다
            {
                delay = TickTimer.CreateFromSeconds(Runner, 0.5f);
                Runner.Spawn(prefabBall,                    // prefabBall을  
                    transform.position + forward,           // 본인 위치에 입력 방향을 더한 방향에 
                    Quaternion.LookRotation(forward),       // 입력방향기준 회전을 시키고 
                    Object.InputAuthority,                  // 생성한 플레이어에게 입력 권한을 준다
                    (runner, obj) =>                        // 스폰 직전에 실행되는 람다식
                    {
                        obj.GetComponent<Ball>().Init();
                    });
                spawnedProjectile = !spawnedProjectile;
            }

            if(data.buttons.IsSet(NetworkInputData.MouseButtonRight))
            {
                delay = TickTimer.CreateFromSeconds(Runner, 0.5f);
                Runner.Spawn(prefabPhysicBall,                    // prefabPhysicBall을  
                    transform.position + forward,           // 본인 위치에 입력 방향을 더한 방향에 
                    Quaternion.LookRotation(forward),       // 입력방향기준 회전을 시키고 
                    Object.InputAuthority,                  // 생성한 플레이어에게 입력 권한을 준다
                    (runner, obj) =>                        // 스폰 직전에 실행되는 람다식
                    {
                     obj.GetComponent<PhysicBall>().Init(moveSpeed * forward);
                     });
                spawnedProjectile = !spawnedProjectile;

            }
        }
    }

    /// <summary>
    /// Spawne함수 이후 불리는 함수
    /// </summary>
    public override void Spawned()
    {
        // 현재 실행중인 시뮬레이션에서 상태를 가져오게 설정
        changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
    }

    /// <summary>
    ///  Update와 같은 간격 FixedUpdate이후 실행 보장
    /// </summary>
    public override void Render()
    {
        // 이 네트워크 오브젝트에서 Networked로 설정된 변수에 변화가 있덨던 것들을 모두 순회
        foreach (string change in changeDetector.DetectChanges(this))
        {

            switch (change)
            {
                case nameof(spawnedProjectile): // spawnedProjectile변수가 변경되었을 때
                    bodyMaterial.color = Color.yellow;
                    break;
            }
        }
        // Render는 유니티 랜더 루프 상에서 작동 => Update와 같은 간격 FixedUpdate이후 실행 보장
        bodyMaterial.color = Color.Lerp(bodyMaterial.color, Color.blue, Time.deltaTime);

    }

    /// <summary>
    /// RPC 발동용 입력처리 함수
    /// </summary>
    /// <param name="context"></param>
    private void OnChat(InputAction.CallbackContext context)
    {
        if (Object.HasInputAuthority)   // 입력권한이 있을때 (자기 Player일때)
        {
            RPC_SendMessage("Hello World"); // Hello World라고 호스트에게 보냄
        }
    }

    /// <summary>
    /// 보내는 쪽에서 입력 권한이 있어야 하고 상태가 호스트 여야 한다
    /// </summary>
    /// <param name="message"></param>
    /// <param name="info"></param>
    /// 소스는 입력 권한이 있어야 한다 => 내 player이여야한다
    /// 타겟은 상태권한이 있어야 한다 => 타겟은 호스트다
    /// 호스트 모드 = SourceIsHostPlayer => 플레이어 입장에서 RPC를 호출한다
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
    public void RPC_SendMessage(string message, RpcInfo info = default)
    {
        RPC_RelayMessage(message, info.Source); // info.Source는 로컬 플레이어(자기 자신의 PlayerRef)
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    /// <param name="messageSource"></param>
    /// 소스틑 상태권한이 있어야 한다 => 소스가 호스트
    /// RpcTargets.All => 받은 내용을 모두에게 전파하게 설정
    /// HostMode = RpcHostMode.SourceIsServer => 서버입장에서 RPC를 보내겠다 
    [Rpc(RpcSources.StateAuthority,RpcTargets.All , HostMode = RpcHostMode.SourceIsServer)]
    public void RPC_RelayMessage(string message, PlayerRef messageSource)
    {
        if (messageText == null)
        {
            messageText = FindAnyObjectByType<TMP_Text>();
        }

        if (messageSource == Runner.LocalPlayer)
        {
            // 서버가 내가 보낸 메세지를 나에게 보낸 경우 (내가 보낸 메세지를 받은 경우)
            message = $"You : {message}\n";
        }
        else
        {
            // 다른 사람이 보낸 메세지를 받은 경우
            message = $"Other : {message}\n";
        }

        messageText.text += message;
    }
}
