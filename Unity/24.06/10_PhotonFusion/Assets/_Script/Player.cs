using Fusion;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class Player : NetworkBehaviour
{
    public float moveSpeed = 5.0f;

    NetworkCharacterController cc;

    Vector3 forward = Vector3.forward;

    private void Awake()
    {
        cc = GetComponent<NetworkCharacterController>();
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
    }
}
