using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillCount : MonoBehaviour
{
    ImageNumber imageNumber;

    float target = 0.0f;
    float current = 0.0f;

    public float countingSpeed = 15.0f;


    private void Awake()
    {
        imageNumber = GetComponent<ImageNumber>();
    }

    private void Start()
    {
        Player player = GameManager.Instance.Player;
        player.onKillCountChange += OnKillCountChange;

    }

    private void Update()
    {
        current += Time.deltaTime * countingSpeed; // current는 target까지 지속적으로 증가
 
        if (current > target)
        {

            current = target;   // 넘치는것 방지
        }

        imageNumber.Number = Mathf.FloorToInt(current);
        
    }

    private void OnKillCountChange(int count)
    {
        target = count;
    }


}
