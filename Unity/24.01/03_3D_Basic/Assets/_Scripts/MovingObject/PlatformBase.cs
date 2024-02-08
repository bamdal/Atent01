using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformBase : WayPointUser
{
    public Action<Vector3> onMove;

    protected override void OnMove()
    {
        base.OnMove();
        onMove?.Invoke(moveDelta);
    }
}
