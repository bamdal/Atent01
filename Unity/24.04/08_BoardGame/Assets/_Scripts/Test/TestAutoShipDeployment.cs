
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestAutoShipDeployment : TestShipDeployment
{
    public Button reset;
    public Button random;
    public Button resetAndRandom;

    protected override void Start()
    {
        base.Start();

        reset.onClick.AddListener(ClearBoard);
        random.onClick.AddListener(AutoShipDeployment);
        resetAndRandom.onClick.AddListener(ClearBoard);
        resetAndRandom.onClick.AddListener(AutoShipDeployment);
    }

    /// <summary>
    /// 아직 배치되지 않는 배를 모두 자동으로 배치하는 함수
    /// </summary>
    protected void AutoShipDeployment()
    {
        /*        Debug.Log("실행");
                //testShips에 !IsDeployed인것들 배치
                foreach(var ship in testShips)
                {
                    if (!ship.IsDeployed)
                    {
                        for (int i = 0; i < Random.Range(0, 3); i++)
                        {
                            ship.Rotate();
                        }

                        if (!board.ShipDeployment(ship, board.IndexToGrid((uint)Randomindex())))
                        {
                            Debug.Log("설치 실패");
                            AutoShipDeployment();
                        }
                        else
                        {
                            ship.gameObject.SetActive(true);
                        }
                    }
                }*/

        int maxCapacity = Board.BoardSize * Board.BoardSize;
        List<int> high = new List<int>(maxCapacity);
        List<int> low = new List<int>(maxCapacity);


        for(int i = 0; i < maxCapacity; i++) 
        {
            if (i % Board.BoardSize == 0                            // 0,10,20,30,40,50,60,70,80,90
                || i % Board.BoardSize == (Board.BoardSize -1)      // 9,19,29,39,49,59,69,79,89,99
                || (i > 0 && i < Board.BoardSize -1)                // 1,2,3,4,5,6,7,8,9
                || (Board.BoardSize * (Board.BoardSize -1) < i && i < Board.BoardSize * Board.BoardSize-1) // 91~98
                )     
            {
                low.Add(i);
            }
            else
            {
                high.Add(i);
            }

            foreach (var ship in testShips)
            {
                if (ship.IsDeployed)
                {
                    int[] shopIndice = new int[ship.Size];
                    for (int j = 0; j < ship.Size; j++)
                    {
                        shopIndice[i] = board.GridToIndex(ship.Positions[j]).Value; //배가 배치된 인덱스
                    }
                    
                    foreach (var index in shopIndice)
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
        }
        // high와 low 내부의 순서 섞기
        int[] temp = high.ToArray();
        Util.Shuffle(temp);
        high = new(temp);

        temp = low.ToArray();
        Util.Shuffle(temp);
        low = new(temp);

        // 함선 배치 시작
        foreach (var ship in testShips)
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
                            int body= board.GridToIndex(shipPositions[i]).Value;
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
                    fail = !board.IsShipDeploymentAvailable(ship,grid,out shipPositions);   //배치 가능한지 확인
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
                ship.gameObject.SetActive(true);

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
        List<int> result= new List<int>(ship.Size *2 + 6);  // 함선 옆면 *2, 머리쪽3개 꼬리쪽 3개
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
            if(index.HasValue) { result.Add(index.Value); }
            index = board.GridToIndex(head + Vector2Int.left);
            if(index.HasValue) { result.Add(index.Value); }
            index = board.GridToIndex(head + Vector2Int.right);
            if(index.HasValue) { result.Add(index.Value); }

            index = board.GridToIndex(tail);
            if(index.HasValue) { result.Add(index.Value); }
            index = board.GridToIndex(tail + Vector2Int.left);
            if(index.HasValue) { result.Add(index.Value); }
            index = board.GridToIndex(tail + Vector2Int.right);
            if(index.HasValue) { result.Add(index.Value); }

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
    void ClearBoard()
    {

        board.ResetBoard(testShips);
    }


}

// reset누르면 배치된 배들 초기화
// random누르면 배치 안된것들이 자동 배치
// 랜덤 배치 위치의 우선 순위가 있음
