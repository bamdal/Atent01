using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Revolber : GunBase
{
    float reloadDuration => fireRateSecond;
    bool isReloading = false;

    protected override void FireProcess(bool isFireStart = true)
    {
        if(isFireStart)
        {
            base.FireProcess();
            HitProcess();       // 명중처리 후
            FireRecoil();       // 총기 반동 신호 보내기

        }
        

    }

    public void Reload()
    {
        if (!isReloading && (BulletCount < clipSize))   // 리로딩 아닐때 실행, 탄이 소보되면 실행
        {
            StopAllCoroutines();                // FireProcess에서 실행시키는 코루틴으로 isFireReady가 true가 되는 것 방지
            isReloading = true;                         // 리로딩 중
            isFireReady = false;                        // 총 발사 방지
            Debug.Log("리볼버재장전중");
            StartCoroutine(RelodingCoroutine());
        }
    }
    
    IEnumerator RelodingCoroutine()
    {
        yield return new WaitForSeconds(reloadDuration);    // 리로딩 시간만큼 대기
        BulletCount = clipSize; // 탄창크기만큼 재장전
        isReloading = false;    // 리로딩 끝남
        isFireReady = true;     // 총 발사 가능
        Debug.Log("리볼버장전완료");
    }
}
