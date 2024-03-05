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
                number = value;
                number = Mathf.Clamp(number, 0, 99999);

            }
        }
    }


    private void Awake()
    {
        digits = GetComponentsInChildren<Image>(true);
    }

    private void Update()
    {


        int tempNumber = Number;

        for (int i = 0; i < digits.Length; i++)
        {
            bool isActive = tempNumber > 0;
            digits[i].gameObject.SetActive(isActive);
            digits[i].sprite = numberImage[tempNumber % 10];

            tempNumber /= 10;
        }

        // number는 5자리까지 표현 최대(99999)
        // number에 값을 세팅하면 digits에 적절한 이미지 세팅
        // 사용되지않는 자리는 disable
    }
}
