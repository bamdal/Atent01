using Fusion;
using UnityEngine;
public struct NetworkInputData : INetworkInput
{

    public const byte MouseButtonLeft = 1;  // 버튼 종류들  byte사이즈 내에 아무 값이나 저장할 수 있음
    public const byte MouseButtonRight = 2;

    public NetworkButtons buttons;  // 버튼들의 입력 상황을 담음
    public Vector3 direction;

}
