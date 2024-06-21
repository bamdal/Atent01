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
    }
}
