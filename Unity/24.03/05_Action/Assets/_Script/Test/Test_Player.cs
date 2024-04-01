using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XInput;

public class Test_Player : MonoBehaviour
{
    // Start is called before the first frame update

    /// �Էµ� �̵� ����
    /// </summary>
    Vector3 inputDirection = Vector3.zero;  // y�� ������ �ٴ� ����

    /// <summary>
    /// ĳ������ ��ǥ�������� ȸ����Ű�� ȸ��
    /// </summary>
    Quaternion targetRotation = Quaternion.identity;

    /// <summary>
    /// ĳ���� ȸ�� �ӵ�
    /// </summary>
    public float turnSpeed = 10.0f;
    PlayerInputController inputController;

    private void Awake()
    {
        inputController = GetComponent<PlayerInputController>();
        inputController.onMove += OnMoveInput;
    }
    /// <summary>
    /// �̵� �Է¿� ���� ��������Ʈ�� �޴� �Լ�
    /// </summary>
    /// <param name="input">�Էµ� �̵� ����</param>
    /// <param name="move">�޸��� �ִ��� �Ǵ��ϴ� �Լ�(true�� Ű �Է���)</param>
    private void OnMoveInput(Vector2 input, bool move)
    {

        inputDirection.x = input.x;     // �Է� ���� ����
        inputDirection.y = 0;
        inputDirection.z = input.y;

        if (move)
        {
            // ī�޶��� y ȸ���� ���� ����
            Quaternion camY = Quaternion.Euler(0, Camera.main.transform.rotation.eulerAngles.y, 0);
            inputDirection = camY * inputDirection; // �Է¹����� ī�޶��� yȸ���� ���� ������ ȸ�� ��Ű��
            targetRotation = Quaternion.LookRotation(inputDirection);   // ��ǥȸ�� ����
        }
    }

    private void Update()
    {
        transform.Translate( Time.deltaTime * 5.0f * inputDirection);
    }

}
