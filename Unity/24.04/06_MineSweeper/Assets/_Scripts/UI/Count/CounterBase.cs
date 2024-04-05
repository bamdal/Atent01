using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ImageNumber))] // 이 스크립트가 들어간 게임오브젝트는 반드시 해당 컴포넌트가 있어야 한다.
public class CounterBase : MonoBehaviour
{
    protected ImageNumber imageNumber;

    protected virtual void Awake()
    {
        imageNumber = GetComponent<ImageNumber>();
    }

    /// <summary>
    /// imageNumber를 갱신해서 표시되는 내용 변경하는 함수
    /// </summary>
    /// <param name="count">imageNumber가 표시할 수</param>
    protected virtual void Refresh(int count)
    {
        imageNumber.Number = count;
    }
}
