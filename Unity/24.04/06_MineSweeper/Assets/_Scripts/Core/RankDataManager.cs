using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
        return Data.CompareTo(other.Data);
    }
}

public class RankDataManager : MonoBehaviour 
{
    const int RankCount = 10;

    // 랭킹 정보
    List<RankData<int>> actionRank;
    List<RankData<float>> timeRank;
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
        timeRank.Sort();
    }

    public void Test_TimeUpdate(float rank, string name)
    {
        timeRank.Add(new(rank, name));
        timeRank.Sort();
    }

    public List<RankData<int>> Test_A()
    {
        return actionRank;
    }

    public List<RankData<float>> Test_B()
    {
        return timeRank;

    }
#endif
}


