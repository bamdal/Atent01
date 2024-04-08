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
                    onFlagReturn?.Invoke();
                    break;
                case CellCoverState.Flag:
                    cover.sprite = Board[CloseCellType.Flag];
                    onFlagUse?.Invoke();
                    break;
                case CellCoverState.Question:
                    cover.sprite = Board[CloseCellType.Question];
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

    public bool IsFlaged => coverState == CellCoverState.Flag;

    private void Awake()
    {
        Transform child = transform.GetChild(0);
        cover = child.GetComponent<SpriteRenderer>();
        child = transform.GetChild(1);
        inside = child.GetComponent<SpriteRenderer>();
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
        if (!isOpne)
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

    public void LeftPress()
    {
        if (!isOpne)
        {
            switch (CoverState)
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
        }
        else
        {
            foreach (var cell in neighbors)
            {
                switch (cell.CoverState)
                {
                    case CellCoverState.None:
                        cell.cover.sprite = Board[CloseCellType.ClosePress];
                        break;
                    case CellCoverState.Question:
                        cell.cover.sprite = Board[CloseCellType.QuestionPress];
                        break;
                    default:
                        break;
                }
            }
        }

    }

    public void LeftRelease()
    {
        //RestoreCover();
        Open();
        
    }

    void Open()
    {
        if (!isOpne && !IsFlaged)
        {
            isOpne = true;
            cover.gameObject.SetActive(false);

            if (aroundMineCount <= 0)
            {
                foreach (var cell in neighbors)
                {
                    cell.Open();
                }
            }
            else if(hasMine)
            {
                Debug.Log("gameover");
            }
        }
  
    }

    /// <summary>
    /// 원래 커버 이미지로 변경하는 함수
    /// </summary>
    public void RestoreCover()
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

#if UNITY_EDITOR
    public void Test_OpenCover()
    {
        if (cover != null)
            cover.gameObject.SetActive(false);
    }
#endif
}
