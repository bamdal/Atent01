using StarterAssets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    /// <summary>
    /// 유니티가 제공하는 시작용 코드(입력처리용 함수를 모은 클래스)
    /// </summary>
    StarterAssetsInputs starterAssets;

    FirstPersonController controller;

    /// <summary>
    /// 총만 촬영하는 카메라가 있는 게임 오브젝트
    /// </summary>
    GameObject gunCamera;

    /// <summary>
    /// 총알이 발사될 카메라 기준점
    /// </summary>
    public Transform FireTransform => transform.GetChild(0);    // 카메라 루트

    /// <summary>
    /// 플레이어가 장비할 수 있는 모든 총
    /// </summary>
    GunBase[] guns;

    /// <summary>
    /// 현재 장비하고 있는 총
    /// </summary>
    GunBase activeGun;

    /// <summary>
    /// 기본 설정 총(리볼버)
    /// </summary>
    GunBase defaultGun;

    /// <summary>
    /// 총이 변경되었음을 알리는 델리게이트
    /// </summary>
    public Action<GunBase> onClipSizeChange;

    private void Awake()
    {
        starterAssets = GetComponent<StarterAssetsInputs>();
        controller = GetComponent<FirstPersonController>();

        gunCamera = transform.GetChild(2).gameObject;

        Transform child = transform.GetChild(3);
        guns = child.GetComponentsInChildren<GunBase>(true);


        defaultGun = guns[0];       // 기본총 설정
        activeGun = defaultGun;     // 기본총 활성화

    }


    private void Start()
    {
        starterAssets.onZoom += DisableGunCamera;   // 줌 할 때 실행될 함수 연결

        Crosshair crosshair = FindAnyObjectByType<Canvas>().GetComponentInChildren<Crosshair>();
        foreach (GunBase gun in guns)
        {
            gun.onFire += controller.FireRecoil;
            gun.onFire += (expend) => { crosshair.Expend(expend * 10); };
        }

        activeGun.Equip();
        activeGun.gameObject.SetActive(true);
        onClipSizeChange?.Invoke(activeGun);
    }
    /// <summary>
    /// 총표시하는 카메라 활성화 설정
    /// </summary>
    /// <param name="enable">true면 총이 안보인다, false면 총이 보인다</param>
    void DisableGunCamera(bool disable = true)
    {
        gunCamera.SetActive(!disable);
    }

    /// <summary>
    /// 장비중인 총을 변경하는 함수
    /// </summary>
    /// <param name="gunType">바꿀총의 타입</param>
    public void GunChage(GunType gunType)
    {
        activeGun.gameObject.SetActive(false);  // 이전총 비활성화
        activeGun.UnEquip();                    // 이전총 장비 해제

        activeGun = guns[(int)gunType];         // 타입에 맞는 총 설정
        activeGun.Equip();                      // 총 장비
        activeGun.gameObject.SetActive(true);   // 총 보이기

        onClipSizeChange?.Invoke(activeGun);
    }

    /// <summary>
    /// 장비중인 총을 발사하는 함수
    /// </summary>
    /// <param name="isFireStart">연사 여부</param>
    public void GunFire(bool isFireStart = true)
    {
        activeGun.Fire(isFireStart);
    }

    /// <summary>
    /// 리볼버를 재장전 하는 함수
    /// </summary>
    public void RevolverReload()
    {
        Revolber revolber = activeGun as Revolber;
        if (revolber != null)   // activeGun이 리볼버일때만 장전
        {
            revolber.Reload();
        }
    }

    /// <summary>
    /// 총알개수가 변경될 때 실행되는 델리게이트 콜백 함수 추가
    /// </summary>
    /// <param name="callback">추가할 콜백함수</param>
    public void AddBulletCountChangeDelegate(Action<int> callback)
    {
        foreach(var gun in guns)
        {
            gun.onBulletCountChange += callback;
        }
    }
}
