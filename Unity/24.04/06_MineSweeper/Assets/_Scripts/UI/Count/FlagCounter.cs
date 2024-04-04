using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagCounter : MonoBehaviour
{
    ImageNumber imageNumber;

    private void Awake()
    {
        imageNumber = GetComponent<ImageNumber>();
    }

    void Start()
    {
        GameManager.Instance.onFlagCountChange += Refresh;
        Refresh(GameManager.Instance.FlagCount);
    }

    private void Refresh(int count)
    {
        imageNumber.Number = count;
    }
}
