using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BulletCount : MonoBehaviour
{
    // 총쏠때마다 남은 총알 개수 갱신
    // 총 바뀔때마다 갱신
    TextMeshProUGUI current;
    TextMeshProUGUI max;


    private void Awake()
    {
        current = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        max = transform.GetChild(2).GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        Player player = GameManager.Instance.Player;
        player.AddBulletCountChangeDelegate(OnBulletCountChange);
        player.onClipSizeChange += OnGunChange;
    }

    private void OnGunChange(GunBase activeGun)
    {
        max.text = activeGun.clipSize.ToString();
    }

    private void OnBulletCountChange(int count)
    {
        current.text = count.ToString();
    }
}
