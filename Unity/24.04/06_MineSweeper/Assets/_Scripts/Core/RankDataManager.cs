using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


/// <summary>
/// 랭킹 하나에 대한 정보를 저장하는 클래스
/// </summary>
/// <typeparam name="T">랭킹 기준이 되는 정보용 데이터 타입(IComparable 상속받아 비교가 가능한 타입이여야 한다)</typeparam>
public class RankData<T> : IComparable<RankData<T>> where T : IComparable<T>
{

    // 리스트에 넣었을 때 정렬이 되어야 한다
    // T 타입은 반드시 ICompatable을 상속받은 것만 가능하다
    readonly T data;

    public T Data => data;

    readonly string name; 
    public string Name => name;

    public RankData(T data, string name)
    {
        this.data = data;
        this.name = name;
    }

    public int CompareTo(RankData<T> other)
    {
        if (other == null)
        {
            // 다른 RankData가 null이라면 이 객체는 크다고 가정
            return 1;
        }
        
        // 데이터의 CompareTo 메서드를 사용하여 비교
        return Data.CompareTo(other.Data);  // 숫자가 작은것이 앞, 큰것이 뒤
    }
}

[Serializable]  // 없으면 배열에 대한 참조값만 나오고 필요한 정보가 안나올수 있음
public class RankSaveData
{
    public int[] actionCountRank;       // 1등 ~ 10 까지의 행동 회수 저장용 배열
    public string[] actionCountRankerName;  // 1등 ~ 10 까지의 이름 (행동횟수 기준)

    public float[] playTimeRank;        // 1등 ~ 10 까지의 플레이시간 저장용 배열
    public string[] playTimeRankerName;     // 1등 ~ 10 까지의 이름 (플레이 시간 기준)
}

public class RankDataManager : MonoBehaviour 
{
    /// <summary>
    /// 최대 표현 가능한 랭킹 개수
    /// </summary>
    const int RankCount = 10;

    /// <summary>
    /// 랭킹 정보(행동 순위)
    /// </summary>
    List<RankData<int>> actionRank;

    public List<RankData<int>> ActionRank => actionRank;

    /// <summary>
    /// 랭킹 정보(시간 순위)
    /// </summary>
    List<RankData<float>> timeRank;

    public List<RankData<float>> TimeRank => timeRank;

    // 상수들
    const string RankDataFloder = "Save";           // 세이브 폴더 이름
    const string RankDataFileName = "Ranking.json"; // 세이브 파일 이름(확장자포함)

    private void Awake()
    {
        actionRank = new List<RankData<int>>(RankCount + 1);    // 랭킹 10개 + 새정보 1개
        timeRank = new List<RankData<float>>(RankCount + 1);
    }

    private void Start()
    {
        LoadRankData();

        // 게임 클리어시 업데이트
        GameManager.Instance.onGameClear += () => UpdateData(
            GameManager.Instance.ActionCount,
            GameManager.Instance.PlayTime,
            GameManager.Instance.PlayerName
            );
    }

    /// <summary>
    /// 랭킹 정보 저장함수
    /// </summary>
    void SaveRankData()
    {
        RankSaveData data = new RankSaveData();                     // 세이브 정보를 저장할 클래스 만들기
        data.actionCountRank = new int[actionRank.Count];
        data.actionCountRankerName = new string[actionRank.Count];
        data.playTimeRank = new float[timeRank.Count];
        data.playTimeRankerName = new string[timeRank.Count];

        
        int index = 0;
        foreach(var rankData in actionRank)
        {
            data.actionCountRank[index] = rankData.Data;            // 클레스에 행동 랭킹 저장
            data.actionCountRankerName[index] = rankData.Name;
            index++;
        }
        index = 0;
        foreach (var rankData in timeRank)
        {
            data.playTimeRank[index] = rankData.Data;               // 클래스에 시간 랭킹 저장
            data.playTimeRankerName[index] = rankData.Name;
            index++;
        }

        string json = JsonUtility.ToJson(data);                     // 클래스에 저장된 정보를 json파일 형식으로 변형
        string path = $"{Application.dataPath}/{RankDataFloder}";   // 저장될 경로 
        if(!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);    // Save폴더 없으면 만들기
        }
        string fullPath = $"{path}/{RankDataFileName}";
        File.WriteAllText(fullPath, json);      // json파일 작성

    }

    /// <summary>
    /// 랭킹 업데이트 함수
    /// </summary>
    /// <param name="actionCount">갱신할 클릭횟수</param>
    /// <param name="playTime">게임 플레이 시간</param>
    /// <param name="rankerName">미리 작성해둔 플레이어 이름</param>
    void UpdateData(int actionCount, float playTime, string rankerName)
    {
        // 적절한 타이밍에 실행해서 파라메터 값에 따라 랭크 갱신
        actionRank.Add(new(actionCount, rankerName));
        timeRank.Add(new(playTime, rankerName));

        actionRank.Sort();
        timeRank.Sort();

        if (actionRank.Count > RankCount) 
        {
            actionRank.RemoveAt(RankCount);
        }

        if(timeRank.Count > RankCount)
        {
            timeRank.RemoveAt(RankCount);

        }
        // 마무리 저장
        SaveRankData();
    }

    /// <summary>
    /// 랭킹 정보를 파일에서 로딩하는 함수
    /// </summary>
    void LoadRankData()
    {
        string path = $"{Application.dataPath}/{RankDataFloder}";
        string fullPath = $"{path}/{RankDataFileName}";

        if (Directory.Exists(path) && File.Exists(fullPath))    // 폴더가 있고 파일이 경로안에 있을때
        { 
            string json = File.ReadAllText(fullPath);                       // json파일의 텍스트정보를 가져오기
            RankSaveData data = JsonUtility.FromJson<RankSaveData>(json);   // 텍스트 정보를 클래스 형태로 적용
            actionRank.Clear();
            timeRank.Clear();
    
            int count = data.actionCountRank.Length;
            for(int i = 0;i < count; i++) 
            {
                actionRank.Add(new(data.actionCountRank[i], data.actionCountRankerName[i]));
            }

            count = data.playTimeRank.Length;
            for (int i = 0; i < count; i++)
            {
                timeRank.Add(new(data.playTimeRank[i], data.playTimeRankerName[i]));
            }
        }

    }

#if UNITY_EDITOR

    public void Test_RankSetting()
    {
        actionRank = new List<RankData<int>>(10);
        actionRank.Add(new(1, "AAA"));
        actionRank.Add(new(10, "BBB"));
        actionRank.Add(new(20, "CCC"));
        actionRank.Add(new(30, "DDD"));
        actionRank.Add(new(40, "EEE"));

        timeRank = new List<RankData<float>>(10);
        timeRank.Add(new(10, "AAA"));
        timeRank.Add(new(20, "BBB"));
        timeRank.Add(new(30, "CCC"));
        timeRank.Add(new(40, "DDD"));
        timeRank.Add(new(50, "EEE"));


    }

    public void Test_ActionUpdate(int rank, string name)
    {
        actionRank.Add(new(rank, name));
        actionRank.Sort();
    }

    public void Test_TimeUpdate(float rank, string name)
    {
        timeRank.Add(new(rank, name));
        timeRank.Sort();
    }

    public void Test_Save()
    {
        SaveRankData();
    }

    public void Test_Load()
    {
        LoadRankData();
    }

#endif
}


