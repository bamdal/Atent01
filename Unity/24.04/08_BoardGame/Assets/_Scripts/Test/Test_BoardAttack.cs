using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test_BoardAttack : TestAutoShipDeployment
{
    public GameObject firePrefab;
    FireSetter fireSetter;
    protected override void Start()
    {
        base.Start();
        fireSetter = FindAnyObjectByType<FireSetter>();


        GetShip(ShipType.Carrier).onHit += (_) => GameManager.Instance.CameraShake(0.1f);
        GetShip(ShipType.BattleShip).onHit += (_) => GameManager.Instance.CameraShake(0.1f);
        GetShip(ShipType.Destroyer).onHit += (_) => GameManager.Instance.CameraShake(0.1f);
        GetShip(ShipType.Submarine).onHit += (_) => GameManager.Instance.CameraShake(0.1f);
        GetShip(ShipType.PatrolBoat).onHit += (_) => GameManager.Instance.CameraShake(0.1f);

        GetShip(ShipType.Carrier).onSink += (ship) =>
        {
            OnSink(ship);
        };
        GetShip(ShipType.BattleShip).onSink += (ship) =>
        {
            OnSink(ship);
        };
        GetShip(ShipType.Destroyer).onSink += (ship) =>
        {
            OnSink(ship);
        };
        GetShip(ShipType.Submarine).onSink += (ship) =>
        {

            OnSink(ship);
        };
        GetShip(ShipType.PatrolBoat).onSink += (ship) =>
        {
            OnSink(ship);
        };

        board.onShipAttacked[ShipType.Carrier] += GetShip(ShipType.Carrier).OnHiitted;
        board.onShipAttacked[ShipType.BattleShip] += GetShip(ShipType.BattleShip).OnHiitted;
        board.onShipAttacked[ShipType.Destroyer] += GetShip(ShipType.Destroyer).OnHiitted;
        board.onShipAttacked[ShipType.Submarine] += GetShip(ShipType.Submarine).OnHiitted;
        board.onShipAttacked[ShipType.PatrolBoat] += GetShip(ShipType.PatrolBoat).OnHiitted;

        AutoShipDeployment();
    }

    private void OnSink(Ship ship)
    {
        GameManager.Instance.CameraShake(1);
 
        if (fireSetter != null)
            fireSetter.SetFire(ship);
    }

    protected override void OnTestLClick(InputAction.CallbackContext context)
    {
        base.OnTestLClick(context);

        // 배치할 배가 선택중이 아니면 공격
        if (TargetShip == null)
        {
            Vector2Int attackPos = board.GetMouseGridPosition();
            Debug.Log($"공격할 그리드 좌표 : {board.GetMouseGridPosition()}");
            board.OnAttacked(attackPos);
        }
    }

    Ship GetShip(ShipType shipType)
    {
        return testShips[(int)shipType - 1];
    }

}
