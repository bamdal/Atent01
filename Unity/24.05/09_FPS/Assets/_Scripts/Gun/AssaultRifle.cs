using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssaultRifle : GunBase
{
    float reloadDuration => fireRateSecond;
    protected override void FireProcess(bool isFireStart = true)
    {
        if (isFireStart)
        {
            StartCoroutine(FireRepeat());
        }
        else
        { 
            StopAllCoroutines();
            isFireReady = true;
        }

    }

    IEnumerator FireRepeat()
    {
        while(BulletCount > 0)  // 총알이 남았으면 반복
        {
            MuzzleEffectOn();   // 머즐 이펙트 키기
            BulletCount--;      // 총알 개수 줄이기

            HitProcess();       // 명중처리 후
            FireRecoil();       // 총기 반동 신호 보내기

            yield return new WaitForSeconds(reloadDuration);    // 발사 간격만큼 대기
        }

        isFireReady = true;
    }
}
