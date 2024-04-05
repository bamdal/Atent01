using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageNumber : MonoBehaviour
{

    /// <summary>
    /// UI에 뜰 숫자
    /// </summary>
    int number = -1;

    /// <summary>
    /// 스프라이트 숫자 이미지
    /// </summary>
    public Sprite[] numberSprites;


    Sprite ZeroSprite => numberSprites[0];  // 0
    Sprite MinusSprite => numberSprites[11];    // -
    Sprite EmptySprite => numberSprites[10];    // 

    /// <summary>
    /// 자리수별 이미지 컴포넌트 (0 : 1자리, 1 : 10자리, 2 : 100자리)
    /// </summary>
    Image[] numberDigits = new Image[3]; 

    public int Number
    {
        get => number;
        set
        {
            // 앞자리 빈곳은 0으로 채움
            if(number != value)
            {
                number = Mathf.Clamp(value, -99, 999);  // 값의 범위는 -99 ~ 999
                Refresh();
            }
        }

    }


    private void Awake()
    {
        numberDigits = GetComponentsInChildren<Image>();
        
    }

    /// <summary>
    /// Number가 변경되었을 때 실행될 함수
    /// </summary>
    void Refresh()
    {
        int temp = Mathf.Abs(Number);           // 일단 양수로 처리

        Queue<int> digits = new Queue<int>(3);  // temp 자리수별로 자른 수를 저장할 큐
        
        // 자리수별로 Number를 나누어서 digits에 담기
        while(temp > 0)
        {
            digits.Enqueue(temp % 10);  // %연산으로 마지막 자리 잘라내기
            temp /= 10;                 // 잘라낸 부분 제거
        }

        // digits에 저장된 데이터를 기반으로 이미지 표시
        int index = 0;
        while(digits.Count > 0)
        {
            int num = digits.Dequeue();                         // 큐에서 하나씩 꺼낸후
            numberDigits[index++].sprite = numberSprites[num];  // 스프라이트 설정
        }

        // 남은 칸에 0으로 이미지 설정하기
        for (int i = index; i < numberDigits.Length; i++)
        {
            numberDigits[i].sprite = ZeroSprite;    // 빈칸은 무조건 0
        }

        // 원래 음수였으면
        if (Number < 0)
        {
            numberDigits[numberDigits.Length- 1].sprite = MinusSprite;  // 앞에 -를 붙힘
        }
    }


}
