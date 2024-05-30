using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : GunBase
{

    public int pellet = 6;

    protected override void FireProcess(bool isFireStart = true)
    {
        if (isFireStart)
        {
            base.FireProcess(isFireStart);
            for (int i = 0; i < 10; i++)
            {
                HitProcess();       // 여러번 hit처리

            }

            FireRecoil();       // 총기 반동 신호 보내기
        }


    }


}
