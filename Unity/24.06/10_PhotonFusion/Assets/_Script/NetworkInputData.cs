using Fusion;
using UnityEngine;
public struct NetworkInputData : INetworkInput
{

    public const byte MouseButtonLeft = 1;  // ��ư ������  byte������ ���� �ƹ� ���̳� ������ �� ����
    public const byte MouseButtonRight = 2;

    public NetworkButtons buttons;  // ��ư���� �Է� ��Ȳ�� ����
    public Vector3 direction;

}
