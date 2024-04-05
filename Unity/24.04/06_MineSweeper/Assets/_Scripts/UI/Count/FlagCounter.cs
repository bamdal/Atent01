using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagCounter : CounterBase
{


    void Start()
    {
        GameManager.Instance.onFlagCountChange += Refresh;
        Refresh(GameManager.Instance.FlagCount);
    }


}
