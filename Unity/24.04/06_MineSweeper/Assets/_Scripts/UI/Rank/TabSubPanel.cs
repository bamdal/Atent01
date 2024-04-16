using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabSubPanel : MonoBehaviour
{

    /// <summary>
    /// 랭킹 한줄을 모두 모아 놓은 배열
    /// </summary>
    RankLine[] rankLines;

    public enum RankType
    {
        Action = 0, // 행동
        Time        // 시간
    }

    /// <summary>
    /// 이 패널의 타입
    /// </summary>
    public RankType rankType = RankType.Action;


    private void Awake()
    {
        rankLines = GetComponentsInChildren<RankLine>();
    }

    private void Start()
    {
        GameManager.Instance.onGameClear += Refresh;    // 게임 클리어 될 때 리프래쉬
        Refresh();          // 시작할 때 리프래쉬
    }

    /// <summary>
    /// 랭크라인 갱신 함수
    /// </summary>
    void Refresh()
    {
        // rankLines를 RankDataManager에 있는 데이터 기반으로 갱신

        RankDataManager dataManager = GameManager.Instance.RankDataManager;

        int index = 0;
        switch (rankType)   // 랭크 종류 별로
        {
            case RankType.Action:
                foreach (var data in dataManager.ActionRank)
                {
                    rankLines[index].SetData<int>(index + 1, data.Data, data.Name); // 순서대로 데이터 입력
                    index++;
                }
                break;
            case RankType.Time:
                foreach (var data in dataManager.TimeRank)
                {
                    rankLines[index].SetData<float>(index + 1, data.Data, data.Name);
                    index++;
                }
                break;
        }
        // 필요 없는 부분은 안보이게 만들기
        for(int i = index;i<rankLines.Length;i++)
        {
            rankLines[i].gameObject.SetActive(false);
        }
    }
}
