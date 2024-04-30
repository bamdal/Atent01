using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 배의 종류
/// </summary>
public enum ShipType : byte
{
    None = 0,
    Carrier,    // 항공모함(5칸)
    BattleShip, // 전함 4
    Destroyer,  // 구축함 3
    Submarine,  // 잠수함 3
    PatrolBoat  // 경비정 2
}

/// <summary>
/// 뱃머리가 바라보는 방향
/// </summary>
public enum ShipDirection : byte
{
    North = 0,
    East = 1,
    South = 2,
    West
}

public class Ship : MonoBehaviour
{
    /// <summary>
    /// 배 종류
    /// </summary>
    ShipType shipType = ShipType.None;

    /// <summary>
    /// 배 종류 확인 및 설정용 프로퍼티
    /// </summary>
    public ShipType Type
    {
        get => shipType;
        private set
        {
            shipType = value;
            switch (shipType)   // 배 종류 별로 크기와 이름을 설정한다
            {
                case ShipType.Carrier:
                    size = 5;
                    shipName = "항공모함";
                    break;
                case ShipType.BattleShip:
                    size = 4;
                    shipName = "전함";
                    break;
                case ShipType.Destroyer:
                    size = 3;
                    shipName = "구축함";
                    break;
                case ShipType.Submarine:
                    size = 3;
                    shipName = "잠수함";
                    break;
                case ShipType.PatrolBoat:
                    size = 2;
                    shipName = "경비정";
                    break;
            }
        }
    }

    /// <summary>
    /// 배의 이름
    /// </summary>
    string shipName = string.Empty;

    /// <summary>
    /// 배의 이름 확인용 프로퍼티
    /// </summary>
    public string ShipName => shipName;

    /// <summary>
    /// 배의 크기( = 최대 HP)
    /// </summary>
    int size = 0;

    /// <summary>
    /// 배의 크기 확인용 프로퍼티
    /// </summary>
    public int Size => size;

    /// <summary>
    /// 배의 현재 내구도
    /// </summary>
    int hp = 0;

    public int HP
    {
        get => hp;
        private set
        {
            hp = value;
            OnHiitted();
            if (!IsAlive)       // hp가 0이하면 
            {
                OnSinking();    // 침몰
            }
        }
    }

    /// <summary>
    /// HP가 0보다 크면 살아있다
    /// </summary>
    bool IsAlive => hp > 0;

    /// <summary>
    /// 배가 바라보는 방향(북동남서로 회전이 정방향)
    /// </summary>
    ShipDirection direction = ShipDirection.North;

    public ShipDirection Direction
    {
        get => direction;
        set
        {
            direction = value;
            modelRoot.rotation = Quaternion.Euler(0, (int)direction * 90.0f, 0);
            //modelRoot 를 방향에 맞게 회전 
        }
    }

    /// <summary>
    /// 배의 모델 메시의 트랜스폼
    /// </summary>
    Transform modelRoot;

    /// <summary>
    /// 배가 배치된 위치(그리드 좌표)
    /// </summary>
    Vector2Int[] positions;

    /// <summary>
    /// 배가 배치된 위치 확인용 프로퍼티
    /// </summary>
    public Vector2Int[] Positions => positions;   

    /// <summary>
    /// 배의 배치 여부(true면 배치되었고, false면 배치되지 않았다.
    /// </summary>
    bool isDeployed = false;

    /// <summary>
    /// 배의 배치 여부 확인용 프로퍼티
    /// </summary>
    public bool IsDeployed => isDeployed;

    /// <summary>
    /// 배의 머티리얼을 변경하기 위한 랜더러
    /// </summary>
    Renderer shipRenderer;

    /// <summary>
    /// 함선이 배치되거나 배치 해제 되었을 때를 알리는 델리게이트(true면 배치 false면 배치해제)
    /// </summary>
    public Action<bool> onDepoloy;

    /// <summary>
    /// 함선이 공격을 당했을 떄를 알리는 델리게이트(Ship : 자기자신, 이름이나 종류에 대한 접근이 필요함)
    /// </summary>
    public Action<Ship> onHit;


    /// <summary>
    /// 함선이 침몰됨을 알리는 델리게이트(Ship : 자기자신)
    /// </summary>
    public Action<Ship> onSink;

    int shipDirectionCount;

    /// <summary>
    /// 배 초기화용 함수
    /// </summary>
    /// <param name="shipType">배의 종류</param>
    public void Initialize(ShipType shipType)
    {
        Type = shipType;
        HP = Size;

        modelRoot = transform.GetChild(0);
        shipRenderer = modelRoot.GetComponentInChildren<Renderer>();
        ResetData();

        gameObject.name = $"{Type}_{Size}";
        gameObject.SetActive(false);

        shipDirectionCount = ShipManager.Instance.ShipTypeCount;
    }


    /// <summary>
    /// 공통적으로 데이터 초기화 함수
    /// </summary>
    void ResetData()
    {

        Direction = ShipDirection.North;
        isDeployed = false;
        positions = null;
    }

    /// <summary>
    /// 함선의 머터리얼 설정
    /// </summary>
    /// <param name="isNomal">true면 불투명, false면 배치용 반투명</param>
    public void SetMaterialType(bool isNomal = true)
    {
        if(shipRenderer != null)
        {
            if (isNomal)
            {
                shipRenderer.material = ShipManager.Instance.NormalShipMaterial;
            }
            else
            {
                shipRenderer.material = ShipManager.Instance.DeployModeShopMaterial;
            }
        }

    }

    /// <summary>
    /// 함선이 배치되었을 때의 처리를 하는 함수
    /// </summary>
    /// <param name="deployPositions">배치되는 위치들</param>
    public void Deploy(Vector2Int[] deployPositions)
    {
        SetMaterialType();
        isDeployed = true;  // 배치되었다고 표시

        positions = deployPositions;    // 배치된 위치(그리드) 기록
    }

    /// <summary>
    /// 함선이 배치 해제되었을 때의 처리를 하는 함수
    /// </summary>
    public void UnDeploy()
    {
        ResetData();    // 데이터 초기화
    }

    /// <summary>
    /// 함선을 90도씩 회전 시키는 함수
    /// </summary>
    /// <param name="isCW">true면 시계방향,false면 반시계방향</param>
    public void Rotate(bool isCW = true)
    {
        if (isCW)
        {
            Direction = (ShipDirection)(((int)Direction + 1) % (shipDirectionCount-1));
        }else
        {
            // 음수 % 연산 방지
            // %연산을 할 때는 나누는 숫자를 몇번을 더해도 결과에 영향을 안끼침
            Direction = (ShipDirection)(((int)Direction + (shipDirectionCount -2)) % (shipDirectionCount-1));
        }
    }

    /// <summary>
    /// 함선을 랜덤한 방향으로 회전시키는 함수
    /// </summary>
    public void RandomRotate()
    {
        int rotateCount = UnityEngine.Random.Range(0,shipDirectionCount);   // 0~3
        for (int i = 0; i < rotateCount; i++)
        {
            Rotate();   // 회전시키기
        }
    }

    /// <summary>
    /// 함선이 공격 받았을 때 실행되는 함수
    /// </summary>
    public void OnHiitted()
    {

    }

    /// <summary>
    /// 배가 가라앉을 때 실행될 함수
    /// </summary>
    void OnSinking()
    {

    }

}
