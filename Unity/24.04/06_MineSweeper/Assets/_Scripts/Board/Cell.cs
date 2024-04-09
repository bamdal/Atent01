using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    // 지뢰 배치 여부에 따라 inside 이미지  변경
    // 열기/닫기
    // 보드에 입력에 따른 cover 이미지 변경

    /// <summary>
    /// 이 셀의 ID(위치계산에도 사용될 수 있음
    /// </summary>
    int? id = null;

    /// <summary>
    /// ID 설정용 프로퍼티
    /// </summary>
    public int ID
    {
        get => id.GetValueOrDefault();  // 0일 경우 맞을 수도 있고 아닐 수도 있다
        set
        {
            if (id == null) // 이 프로퍼티는 한번만 설정 가능하다
            {
                id = value;
            }
        }
    }

    /// <summary>
    /// 겉면의 스프라이트 랜더러(Close, Question, Flag)
    /// </summary>
    SpriteRenderer cover;

    /// <summary>
    /// 안쪽의 스프라이트 랜더러(지뢰, 주변 지뢰개수
    /// </summary>
    SpriteRenderer inside;

    /// <summary>
    /// 셀에 지뢰가 있는지 여부
    /// </summary>
    bool hasMine = false;

    public bool HasMine => hasMine;

    /// <summary>
    /// 이 셀이 있는 보드
    /// </summary>
    Board parentBoard = null;


    public Board Board
    {
        get => parentBoard;
        set
        {
            if (parentBoard == null)    // 한번만 설정가능
            {
                parentBoard = value;
            }
        }
    }

    /// <summary>
    /// 게임이 플레이중인지 확인하는 프로퍼티
    /// </summary>
    bool IsPlaying => Board.IsPlaying;

    /// <summary>
    /// 자기 주변 셀의 목록
    /// </summary>
    List<Cell> neighbors;

    /// <summary>
    /// 자기 주변의 지뢰 개수
    /// </summary>
    int aroundMineCount = 0;

    /// <summary>
    /// 셀이 열려있는지 여부
    /// </summary>
    bool isOpne = false;

    /// <summary>
    /// 셀의 커버 표시 상태용(닫혀있을 때의 상태
    /// </summary>
    enum CellCoverState
    {
        None,       // 아무것도 표기되지 않은 상태 
        Flag,       // 깃발이 표시된 상태
        Question,   // 물음표가 표시된 상태
    }


    CellCoverState coverState = CellCoverState.None;


    /// <summary>
    /// 셀의 커버 상태를 설정하고 확인하기 위한 프로퍼티
    /// </summary>
    CellCoverState CoverState
    {
        get => coverState;
        set
        {
            coverState = value;
            switch (coverState)
            {
                case CellCoverState.None:
                    cover.sprite = Board[CloseCellType.Close];

                    break;
                case CellCoverState.Flag:
                    cover.sprite = Board[CloseCellType.Flag];
                    onFlagUse?.Invoke();        // 깃발이 설치되었음을 알림
                    break;
                case CellCoverState.Question:
                    cover.sprite = Board[CloseCellType.Question];
                    onFlagReturn?.Invoke();     // 깃발이 해제되었음을 알림
                    break;
                default:
                    break;
            }

        }
    }

    /// <summary>
    /// 깃발이 사용되었다고 알리는 델리게이트
    /// </summary>
    public Action onFlagUse;

    /// <summary>
    /// 깃발 사용해제 되었다고 알리는 델리게이트
    /// </summary>
    public Action onFlagReturn;

    /// <summary>
    /// 지뢰가 터졌음을 알리는 델리게이트
    /// </summary>
    public Action onExplosion;


    /// <summary>
    /// 깃발 설치 여부를 알려주는 프로퍼티
    /// </summary>
    public bool IsFlaged => coverState == CellCoverState.Flag;

    /// <summary>
    /// 이 셀에 의해 눌려진 셀의 목록(자기자신 or 자기 주변)
    /// </summary>
    List<Cell> pressedCells;


    private void Awake()
    {
        Transform child = transform.GetChild(0);
        cover = child.GetComponent<SpriteRenderer>();
        child = transform.GetChild(1);
        inside = child.GetComponent<SpriteRenderer>();

        pressedCells = new List<Cell>(8);
    }

    /// <summary>
    /// 셀 생성 초기화 함수 (처음에 단 한번만 실행되면 됨)
    /// </summary>
    public void Initialize()
    {
        neighbors = Board.GetNeighbors(ID); // 이웃 셀 저장해 놓기
    }

    public void ResetData()
    {
        hasMine = false;
        aroundMineCount = 0;
        isOpne = false;

        pressedCells.Clear();
        CoverState = CellCoverState.None;

        cover.sprite = Board[CloseCellType.Close];
        inside.sprite = Board[OpenCellType.Empty];
        cover.gameObject.SetActive(true);
    }

    /// <summary>
    /// 이 셀에 지뢰를 설치하는 함수
    /// </summary>
    public void SetMine()
    {
        hasMine = true;                             // 지뢰 설치했다고 표시
        inside.sprite = Board[OpenCellType.Mine];   // 이미지 변경

        foreach (Cell cell in neighbors)
        {
            cell.IncreaseAtoundMineCount(); // 주변 셀의 지뢰 개수에 따라 개수 증가

        }
    }

    /// <summary>
    ///  주변 지뢰 증가용 함수
    /// </summary>
    void IncreaseAtoundMineCount()
    {
        if (!hasMine)   // 현재 셀이 지뢰가 아닐때만 개수 증가
        {
            aroundMineCount++;                                      // 주변 지뢰 개수
            inside.sprite = Board[(OpenCellType)aroundMineCount];   // 주변 지뢰 개수에 따라 이미지 변화
        }
    }


    /// <summary>
    /// 셀이 우클릭 되었을 때 불리는 함수
    /// </summary>
    public void RightPress()
    {
        if (!isOpne && IsPlaying)
            switch (CoverState)
            {
                case CellCoverState.None:
                    CoverState = CellCoverState.Flag;
                    break;
                case CellCoverState.Flag:
                    CoverState = CellCoverState.Question;
                    break;
                case CellCoverState.Question:
                    CoverState = CellCoverState.None;
                    break;
                default:
                    break;
            }
    }

    /// <summary>
    /// 셀이 좌클릭을 했을때 불리는 함수
    /// 누르는 표시용
    /// </summary>
    public void LeftPress()
    {
        if (IsPlaying)
        {
            if (!isOpne)    // 열려있지 않은경우
            {
                switch (CoverState)     // 커버 상태에 따라 눌려진 이미지로 변경
                {
                    case CellCoverState.None:
                        cover.sprite = Board[CloseCellType.ClosePress];
                        break;
                    case CellCoverState.Question:
                        cover.sprite = Board[CloseCellType.QuestionPress];
                        break;
                    default:
                        break;
                }
                pressedCells.Add(this);
            }
            else            // 열려있는 경우
            {
                foreach (var cell in neighbors)         // 열려있는셀의 이웃셀에 대해
                {
                    if (!cell.isOpne && !cell.IsFlaged)  // 닫혀있고 깃발 표시가 안되어있는 셀을
                    {
                        pressedCells.Add(cell);         // 눌려진 셀에 기록
                        cell.LeftPress();               // 누르는 처리 싱행
                    }

                }
            }
        }


    }

    /// <summary>
    /// 마우스 좌클릭을 땠을 때 실행되는 함수
    /// 눌려진 상태 복구용
    /// </summary>
    public void LeftRelease()
    {
        if (IsPlaying)
        {
            if (isOpne)
            {
                // 셀이 열려있다면
                int flagCount = 0;
                foreach (var cell in neighbors) // 주위에 깃발개수 탐색
                {
                    if (cell.IsFlaged)
                    {
                        flagCount++;
                    }
                }
                if (aroundMineCount == flagCount)   // 깃발개수와 주위 폭탄개수가 같다면
                {
                    foreach (var cell in pressedCells)
                    {
                        cell.Open();                // 눌려진 셀들을 연다
                    }
                }
                else
                {
                    RestoreCovers();                // 아니라면 커버 복구
                }
            }
            else
            {
                // 닫혀있는 셀이라면 
                Open(); // 열기

            }

        }


    }

    /// <summary>
    /// 셀을 여는 함수
    /// </summary>
    void Open()
    {
        if (!isOpne && !IsFlaged)   // 닫혀있고 깃발이 설치되어있지 않은 셀만 처리
        {
            Board.count--;
            isOpne = true;                      // 열림처리후
            cover.gameObject.SetActive(false);  // 커버를 지운다

            if (hasMine)                        // 지뢰라면 게임오버
            {
                onExplosion?.Invoke();
                inside.sprite = Board[OpenCellType.Mine_Explosion];
            }
            else if(aroundMineCount <= 0)       // 열은 셀의 주위 지뢰가 하나도 없다면 
            {
                foreach (var cell in neighbors) // 이웃을 전부 연다
                {
                    cell.Open();

                }
            }
        }
  
    }

    /// <summary>
    /// 원래 커버 이미지로 변경하는 함수
    /// </summary>
    void RestoreCover()
    {
        if (!isOpne)
            switch (CoverState)
            {
                case CellCoverState.None:
                    cover.sprite = Board[CloseCellType.Close];
                    break;
                case CellCoverState.Question:
                    cover.sprite = Board[CloseCellType.Question];
                    break;
                default:
                    break;
            }
    }

    /// <summary>
    /// 눌려진 셀을 모두 원래 커버이미지로 변경하는 함수
    /// </summary>
    public void RestoreCovers()
    {
        foreach (var cell in pressedCells)
        {
            cell.RestoreCover();
        }
        pressedCells.Clear();
    }

    /// <summary>
    /// 지뢰가 아닌데 깃발을 설치했을때 표시해주는 함수
    /// </summary>
    public void FlagMistake()
    {
        cover.gameObject.SetActive(false);
        inside.sprite = Board[OpenCellType.Mine_Mistake];

    }

    /// <summary>
    /// 지뢰를 전부 못찾았을때 지뢰위치를 보여주는 함수
    /// </summary>
    public void MineNotFound()
    {
        cover.gameObject.SetActive(false);
    }
#if UNITY_EDITOR
    public void Test_OpenCover()
    {
        if (cover != null)
            cover.gameObject.SetActive(false);
    }
#endif
}
