using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Device;
using UnityEngine.InputSystem;

public class Board : MonoBehaviour
{
    // 셀을 생성하고 관리(리셋, 지뢰 설치 등등)

    /// <summary>
    /// 생성할 셀의 프리펩
    /// </summary>
    public GameObject cellPrefab;

    /// <summary>
    /// 보드의 가로길이
    /// </summary>
    int width = 16;

    /// <summary>
    /// 보드의 세로 길이
    /// </summary>
    int height = 16;

    /// <summary>
    /// 배치될 지뢰의 개수
    /// </summary>
    int mineCount = 10;

    /// <summary>
    /// 이 보드가 생성한 모든 셀
    /// </summary>
    Cell[] cells;

    /// <summary>
    /// 셀 한칸의 크기
    /// </summary>
    const float Distance = 1.0f;

    public bool IsPlaying => gameManager.IsPlaying;

    /// <summary>
    /// 현재 마우스가 누르고 있는 셀
    /// </summary>
    Cell currentCell = null;

    Cell CurrnetCell
    {
        get => currentCell;
        set
        {
            if (currentCell != value)           // currentCell이 변경되면
            {
                currentCell?.RestoreCovers();   // 이전셀이 눌러 놓았던것은 원래대로 복구
                currentCell = value;
                currentCell?.LeftPress();       // 새 currentCell에 누르기 처리
            }
        }
    }

    public Action onBoardLeftPress;
    public Action onBoardLeftRelease;

    /// <summary>
    /// 입력용 인풋시스템
    /// </summary>
    PlayerInputActions inputActions;

    public Sprite[] openCellImage;
    public Sprite this[OpenCellType type] => openCellImage[(int)type];

    public Sprite[] closeCellImage;
    public Sprite this[CloseCellType type] => closeCellImage[(int)type];

    /// <summary>
    /// 게임 매니저
    /// </summary>
    GameManager gameManager;


    private void Awake()
    {
        inputActions = new PlayerInputActions();    
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.LeftClick.performed += OnLeftPress;     // 왼버튼 누를때
        inputActions.Player.LeftClick.canceled += OnLeftRelease;    // 왼버튼 땠을 때
        inputActions.Player.RightClick.performed += OnRightClick;   // 오른버튼 누를때
        inputActions.Player.MouseMove.performed += OnMouseMove;     // 마우스 움직일 때
    }

    private void OnDisable()
    {
        inputActions.Player.MouseMove.performed -= OnMouseMove;
        inputActions.Player.RightClick.performed -= OnRightClick;
        inputActions.Player.LeftClick.performed -= OnLeftPress;
        inputActions.Player.LeftClick.canceled -= OnLeftRelease;
        inputActions.Player.Disable();
    }

    /// <summary>
    /// 이 보드가 가질 모든 셀을 생성하고 배치하는 함수
    /// </summary>
    /// <param name="newWidth">보드의 가로길이</param>
    /// <param name="newHieght">보드의 세로길이</param>
    /// <param name="newMineCount">배치될 지뢰의 개수</param>
    public void Initialize(int newWidth, int newHieght, int newMineCount)
    {
        // 게임 매니저 저장해 놓기
        gameManager = GameManager.Instance;

        // 값 설정
        width = newWidth;
        height = newHieght;
        mineCount = newMineCount;

        // 셀 배열 만들기
        cells = new Cell[width * height];
        for(int y = 0; y < height; y++)
        {
            for(int x = 0; x < width; x++)
            {
                // 셀 하나 씩 생성하고 위치 설정
                GameObject cellObj = Instantiate(cellPrefab,transform); // 셀 게임 오브젝트 생성
                Cell cell = cellObj.GetComponent<Cell>();

                int id = x + y * width;
                cell.ID = id;           // ID 설정
                cell.transform.localPosition = new Vector3(x * Distance, -y * Distance);
                cell.Board = this;      // Board 기록해두기

                cell.onFlagReturn = gameManager.IncreaseFlagCount;  // 셀에 깃발 설치됐을 때 실행할 함수 연결
                cell.onFlagUse = gameManager.DecreaseFlagCount;     // 셀에 깃발 해제됐을 때 실행할 함수 연결
                cell.onExplosion = gameManager.GameOver;            // 셀에서 지뢰가 터졌을 때 실행할 함수 연결 

                cellObj.name = $"Cell_{id}_({x},{y})";

                cells[id] = cell;

                

            }
        }

        // 셀 전체 초기화
        foreach (Cell cell in cells)
        {
            cell.Initialize();

        }
        gameManager.onGameReady = ResetBoard;   // 레디로 가면 보드 리셋
        gameManager.onGameOver += OnGameOver;   // 게임오버

        // 보드 데이터 리셋
        ResetBoard();

    }



    /// <summary>
    /// 보드에 존재하는 모든 셀의 데이터 리셋하고 지뢰를 새로 배치하는 함수(게임 재시작용 함수)
    /// </summary>
    void ResetBoard()
    {
        // 전체 셀의 데이터 리셋
        foreach (Cell cell in cells)
        {

            cell.ResetData();
        }
        // mineCount만큼 지뢰 배치
        Shuffle(cells.Length, out int[] shuffleResult); // 숫자 섞기(0~ cells.Length-1)

        for (int i = 0; i < mineCount; i++)
        {
            cells[shuffleResult[i]].SetMine();
        }
        count = cells.Length;
    }

    // 게임 매니저 상태 변화시 사용할 함수들 ---------------------------------



    /// <summary>
    /// 게임오버가 되면 보드가 처리할 일을 기록해 놓은 함수
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    private void OnGameOver()
    {
        // 잘못 설치한 깃발은 NotMine으로 cover변환
        // 못찾은 지뢰는 커버를 제거
        foreach (Cell cell in cells)
        {
            if (!cell.HasMine && cell.IsFlaged)
            {
                cell.FlagMistake();

            }
            else if (cell.HasMine && !cell.IsFlaged)
            {
                cell.MineNotFound();
            }
        }

    }



    // 셀 확인용 함수들 ------------------------------------------------------------------

    /// <summary>
    /// 스크린 좌표를 그리드 좌표로 변경해주는 함수
    /// </summary>
    /// <param name="screen">스크린 좌표</param>
    /// <returns>변환된 그리드 좌표</returns>
    Vector2Int ScreenToGrid(Vector2 screen)
    {
        Vector2 world = Camera.main.ScreenToWorldPoint(screen);
        Vector2 diff = world - (Vector2)transform.position;

        return new Vector2Int(Mathf.FloorToInt(diff.x/Distance), Mathf.FloorToInt(-diff.y / Distance));
    }

    /// <summary>
    /// 그리드 좌표를 인덱스로 바꿔주는 함수
    /// </summary>
    /// <param name="x">그리드 좌표 x</param>
    /// <param name="y">그리드 좌표 y</param>
    /// <returns>변환된 인덱스(잘못된 좌표면 null)</returns>
    int? GridToIndex(int x, int y)
    {
        int? result = null;
        if(IsValidGrid(x, y))
        {
            result = x + y * width;
        }



        return result;
    }

    /// <summary>
    /// 인덱스를 그리드 좌표로 만들어 주는 함수
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    Vector2Int IndexToGrid(int index)
    {
        return new(index % width, index / width);
    }

    /// <summary>
    /// 지정된 그리드 좌표가 보드 내부인지 확인하는 함수
    /// </summary>
    /// <param name="x">x 좌표</param>
    /// <param name="y">y 좌표</param>
    /// <returns>보등 안이면 true, 밖이면 false, 셀이 초기화 되지않았다면 null</returns>
    bool IsValidGrid(int x, int y)
    {
        return x >= 0 && y >= 0 && x < width && y < height && cells != null;
    }

    /// <summary>
    /// 특정 스크린 좌표에 있는 셀을 리턴하는 함수
    /// </summary>
    /// <param name="screen">스크린 좌표</param>
    /// <returns>셀이 없으면 null, 그 외에는 스크린 좌표에 있는 셀</returns>
    Cell GetCell(Vector2 screen)
    {
        Cell result = null;
        Vector2Int grid = ScreenToGrid(screen);
        int? index = GridToIndex(grid.x, grid.y);
        if (index != null)
        {
            result = cells[index.Value];
        }
        return result;
    }

    // 입력 처리용 함수 -------------------------------------------------------------------
    private void OnLeftPress(InputAction.CallbackContext context)
    {
        Vector2 screen = Mouse.current.position.ReadValue();
        Cell cell = GetCell(screen);
        if (cell != null)
        {
            CurrnetCell = cell;
            gameManager.GameStart();

            if (gameManager.IsPlaying)
            {
                onBoardLeftPress?.Invoke();

            }
        }
        // 눌렀을 대 커버 변경
        // None Cell ClosePress가 보이고
        // Flag 변화 없음
        // Question - QuestionPress변경
    }
    public int count;
    private void OnLeftRelease(InputAction.CallbackContext context)
    {
        Vector2 screen = Mouse.current.position.ReadValue();
        Cell cell = GetCell(screen);
        if (cell != null)
        {
            cell.LeftRelease();
            gameManager.GameStart();

            if (gameManager.IsPlaying)
            {
                onBoardLeftRelease?.Invoke();
                if(count == gameManager.mineCount)
                {
                    gameManager.GameClear();
                }
            }
           
        }


    }

    

    private void OnRightClick(InputAction.CallbackContext context)
    {
        Vector2 screen = Mouse.current.position.ReadValue();
        Cell cell = GetCell(screen);
        if(cell != null)
        {
            cell.RightPress();
        }

    }
    private void OnMouseMove(InputAction.CallbackContext context)
    {
        if (Mouse.current.leftButton.isPressed)
        {
            Vector2 screen = context.ReadValue<Vector2>();
            Cell cell = GetCell(screen);
            if (cell != null)
            {
                CurrnetCell = cell;
            }
        }


    }
    // 기타 유틸리티 함수들 ------------------------------------------------------------------------------------------

    /// <summary>
    /// 셔플용 함수
    /// </summary>
    /// <param name="count">셔플할 숫자 범위 (0 ~ count -1)</param>
    /// <param name="result">셔플 결과</param>
    void Shuffle(int count, out int[] result)
    {
        // count만큼 순서대로 숫자가 들어간 배열 만들기
        result = new int[count];
        for (int i = 0; i < count; i++)
        {
            result[i] = i;
        }
        // 위에서 만든 배열을 섞기 
        int loopCount = result.Length-1;
        for (int i = 0; i < loopCount; i++) // 8*8일때 63번 반복
        {
            int randomIndex = UnityEngine.Random.Range(0, result.Length - i);   // 처음에는 0 ~ 63 중 랜덤 선택
            int lastIndex = loopCount - i;                                      // 처음엔 63번 인덱스

            (result[lastIndex], result[randomIndex]) = (result[randomIndex], result[lastIndex]);    // 랜덤으로 나온 값과 63번 값 스왑
        }
    }

    /// <summary>
    /// 특정 셀의 주변 셀을 돌려주는 함수
    /// </summary>
    /// <param name="id">중심 셀의 아이디</param>
    /// <returns>중심 셀의 주변 셀들의 리스트</returns>
    public List<Cell> GetNeighbors(int id)
    {
        List<Cell> result = new List<Cell>();
        Vector2Int grid = IndexToGrid(id);  // id의 그리드 위치를 +-1씩해서 구하기
        for (int y = -1; y < 2; y++)
        {
            for (int x = -1; x < 2; x++)
            {
                if (!(x == 0 && y == 0))    // 자기 자신은제외
                {
                    int? index = GridToIndex(x + grid.x, y + grid.y);
                    if (index != null)      // valid한 id면 추가
                    {
                        result.Add(cells[index.Value]);
                    }
                }
            }
        }

        return result;
    }


    // 게임 상태 변환
#if UNITY_EDITOR
    public void Test_OpenAllCover()
    {
        if(cells != null)
        {
            foreach (var cell in cells)
            {
                cell.Test_OpenCover();
            }
        }
        
    }

    public void Test_BoardReset()
    {
        ResetBoard();
    }
#endif
}
