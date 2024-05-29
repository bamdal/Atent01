using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    /// <summary>
    /// 유니티가 제공하는 시작용 코드(입력처리용 함수를 모은 클래스)
    /// </summary>
    StarterAssetsInputs starterAssets;

    /// <summary>
    /// 총만 촬영하는 카메라가 있는 게임 오브젝트
    /// </summary>
    GameObject gunCamera;

    /// <summary>
    /// 총알이 발사될 카메라 기준점
    /// </summary>
    public Transform FireTransform => transform.GetChild(0);    // 카메라 루트
    private void Awake()
    {
        starterAssets = GetComponent<StarterAssetsInputs>();

        gunCamera = transform.GetChild(2).gameObject;
    }

    private void Start()
    {
        starterAssets.onZoom += DisableGunCamera;
    }
    /// <summary>
    /// 총표시하는 카메라 활성화 설정
    /// </summary>
    /// <param name="enable">true면 총이 안보인다, false면 총이 보인다</param>
    void DisableGunCamera(bool disable = true)
    {
        gunCamera.SetActive(!disable);
    }
}
