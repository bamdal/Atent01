using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VirtualButton : MonoBehaviour, IPointerClickHandler
{
    public Action onClick;
    Image coolDown;


    private void Awake()
    {
        Transform child = transform.GetChild(1);
        coolDown = child.GetComponent<Image>();
        coolDown.fillAmount = 0.0f;
        GameManager.Instance.Player.onJumpCoolTimeChange += RefreshCoolTime;
    }



    public void OnPointerClick(PointerEventData eventData)
    {
        onClick?.Invoke();
    }

    public void RefreshCoolTime(float ratio)
    {
        coolDown.fillAmount = ratio;
    }

    public void Stop()
    {
        onClick = null;
        coolDown.fillAmount = 1.0f;
    }
}
