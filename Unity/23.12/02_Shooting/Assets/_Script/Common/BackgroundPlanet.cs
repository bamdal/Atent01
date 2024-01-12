using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundPlanet : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    float minRightEnd = 30.0f;
    float maxRightEnd = 60.0f;
    float minY = -8.0f;
    float maxY = -5.0f;

    float baseLineX;

    private void Start()
    {
        baseLineX = transform.position.x; // 기준선은 초기 위치 x 값으로
    }

    private void Update()
    {
        transform.Translate(Time.deltaTime * moveSpeed * -transform.right); // 왼쪽 이동하다

        if (transform.position.x < baseLineX) // 기준선보다 왼쪽으로 가면
        {
            transform.position = new Vector3(Random.Range(minRightEnd, maxRightEnd), Random.Range(minY, maxY), 0.0f);
            // 오른쪽에 최대범위에 지정된랜덤위치로 이동
        }
    }
}
