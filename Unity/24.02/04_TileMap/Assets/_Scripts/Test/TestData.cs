using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestData : IComparable<TestData>
{
    int x;
    float y;
    string z;

    public int CompareTo(TestData other)
    {

        if (other == null)               // other가 null이면 내가 크다
            return 1;


        return other.z.CompareTo(this.z);
    }

    public TestData(int x, float y, string z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public static bool operator == (TestData a, TestData b)
    {

        return a.x == b.x;
    }

    public static bool operator !=(TestData a, TestData b)
    {
        return a.x != b.x;
    }

    public override bool Equals(object obj)
    {
        return obj is TestData testData && this.x == testData.x;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(x, y, z);   
    }
    // 생성자 만들기(x,y,z값 받기)
    // TestData의 리스트에서 Sort함수를 사용할 수 있게 만들기 (기준은 z, 내림차순)
    // == 명령어 오버로딩하기(x값이 같으면 같다)
}
