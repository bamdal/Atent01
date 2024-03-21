using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemRotator : MonoBehaviour
{
    public float rotateSpeed = 360.0f;
    public float moveSpeed = 2.0f;

    public float minHeight = 0.5f;
    public float maxHeight = 1.5f;

    float timeElapsed = 0.0f;

    private void Start()
    {
        transform.Rotate(0, Random.Range(0, 360), 0);
        transform.position = transform.parent.position + Vector3.up * minHeight;
    }

    private void Update()
    {
        timeElapsed += Time.deltaTime * moveSpeed;

        // cos() + 1 = 0~2
        // (cos() + 1) * 0.5 = 0~1
        // ((cos() + 1) * 0.5) * (max - min) = 0 ~ (max - min)
        // min + ((cos() + 1) * 0.5) * (max - min) = min ~ max

        Vector3 pos;
        pos.x = transform.position.x;
        pos.y = minHeight + (Mathf.Sin(timeElapsed)+1) * 0.5f * (maxHeight-minHeight);
        pos.z = transform.position.z;

        transform.position = pos;
        transform.Rotate(Time.deltaTime * rotateSpeed * Vector3.up);
        //transform.Translate(Mathf.Sin(Time.deltaTime) * )
    }

}
