using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Background : MonoBehaviour
{

    /// <summary>
    /// 이동 속도
    /// </summary>
    public float scrollingSpeed = 2.5f;

    /// <summary>
    /// 그림의 가로 길이
    /// </summary>
    const float BackgroundWidth = 13.6f;
    /*    Vector3 BGMove=Vector3.zero;
        float BGUp=0f;
        int BGIndex = 0;
        public GameObject[] Slot = new GameObject[3];
        //실습
        //배경이 계속 반복되게 만들기

        private void Awake()
        {

        }
        private void FixedUpdate()
        {
            BGMove = scrollingSpeed * Time.deltaTime * Vector3.left;
            transform.Translate(BGMove);
            BGUp += BGMove.x;
            Debug.Log(BGUp);
            if(BGUp <= -BackgroundWidth)
            {
                BGUp = 0.0f;
                Slot[BGIndex].transform.position = new Vector3(Slot[BGIndex].transform.position.x + BackgroundWidth*3, Slot[BGIndex].transform.position.y);
                BGIndex++;
                if(BGIndex >2)
                    BGIndex = 0;
            }
        }*/

    //실습
    //배경이 계속 반복되게 만들기

    /// <summary>
    /// 움직일 대상
    /// </summary>
    Transform[] bgSlots;

    /// <summary>
    /// 오른쪽 끝으로 보내는 기준
    /// </summary>
    float baseLineX;

    protected virtual void Awake()
    {
        bgSlots = new Transform[transform.childCount]; // 배열 만들고
        for (int i = 0; i < bgSlots.Length; i++)
        {
            bgSlots[i] = transform.GetChild(i); // 배열에 자식을 하나씩 넣기
        }

        baseLineX = transform.position.x - BackgroundWidth; // 기준이 될 x위치 구하기
    }

    private void Update()
    {
        for (int i = 0; i < bgSlots.Length; i++)
        {
            bgSlots[i].Translate(Time.deltaTime * scrollingSpeed * -transform.right); // 이동 대상을 계속 왼쪽으로 이동
            if (bgSlots[i].position.x < baseLineX) // 기준선을 넘었는지 확인후
            {
                MoveRight(i); // 넘었으면 오른쪽 끝으로 보내기
            }
        }
    }

    /// <summary>
    /// 오른쪽 끝으로 이동시키는 함수
    /// </summary>
    /// <param name="index">이동시킬 대상의 인덱스</param>
    protected virtual void MoveRight(int index)
    {
        bgSlots[index].Translate(BackgroundWidth * bgSlots.Length * transform.right); // 들어있는 개수 * 가로길이 만큼 오른쪽으로 보내기
    }
}
