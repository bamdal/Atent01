using StarterAssets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{

    /// <summary>
    /// 유니티가 제공하는 시작용 코드(입력처리용 함수를 모은 클래스)
    /// </summary>
    StarterAssetsInputs starterAssets;

    /// <summary>
    /// 유니티가 제공한 입력처리용 코드
    /// </summary>
    FirstPersonController controller;

    /// <summary>
    /// 플레이어 인풋 컨트롤러
    /// </summary>
    PlayerInput playerInput;

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

    public float MaxHP = 100.0f;

    float hp;

    public float HP
    {
        get => hp;
        set
        {
            hp = value;
            if (hp <= 0)
            {
                Die();
            }
            hp = Mathf.Clamp(hp, 0, MaxHP);
            onHPChange?.Invoke(hp);
        }
    }

    /// <summary>
    /// 플레이어가 맵의 가운데 배치되었을 때 실행될 델리게이트
    /// </summary>
    public Action onSpawn;

    /// <summary>
    /// HP 변경됨을 알리는 델리게이트
    /// </summary>
    public Action<float> onHPChange;

    /// <summary>
    /// 공격 받았을 때 실행될 델리게이트(float: 공격받은 각도)
    /// </summary>
    public Action<float> onAttacked;


    /// <summary>
    /// 총이 변경되었음을 알리는 델리게이트
    /// </summary>
    public Action<GunBase> onGunChange;

    /// <summary>
    /// 플레이어 사망 델리게이트
    /// </summary>
    public Action onDie;
    private void Awake()
    {
        starterAssets = GetComponent<StarterAssetsInputs>();
        controller = GetComponent<FirstPersonController>();
        playerInput = GetComponent<PlayerInput>();


        gunCamera = transform.GetChild(2).gameObject;

        Transform child = transform.GetChild(3);
        guns = child.GetComponentsInChildren<GunBase>(true);


        activeGun = guns[0];       // 기본총 설정
        
    }


    private void Start()
    {
        starterAssets.onZoom += DisableGunCamera;   // 줌 할 때 실행될 함수 연결

        Crosshair crosshair = FindAnyObjectByType<Canvas>().GetComponentInChildren<Crosshair>();
        foreach (GunBase gun in guns)
        {
            gun.onFire += controller.FireRecoil;
            gun.onFire += (expend) => { crosshair.Expend(expend * 10); };
            gun.onAmmoDepleted += () => 
            {
                if (!(activeGun is Revolber))
                    GunChage(GunType.Revolver);
            };
        }

        activeGun.Equip();
        activeGun.gameObject.SetActive(true);
        onGunChange?.Invoke(activeGun);
        HP = MaxHP;

        GameManager.Instance.onGameEnd += (_) =>InputDisable();

        Spawn();
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

        onGunChange?.Invoke(activeGun);
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
    public void AddAmmoCountChangeDelegate(Action<int> callback)
    {
        foreach(var gun in guns)
        {
            gun.onAmmoCountChange += callback;
        }
    }

    /// <summary>
    /// 공격받는 함수
    /// </summary>
    /// <param name="enemy">공격하는 적</param>
    public void OnAttacked(Enemy enemy)
    {
        Debug.Log(enemy + "가 공격함");
        HP -= enemy.attackPower;
        /*        float angle = Vector3.Angle(-transform.forward, enemy.transform.forward);    // 공격 당한 각도
                if (Vector3.Cross(transform.forward, enemy.transform.forward).y < 0)
                {
                    angle *= -1;
                }*/
        float angle = Vector3.SignedAngle(-transform.forward, enemy.transform.forward, Vector3.down);
        Debug.Log(angle);
        onAttacked?.Invoke(angle);
    }

    /// <summary>
    /// 플레이어를 맵에 배치시키는 함수
    /// </summary>
    public void Spawn()
    {
        GameManager gameManager = GameManager.Instance;
        Vector3 centerPos = MazeVisualizer.GridToWorld(gameManager.MazeWidth / 2, gameManager.MazeHeight / 2);
        transform.position = centerPos;  // 플레이어를 미로의 가운데 위치로 옮기기
        onSpawn?.Invoke();
    }

    /// <summary>
    /// 플레이어 사망
    /// </summary>
    void Die()
    {
        onDie?.Invoke();                // 죽었음을 알림
        gameObject.SetActive(false);    // 플레이어 오브젝트 비활성화
    }

    /// <summary>
    /// 입력을 막는 함수
    /// </summary>
    private void InputDisable()
    {
        playerInput.actions.actionMaps[0].Disable();    // 액션맵이 1개만 있기 때문에 적용
        /*    controller.enabled = false;
            starterAssets.enabled = false;
        */

    /*     InputActionMap playerActionMap = playerInput.actions.FindActionMap("Player");
        playerActionMap.Disable();*/
    }
}
