using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XInput;

public class Test_Player : MonoBehaviour
{
    // Start is called before the first frame update

    /// 입력된 이동 방향
    /// </summary>
    Vector3 inputDirection = Vector3.zero;  // y는 무조건 바닥 높이

    /// <summary>
    /// 캐릭터의 목표방향으로 회전시키는 회전
    /// </summary>
    Quaternion targetRotation = Quaternion.identity;

    /// <summary>
    /// 캐릭터 회전 속도
    /// </summary>
    public float turnSpeed = 10.0f;
    PlayerInputController inputController;

    private void Awake()
    {
        inputController = GetComponent<PlayerInputController>();
        inputController.onMove += OnMoveInput;
    }
    /// <summary>
    /// 이동 입력에 대해 델리게이트로 받는 함수
    /// </summary>
    /// <param name="input">입력된 이동 방향</param>
    /// <param name="move">달리고 있는지 판단하는 함수(true면 키 입력중)</param>
    private void OnMoveInput(Vector2 input, bool move)
    {

        inputDirection.x = input.x;     // 입력 방향 저장
        inputDirection.y = 0;
        inputDirection.z = input.y;

        if (move)
        {
            // 카메라의 y 회전만 따로 추출
            Quaternion camY = Quaternion.Euler(0, Camera.main.transform.rotation.eulerAngles.y, 0);
            inputDirection = camY * inputDirection; // 입력방향을 카메라의 y회전과 같은 정도로 회전 시키기
            targetRotation = Quaternion.LookRotation(inputDirection);   // 목표회전 저장
        }
    }

    private void Update()
    {
        transform.Translate( Time.deltaTime * 5.0f * inputDirection);
    }

}
