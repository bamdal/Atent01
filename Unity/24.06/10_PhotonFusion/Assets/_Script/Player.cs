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
    /// Networked�� ������ ������ ��ȭ�� �����ϴ� Ŭ����
    /// </summary>
    private ChangeDetector changeDetector;

    /// <summary>
    /// player�� ���� ��Ƽ����
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
    /// ��Ʈ��ũ ƽ���� ��ӽ���Ǵ� �Լ�
    /// </summary>
    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))    // �����ʿ��� �Է� ���� �޾ƿ��� 
        {
            //data.direction.Normalize();             // ���ֺ��ͷ� �����

            cc.Move(Runner.DeltaTime * moveSpeed * data.direction); // �ʴ� moveSpeed�� �ӵ��� data.direction�������� �̵�

            if(data.direction.sqrMagnitude > 0 )
            {
                forward = data.direction;   // ȸ�� ���߿� forward�������� ���� ������ �� ������ �����¿� �밢���� ��������
            }
        }
        
        if (HasStateAuthority && delay.ExpiredOrNotRunning(Runner))  // ����(ȣ��Ʈ)���� üũ && �����̰� ������ ���ְų� 0.5f �̳�
        {
            if (data.buttons.IsSet(NetworkInputData.MouseButtonLeft))   // ���콺 ���� ��ư�� ���ȴ�
            {
                delay = TickTimer.CreateFromSeconds(Runner, 0.5f);
                Runner.Spawn(prefabBall,                    // prefabBall��  
                    transform.position + forward,           // ���� ��ġ�� �Է� ������ ���� ���⿡ 
                    Quaternion.LookRotation(forward),       // �Է¹������ ȸ���� ��Ű�� 
                    Object.InputAuthority,                  // ������ �÷��̾�� �Է� ������ �ش�
                    (runner, obj) =>                        // ���� ������ ����Ǵ� ���ٽ�
                    {
                        obj.GetComponent<Ball>().Init();
                    });
                spawnedProjectile = !spawnedProjectile;
            }

            if(data.buttons.IsSet(NetworkInputData.MouseButtonRight))
            {
                delay = TickTimer.CreateFromSeconds(Runner, 0.5f);
                Runner.Spawn(prefabPhysicBall,                    // prefabPhysicBall��  
                    transform.position + forward,           // ���� ��ġ�� �Է� ������ ���� ���⿡ 
                    Quaternion.LookRotation(forward),       // �Է¹������ ȸ���� ��Ű�� 
                    Object.InputAuthority,                  // ������ �÷��̾�� �Է� ������ �ش�
                    (runner, obj) =>                        // ���� ������ ����Ǵ� ���ٽ�
                    {
                     obj.GetComponent<PhysicBall>().Init(moveSpeed * forward);
                     });
                spawnedProjectile = !spawnedProjectile;

            }
        }
    }

    /// <summary>
    /// Spawne�Լ� ���� �Ҹ��� �Լ�
    /// </summary>
    public override void Spawned()
    {
        // ���� �������� �ùķ��̼ǿ��� ���¸� �������� ����
        changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
    }

    /// <summary>
    ///  Update�� ���� ���� FixedUpdate���� ���� ����
    /// </summary>
    public override void Render()
    {
        // �� ��Ʈ��ũ ������Ʈ���� Networked�� ������ ������ ��ȭ�� �։B�� �͵��� ��� ��ȸ
        foreach (string change in changeDetector.DetectChanges(this))
        {

            switch (change)
            {
                case nameof(spawnedProjectile): // spawnedProjectile������ ����Ǿ��� ��
                    bodyMaterial.color = Color.yellow;
                    break;
            }
        }
        // Render�� ����Ƽ ���� ���� �󿡼� �۵� => Update�� ���� ���� FixedUpdate���� ���� ����
        bodyMaterial.color = Color.Lerp(bodyMaterial.color, Color.blue, Time.deltaTime);

    }

    /// <summary>
    /// RPC �ߵ��� �Է�ó�� �Լ�
    /// </summary>
    /// <param name="context"></param>
    private void OnChat(InputAction.CallbackContext context)
    {
        if (Object.HasInputAuthority)   // �Է±����� ������ (�ڱ� Player�϶�)
        {
            RPC_SendMessage("Hello World"); // Hello World��� ȣ��Ʈ���� ����
        }
    }

    /// <summary>
    /// ������ �ʿ��� �Է� ������ �־�� �ϰ� ���°� ȣ��Ʈ ���� �Ѵ�
    /// </summary>
    /// <param name="message"></param>
    /// <param name="info"></param>
    /// �ҽ��� �Է� ������ �־�� �Ѵ� => �� player�̿����Ѵ�
    /// Ÿ���� ���±����� �־�� �Ѵ� => Ÿ���� ȣ��Ʈ��
    /// ȣ��Ʈ ��� = SourceIsHostPlayer => �÷��̾� ���忡�� RPC�� ȣ���Ѵ�
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
    public void RPC_SendMessage(string message, RpcInfo info = default)
    {
        RPC_RelayMessage(message, info.Source); // info.Source�� ���� �÷��̾�(�ڱ� �ڽ��� PlayerRef)
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    /// <param name="messageSource"></param>
    /// �ҽ��z ���±����� �־�� �Ѵ� => �ҽ��� ȣ��Ʈ
    /// RpcTargets.All => ���� ������ ��ο��� �����ϰ� ����
    /// HostMode = RpcHostMode.SourceIsServer => �������忡�� RPC�� �����ڴ� 
    [Rpc(RpcSources.StateAuthority,RpcTargets.All , HostMode = RpcHostMode.SourceIsServer)]
    public void RPC_RelayMessage(string message, PlayerRef messageSource)
    {
        if (messageText == null)
        {
            messageText = FindAnyObjectByType<TMP_Text>();
        }

        if (messageSource == Runner.LocalPlayer)
        {
            // ������ ���� ���� �޼����� ������ ���� ��� (���� ���� �޼����� ���� ���)
            message = $"You : {message}\n";
        }
        else
        {
            // �ٸ� ����� ���� �޼����� ���� ���
            message = $"Other : {message}\n";
        }

        messageText.text += message;
    }
}
