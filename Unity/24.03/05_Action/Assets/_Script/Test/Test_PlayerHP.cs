using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test_PlayerHP : TestBase
{
    Player player;
    public float data = 10.0f;
    public float interval = 0.5f;
    public float duration = 1.0f;
    public uint count = 4;
    private void Start()
    {
        player = GameManager.Instance.Player;
    }

    protected override void OnTest1(InputAction.CallbackContext context)
    {
        // 플레이어 HP 증가
        player.HP += data;
        player.MP += data;
    
    }

    protected override void OnTest2(InputAction.CallbackContext context)
    {
        // 플레이어 HP 감소
        player.HP -= data;
        player.MP -= data;
    }

    protected override void OnTest3(InputAction.CallbackContext context)
    {
        // 플레이어 HP 재생
        player.HealthRegenerate(data, duration);
        player.ManaRegenerate(data, duration);
    }

    protected override void OnTest4(InputAction.CallbackContext context)
    {
        // 플레이어 HP 틱당 재생
        player.HealthRegenerateByTick(5, interval, count);
        player.ManaRegenerateByTick(5, interval, count);
    }
    protected override void OnTest5(InputAction.CallbackContext context)
    {
        Factory.Instance.MakeItem(ItemCode.ManaPotion);
    }
}

// IMana 인터페이스 
// IHealth, IMana 사용하는 플레이어용 UI
// 화면 왼쪽 위에 배치
// 플레이어의 HP와 MP가 변경되면 UI도 같이 변경