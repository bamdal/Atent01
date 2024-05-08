
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBase : MonoBehaviour
{
    /// <summary>
    /// 이 플레이어가 가지는 보드
    /// </summary>
    protected Board board;

    public Board Board  => board;

    /// <summary>
    /// 공격 후보지역 표시용 프리펩
    /// </summary>
    public GameObject criticalMarkPrefab;

    /// <summary>
    /// 공격 후보지역들이 생성후 붙을 부모 트랜스폼
    /// </summary>
    Transform criticalMarkParent;

    /// <summary>
    /// 생성한 공격 후보지역들
    /// </summary>
    Dictionary<uint,GameObject> criticalMarks;


    protected PlayerBase opponent;
    
    Ship ship;

    /// <summary>
    /// 이 플레이거가 가지는 함선
    /// </summary>
    protected Ship[] ships;

    public Ship[] Ships => ships;

    FireSetter fireSetter;

    /// <summary>
    /// 함선 매니저
    /// </summary>
    protected ShipManager shipManager;

    /// <summary>
    /// 게임매니저
    /// </summary>
    protected GameManager gameManager;

    /// <summary>
    /// 현재 확인하고 있는 함선
    /// </summary>
    public Ship TargetShip
    {
        get => ship;
        set
        {
            if (ship != null && !ship.IsDeployed)    // 이전배가 있고 배치가 되지않았을 때만
            {
                ship.gameObject.SetActive(false);   // 이전배는 안보이게
            }

            ship = value;
            if (ship != null && !ship.IsDeployed)   // 새로 배가 설정되면
            {

                ship.SetMaterialType(false);    // 머티리얼 배치모드로 바꾸기
                OnShipMovement();               // 배치가능한지 머터리얼수정
                ship.transform.position = Board.GridToWorld(Board.GetMouseGridPosition()) + Board.transform.position;  // 마우스 위치로 옮기고
                ship.gameObject.SetActive(true);// 보여주기
            }
        }
    }

    /// <summary>
    /// 일반 공격 후보지역들의 인덱스
    /// </summary>
    List<uint> normalAttackIndice;

    /// <summary>
    /// 우선 순위가 높은 공격후보지역들의 인덱스들
    /// </summary>
    List<uint> criticalAttackIndice;

    /// <summary>
    /// 마지막 공격이 성공한 그리드 좌표 , NOT_SUCCESS면 이전공격 실패
    /// </summary>
    Vector2Int lastSucessAttackPosition;

    /// <summary>
    /// 이전 공격이 실패했다 알리는 읽기전용 변수
    /// </summary>
    readonly Vector2Int NOT_SUCCESS = -Vector2Int.one;

    /// <summary>
    /// 이번 공격으로 상대방 함선이 침몰했는지 알려주는 변수(true면 침몰, false면 아님)
    /// </summary>
    bool opponentShipDestroyed = false;

    /// <summary>
    /// 이웃 위치 확인용
    /// </summary>
    readonly Vector2Int[] neighbors = { new(-1, 0), new(1, 0), new(0, 1), new(0, -1) };
    protected virtual void Awake()
    {
        Transform child = transform.GetChild(0);
        board = child.GetComponent<Board>();
        child = transform.GetChild(1);
        criticalMarkParent = child;
        criticalMarks = new Dictionary<uint, GameObject>(10);
    }

    protected virtual void Start()
    {


        fireSetter = transform.GetChild(0).GetComponentInChildren<FireSetter>();
        shipManager = ShipManager.Instance;
        gameManager = GameManager.Instance;
        Initialize();


        
    }

    protected virtual void Initialize()
    {
        // 함선 생성

        int count = shipManager.ShipTypeCount;
        ships = new Ship[count];

        for(int i = 0; i < count; i++)
        {
            ShipType shipType = (ShipType)i+1;
            Ships[i] = shipManager.MakeShip(shipType, Board.transform);         // 종류별 함선 생성

            Board.onShipAttacked[shipType] += ships[i].OnHiitted;               // 공격 당했을 때 실행할 함수 연결
            ships[i].onHit += (_) => GameManager.Instance.CameraShake(0.1f);    
            ships[i].onSink += (ship) => { OnSink(ship); };
            ships[i].onSink += OnShipDestroy;
        }

        Board.ResetBoard(ships);

        // 일반 공격 후보지역 만들기
        int fullsize = Board.BoardSize * Board.BoardSize;
        uint[] temp = new uint[fullsize];
        for (uint i = 0; i < fullsize; i++)
        {
            temp[i] = i;
        }
        
        Util.Shuffle(temp);
        normalAttackIndice = new List<uint>(temp);  // 배열 무작위 순서 리스트

        criticalAttackIndice = new List<uint>(10);  // 우선 순위가 높은 공격 후보 지역 만들기(비어있음)

        lastSucessAttackPosition = NOT_SUCCESS;
    }

    // 턴 관리용 함수                 --------------------------------------------------------------------------


    // 공격 관련 함수                 --------------------------------------------------------------------------

    /// <summary>
    /// 적을 공격하는 함수
    /// </summary>
    /// <param name="attackGrid">공격 위치(그리드)</param>
    public void Attack(Vector2Int attackGrid)
    {
        Board oppenentBoard = opponent.Board;
            
        if (oppenentBoard.IsInBoard(attackGrid) && oppenentBoard.IsAttackable(attackGrid))
        {
            Debug.Log($"공격할 그리드 좌표 : {oppenentBoard.GetMouseGridPosition()}");
            bool result = oppenentBoard.OnAttacked(attackGrid);
            if (result)
            {
                if (opponentShipDestroyed)
                {
                    // 현재 공격으로 침몰된 경우
                    RemoveAllCriticalPositions();   // 우선 순위가 높은 후보지역 모두 제거
                    opponentShipDestroyed = false;  // 확인 되었으니 리셋
                }
                else
                {
                    // 침몰되지 않은 경우
                    if (lastSucessAttackPosition != NOT_SUCCESS)
                    {
                        // 연속공격 성공 => 한줄로 공격 성공
                        AddCrticalFromTwoPoint(attackGrid, lastSucessAttackPosition);
                    }
                    else
                    {
                        AddCriticalFromNeighbors(attackGrid);
                    }

                    lastSucessAttackPosition = attackGrid;
                }

            }
            else
            {
                //lastSucessAttackPosition = NOT_SUCCESS;   // 성공->실패->성공순서였을때 두번째 성공에서 주변모두를 추가하는 문제 수정용
            }

            uint attackIndex = (uint)Board.GridToIndex(attackGrid).Value;
            RemoveCriticalPosition(attackIndex);
            normalAttackIndice.Remove(attackIndex);
        }


    }



    /// <summary>
    /// 적을 공격하는 함수
    /// </summary>
    /// <param name="world">공격 위치(월드)</param>
    public void Attack(Vector3 world)
    {
        Attack(opponent.Board.WorldToGrid(world));
    }

    /// <summary>
    /// 적을 공격하는 함수
    /// </summary>
    /// <param name="index">공격 위치(인덱스)</param>
    public void Attack(uint index)
    {
        Attack(opponent.Board.IndexToGrid(index));
    }



    /// <summary>
    /// 자동으로 공격하는 함수, Enemy가 공격할 때나 User가 타임 아웃되었을 때 사용하는 목적
    /// </summary>
    public void AutoAttack()
    {
        // 무작위
        // 이전공격이 성공했을때
        // 공격이 한줄로 성공이 되었을때

        uint target;
        if(criticalAttackIndice.Count > 0)      // 우선순위가 높은 공격이 있는지 체크
        {
            target = criticalAttackIndice[0];   // 0번 꺼내기
            criticalAttackIndice.RemoveAt(0);   // 0번 삭제
            normalAttackIndice.Remove(0);       // normal에도 제거
        }
        else
        {
            target = normalAttackIndice[0];     // 우선 순위가 높은 공격 후보지역이 없으면 noraml에서 선택
            normalAttackIndice.RemoveAt(0);     // 0번 삭제
        }

        Attack(target);
    }

    // 함선 배치 관련 함수            --------------------------------------------------------------------------

    /// <summary>
    /// 배에 움직임이 있었을 때 그 상태가 배치가능한지 여부 파악후 색상 변경
    /// </summary>
    protected virtual void OnShipMovement()
    {
        bool isSucess = board.IsShipDeploymentAvailable(TargetShip, TargetShip.transform.position); // 배치 가능한지 확인
        ShipManager.Instance.SetDeployModeColor(isSucess);

    }


    /// <summary>
    /// 아직 배치되지 않는 배를 모두 자동으로 배치하는 함수
    /// <paramref name="isShipsShow">보여줄지 말지 결정</param>
    /// </summary>
    public void AutoShipDeployment(bool isShipsShow)
    {


        int maxCapacity = Board.BoardSize * Board.BoardSize;
        List<int> high = new List<int>(maxCapacity);
        List<int> low = new List<int>(maxCapacity);


        for (int i = 0; i < maxCapacity; i++)
        {
            if (i % Board.BoardSize == 0                            // 0,10,20,30,40,50,60,70,80,90
                || i % Board.BoardSize == (Board.BoardSize - 1)      // 9,19,29,39,49,59,69,79,89,99
                || (i > 0 && i < Board.BoardSize - 1)                // 1,2,3,4,5,6,7,8,9
                || (Board.BoardSize * (Board.BoardSize - 1) < i && i < Board.BoardSize * Board.BoardSize - 1) // 91~98
                )
            {
                low.Add(i);
            }
            else
            {
                high.Add(i);
            }

           
        }

        foreach (var ship in Ships)
        {
            if (ship.IsDeployed)
            {
                int[] shipIndice = new int[ship.Size];
                for (int i = 0; i < ship.Size; i++)
                {
                    shipIndice[i] = Board.GridToIndex(ship.Positions[i]).Value; //배가 배치된 인덱스
                }

                foreach (var index in shipIndice)
                {
                    high.Remove(index); // 이미 배치된 위치를 제거
                    low.Remove(index);
                }

                List<int> toLow = GetShipAroundPositions(ship);
                foreach (var index in toLow)
                {
                    high.Remove(index);
                    low.Add(index);
                }
            }
        }
        // high와 low 내부의 순서 섞기
        int[] temp = high.ToArray();
        Util.Shuffle(temp);
        high = new(temp);

        temp = low.ToArray();
        Util.Shuffle(temp);
        low = new(temp);

        // 함선 배치 시작
        foreach (var ship in Ships)
        {
            if (!ship.IsDeployed)   // 배치 되어있는경우 처리
            {
                ship.RandomRotate();    // 함선 방향 돌리기

                bool fail = true;   // 배치 가능 여부
                int count = 0;      // 배치 시도 횟수
                const int maxHighCount = 5;    // 배치 최대 횟수
                Vector2Int grid;                // 배의 머리위치
                Vector2Int[] shipPositions;     // 함선이 배치 가능할때의 배치되는 위치들
                do // high에서 위치 고르기
                {
                    count++;
                    int head = high[0];     // high에서 첫번째 아이템 가져오기
                    high.RemoveAt(0);

                    grid = board.IndexToGrid((uint)head);   // 인덱스를 그리드로
                    fail = !board.IsShipDeploymentAvailable(ship, grid, out shipPositions); // 함선 배치
                    if (fail)
                    {   // 머리부분이 배치 불가능이면 high로 되돌리기
                        high.Add(head);
                    }
                    else
                    {
                        for (int i = 1; i < shipPositions.Length; i++)  // 몸통도 배치가능한지 체크
                        {
                            int body = board.GridToIndex(shipPositions[i]).Value;
                            if (!high.Contains(body))
                            {
                                // high에 나머지 부분이 없으면 high에 되돌리고 실패처리
                                high.Add(head);
                                fail = true;
                                break;
                            }
                        }
                    }

                    // 실패했고, 반복횟수가 10 미만에 high가 아직 인덱스에 남아있으면 반복
                } while (fail && count < maxHighCount && high.Count > 0);

                // low에서 위치 고르기
                count = 0;
                while (fail && count < 1000)
                {
                    int head = low[0];  // low의 첫번째 아이템 꺼내기
                    low.RemoveAt(0);
                    grid = board.IndexToGrid((uint)head);   // 함선머리부분의 그리드 좌표 구하기
                    fail = !board.IsShipDeploymentAvailable(ship, grid, out shipPositions);   //배치 가능한지 확인
                    if (fail)
                    {
                        low.Add(head);  // 배치가 불가능하면 low에 되돌리기
                    }
                    count++;
                }

                if (fail)
                {
                    Debug.LogWarning("배치 실패");
                    return;
                }

                // 실제 배치
                board.ShipDeployment(ship, grid);
                ship.gameObject.SetActive(isShipsShow);

                // 배치된 위치를 high와 low에서 제거
                List<int> tempList = new List<int>(shipPositions.Length);
                foreach (var pos in shipPositions)
                {
                    tempList.Add(board.GridToIndex(pos).Value);
                }

                foreach (var index in tempList)
                {
                    high.Remove(index);
                    low.Remove(index);
                }

                // 배치된 함선 주변 위치를 low로 보내기
                List<int> toLow = GetShipAroundPositions(ship);
                foreach (var index in toLow)
                {
                    if (high.Contains(index))   // high에 있으면 
                    {
                        low.Add(index);         // low로 보내기
                        high.Remove(index);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 함선 주변 위치들의 인덱스를 구하는 함수
    /// </summary>
    /// <param name="ship">주변 위치를 구할 배</param>
    /// <returns>배의 주변 위치들</returns>
    List<int> GetShipAroundPositions(Ship ship)
    {
        List<int> result = new List<int>(ship.Size * 2 + 6);  // 함선 옆면 *2, 머리쪽3개 꼬리쪽 3개
        int? index = null;
        if (ship.Direction == ShipDirection.North || ship.Direction == ShipDirection.South)
        {
            //함선이 세로로 있다 함선 옆면 두줄 넣기
            foreach (var pos in ship.Positions)
            {
                index = board.GridToIndex(pos + Vector2Int.left);
                if (index.HasValue)
                {
                    result.Add(index.Value);
                }
                index = board.GridToIndex(pos + Vector2Int.right);
                if (index.HasValue)
                {
                    result.Add(index.Value);
                }
            }

            Vector2Int head;
            Vector2Int tail;
            if (ship.Direction == ShipDirection.North)
            {
                head = ship.Positions[0] + Vector2Int.up;
                tail = ship.Positions[^1] + Vector2Int.down;
            }
            else
            {
                head = ship.Positions[0] + Vector2Int.down;
                tail = ship.Positions[^1] + Vector2Int.up;

            }

            index = board.GridToIndex(head);
            if (index.HasValue) { result.Add(index.Value); }
            index = board.GridToIndex(head + Vector2Int.left);
            if (index.HasValue) { result.Add(index.Value); }
            index = board.GridToIndex(head + Vector2Int.right);
            if (index.HasValue) { result.Add(index.Value); }

            index = board.GridToIndex(tail);
            if (index.HasValue) { result.Add(index.Value); }
            index = board.GridToIndex(tail + Vector2Int.left);
            if (index.HasValue) { result.Add(index.Value); }
            index = board.GridToIndex(tail + Vector2Int.right);
            if (index.HasValue) { result.Add(index.Value); }

        }
        else
        {
            // 함선이 가로로 있는 경우
            foreach (var pos in ship.Positions)
            {
                index = board.GridToIndex(pos + Vector2Int.up);
                if (index.HasValue)
                {
                    result.Add(index.Value);
                }
                index = board.GridToIndex(pos + Vector2Int.down);
                if (index.HasValue)
                {
                    result.Add(index.Value);
                }
            }

            Vector2Int head;
            Vector2Int tail;
            if (ship.Direction == ShipDirection.East)
            {
                head = ship.Positions[0] + Vector2Int.right;
                tail = ship.Positions[^1] + Vector2Int.left;
            }
            else
            {
                head = ship.Positions[0] + Vector2Int.left;
                tail = ship.Positions[^1] + Vector2Int.right;
            }

            index = board.GridToIndex(head);
            if (index.HasValue) { result.Add(index.Value); }
            index = board.GridToIndex(head + Vector2Int.up);
            if (index.HasValue) { result.Add(index.Value); }
            index = board.GridToIndex(head + Vector2Int.down);
            if (index.HasValue) { result.Add(index.Value); }

            index = board.GridToIndex(tail);
            if (index.HasValue) { result.Add(index.Value); }
            index = board.GridToIndex(tail + Vector2Int.up);
            if (index.HasValue) { result.Add(index.Value); }
            index = board.GridToIndex(tail + Vector2Int.down);
            if (index.HasValue) { result.Add(index.Value); }
        }

        return result;
    }

    /// <summary>
    /// 모든 함선의 배치를 취소하는 함수
    /// </summary>
    public void UndoAllShipDeployment()
    {

        Board.ResetBoard(Ships);
    }




    // 함선 침몰 및 패배처리 함수      --------------------------------------------------------------------------
    private void OnSink(Ship ship)
    {
        GameManager.Instance.CameraShake(1);

        if (fireSetter != null)
            fireSetter.SetFire(ship);
    }




    /// <summary>
    /// grid 사방을 우선 순위가 높은 지역으로 설정
    /// </summary>
    /// <param name="grid"></param>
    private void AddCriticalFromNeighbors(Vector2Int grid)
    {

        Util.Shuffle(neighbors);
        foreach(var neighbor in neighbors)
        {
            Vector2Int pos = grid + neighbor;
            if (opponent.Board.IsInBoard(pos) && opponent.Board.IsAttackable(pos))
            {
                AddCritical((uint)Board.GridToIndex(pos).Value);
            }

        }
    }

    /// <summary>
    /// 현재 성공지점의 양끝은 우선 순위가 높은 후보지역으로 만드는 함수
    /// </summary>
    /// <param name="now">지금 공격 성공한 위치</param>
    /// <param name="last">직전에 공격 성공한위치</param>
    private void AddCrticalFromTwoPoint(Vector2Int now, Vector2Int last)
    {
        if (IsSuccessLine(last, now, true))
        {
            // 양끝에 위치를 Critical에 추가

            // 같은 줄에 있는 것이 아니면 제거
            Vector2Int grid = now;
            List<uint> deleteTarget = new List<uint>(16);
            foreach (var index in criticalAttackIndice)
            {
                grid = Board.IndexToGrid(index);    
                if (grid.y != now.y)            // y가 다르면 한줄에 있는것이 아니다
                {
                    deleteTarget.Add(index);
                }
            }
            foreach (var index in deleteTarget)
            {
                RemoveCriticalPosition(index);  // 같은 줄에 있는것이 아니면 삭제
            }
            
            grid = now;
            for (grid.x = now.x + 1; grid.x < Board.BoardSize; grid.x++)  // now의 오른쪽 확인해서 추가
            {
                if (!Board.IsInBoard(grid))
                    break;
                if (opponent.Board.IsAttackFailPosition(grid))
                    break;
                if (opponent.Board.IsAttackable(grid))
                {
                    AddCritical((uint)Board.GridToIndex(grid).Value);
                    break;
                }
            }
            for (grid.x = now.x - 1; grid.x > -1; grid.x--) // now의 왼쪽 확인해서 추가
            {
                if (!Board.IsInBoard(grid))
                    break;
                if (opponent.Board.IsAttackFailPosition(grid))
                    break;
                if (opponent.Board.IsAttackable(grid))
                {
                    AddCritical((uint)Board.GridToIndex(grid).Value);
                    break;
                }
            }

        }
        else if (IsSuccessLine(last, now, false))
        {
            // 양끝에 위치를 Critical에 추가
            Vector2Int grid = now;
            List<uint> deleteTarget = new List<uint>(16);
            foreach (var index in criticalAttackIndice)
            {
                grid = Board.IndexToGrid(index);
                if (grid.x != now.x)            // y가 다르면 한줄에 있는것이 아니다
                {
                    deleteTarget.Add(index);
                }
            }
            foreach (var index in deleteTarget)
            {
                RemoveCriticalPosition(index);  // 같은 줄에 있는것이 아니면 삭제
            }



            grid = now;
            for (grid.y = now.y + 1; grid.y < Board.BoardSize; grid.y++)  // now의 아래쪽 확인해서 추가
            {
                if (!Board.IsInBoard(grid))
                    break;
                if (opponent.Board.IsAttackFailPosition(grid))
                    break;
                if (opponent.Board.IsAttackable(grid))
                {
                    AddCritical((uint)Board.GridToIndex(grid).Value);
                    break;
                }
            }
            for (grid.y = now.y - 1; grid.y > -1; grid.y--) // now의 위쪽 확인해서 추가
            {
                if (!Board.IsInBoard(grid))
                    break;
                if (opponent.Board.IsAttackFailPosition(grid))
                    break;
                if (opponent.Board.IsAttackable(grid))
                {
                    AddCritical((uint)Board.GridToIndex(grid).Value);
                    break;
                }
            }

        }
        else
        {
            // 실패 같은 줄 아님
            AddCriticalFromNeighbors(now);
        }
    }

    /// <summary>
    /// 한줄로 공격이 성공했는지 (start에서 end까지 모두 공격 성공이였는지를) 체크하는 함수
    /// </summary>
    /// <param name="start">확인 시작점</param>
    /// <param name="end">확인 종료지점</param>
    /// <param name="isHorizontal">true면 가로로 체크, false면 세로로 체크</param>
    /// <returns>true면 같은 라인에 있고 그 사이는 모두 공격 성공, false는 다른라인이거나 하나라도 공격실패면 false</returns>
    private bool IsSuccessLine(Vector2Int start, Vector2Int end, bool isHorizontal)
    {
        bool result = true;

        Vector2Int pos = start; // start에서 end까지 순차적으로 위치를 저장할 임시 변수
        int dir = 1;            // start에서 end로 가는 방향( 1 or -1 )

        if (isHorizontal)
        {
            if (start.y == end.y)        // y가 같으면 가로로 된 줄이 맞다.
            {
                if (start.x > end.y)     // start가 end보다 오른쪽에 있다.
                {
                    dir = -1;           // 진행 방향을 반대로 설정(역방향)
                }

                start.x *= dir;         // 역방향일 경우 for문에서 정상적으로 돌리기 위해 뒤집기
                end.x *= dir;
                end.x++;                // end의 x까지 확인하기 위해 1증가

                for (int i = start.x; i < end.x; i++)  // i는 start의 x에서 end의 x까지 증가
                {
                    pos.x = i * dir;
                    if (opponent.Board.IsAttackFailPosition(pos))  // 공격이 실패한 지점인지 확인
                    {
                        result = false;
                        break;
                    }
                }
            }
            else
            {
                // 가로로 된 직선이 아니다
                result = false;
            }
        }
        else
        {
            if (start.x == end.x)       // x가 같으면 세로로 된 줄이 맞다.
            {
                if (start.y > end.y)    // start가 end보다 아래쪽에 있다.
                {
                    dir = -1;           // 진행 방향을 반대로 설정(역방향)
                }

                start.y *= dir;         // 역방향일 경우 for문에서 정상적으로 돌리기 위해 뒤집기
                end.y *= dir;
                end.y++;                // end의 y까지 확인하기 위해 1증가

                for (int i = start.y; i < end.y; i++)  // i는 start의 y에서 end의 y까지 증가
                {
                    pos.y = i * dir;
                    if (opponent.Board.IsAttackFailPosition(pos))  // 공격이 실패한 지점인지 확인
                    {
                        result = false;
                        break;
                    }
                }
            }
            else
            {
                // 가로로 된 직선이 아니다
                result = false;
            }
        }
        return result;
    }

    /// <summary>
    /// 우선 순위가 높은 후보지역에 인덱스를 추가하는 함수
    /// </summary>
    /// <param name="index"></param>
    private void AddCritical(uint index)
    {
        if (!criticalAttackIndice.Contains(index))  // 없을 때만 추가
        {
            criticalAttackIndice.Insert(0, index);  // 항상 앞에 추가 (새로 추가되는 위치가 성공확률이 더 높기 때문)
            
            if (GameManager.Instance.IsTestMode)
            {
                //후보지역 표시
                GameObject obj = Instantiate(criticalMarkPrefab, criticalMarkParent);                            // 프리펨 생성
                obj.transform.position = opponent.Board.transform.position + opponent.Board.IndexToWorld(index); // 적 보드 위치에 맞게 수정
                Vector2Int grid = opponent.Board.IndexToGrid(index);
                obj.name = $"Critical_({grid.x},{grid.y})";
                criticalMarks[index] = obj; // criticalMarks.Add(index,obj);
            }


            
        }
    }

    /// <summary>
    /// 우선 순위가 낮은 후보지역을 제거
    /// </summary>
    /// <param name="index"></param>
    private void RemoveCriticalPosition(uint index)
    {
        if (criticalAttackIndice.Contains(index))
        {
            criticalAttackIndice.Remove(index);     // 공격 후보지역에서 제거

            if (criticalMarks.ContainsKey(index))   // 키가 있다면
            {
                Destroy(criticalMarks[index]);      // 오브젝트 제거
                criticalMarks[index] = null;        // 제거 했다고 표시
                criticalMarks.Remove(index);        // 키값제거

            }
        }
    }


    /// <summary>
    /// 모든 우선 순위가 높은 후보지역을 제거
    /// </summary>
    void RemoveAllCriticalPositions()
    {
        while(criticalMarkParent.childCount > 0)
        {
            Transform child = criticalMarkParent.GetChild(0);
            child.SetParent(null);
            Destroy(child.gameObject);
        }
        criticalMarks.Clear();                  // 딕셔너리 초기화
        criticalAttackIndice.Clear();           // 공격 후보지역 리스트 초기화
        lastSucessAttackPosition = NOT_SUCCESS;
    }

    void OnShipDestroy(Ship ship)
    {
        opponent.opponentShipDestroyed = true;              // 상대방에게 (상대방의 상대방(나의))함선이 침몰되었다고 표시
        opponent.lastSucessAttackPosition = NOT_SUCCESS;    // 상대방의 마지막 공격 성공 위치 초기화(함선이 침몰되었으므로 다음에는 랜덤)
    }



    // 기타

    public void Clear()
    {
        opponentShipDestroyed = false;
        Board.ResetBoard(ships);
    }
}
