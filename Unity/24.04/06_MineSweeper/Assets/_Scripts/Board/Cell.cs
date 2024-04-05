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

    private void Awake()
    {
        Transform child = transform.GetChild(0);
        cover = child.GetComponent<SpriteRenderer>();
        child = transform.GetChild(1);
        inside = child.GetComponent<SpriteRenderer>();
    }


    public void ResetData()
    {
        hasMine = false;
        cover.sprite = Board[CloseCellType.Close];
        inside.sprite = Board[OpenCellType.Empty];
        cover.gameObject.SetActive(true);
    }
    
    /// <summary>
    /// 이 셀에 지뢰를 설치하는 함수
    /// </summary>
    public void SetMine()
    {
        hasMine = true;
        inside.sprite = Board[OpenCellType.Mine];
    }

#if UNITY_EDITOR
    public void Test_OpenCover()
    {
        cover.gameObject.SetActive(false);
    }
#endif
}
