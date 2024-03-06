using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageNumber : MonoBehaviour
{
    public Sprite[] numberImage;

    Image[] digits;

    /// <summary>
    /// 목표값
    /// </summary>
    public int number = 5655;

    public int Number
    {
        get => number;
        set
        {
            if (number != value)
            {
                number = Mathf.Min(value, 99999);   // 최대 5자리로 숫자 설정

                int temp = number;                  // 임시 변수에 number복사
                for (int i = 0; i < digits.Length; i++) 
                {

                    if (temp != 0 || i == 0)                    // temp가 0이 아니면 처리
                    {
                        int digit = temp % 10;                  // 1자리 숫자 추출하기
                        digits[i].sprite = numberImage[digit];  // 추출한 숫자에 맞게 이미지 선택
                        digits[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        digits[i].gameObject.SetActive(false);  // temp가 0이면 그 자리수는 안보이게 만들기(1자리 제외)

                    }
                    temp /= 10;                                 // 1자리수 제거하기

                }


            }
        }
    }


    private void Awake()
    {
        digits = GetComponentsInChildren<Image>(true);
    }



    private void Update()
    {

/*
        int tempNumber = Number;

        for (int i = 0; i < digits.Length; i++)
        {
            bool isActive = tempNumber > 0;
            digits[i].gameObject.SetActive(isActive);
            digits[i].sprite = numberImage[tempNumber % 10];

            tempNumber /= 10;
        }
*/
        // number는 5자리까지 표현 최대(99999)
        // number에 값을 세팅하면 digits에 적절한 이미지 세팅
        // 사용되지않는 자리는 disable
    }
}
