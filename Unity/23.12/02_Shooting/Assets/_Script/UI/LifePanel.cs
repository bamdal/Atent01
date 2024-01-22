using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifePanel : MonoBehaviour
{
    // 플레이어 생명 표시용 패널

    Image[] lifeImages;

    public Color disableColor;

    private void Awake()
    {
        //lifeImages = GetComponentsInChildren<Image>();
        lifeImages = new Image[transform.childCount];
        for(int i = 0;i<transform.childCount;i++)
        {
            Transform child = transform.GetChild(i);
            lifeImages[i] = child.GetComponent<Image>();
        }

    }

    void OnEnable()
    {
        Player player = GameManager.Instance.Player;
        if (player != null)
        {
            player.onLifeChange += OnLifeChange;
        }
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            Player player = GameManager.Instance.Player;
            player.onLifeChange -= OnLifeChange;
        }

        
    }

    private void OnLifeChange(int life)
    {
        // 플렝이어의 생명수치에 따라 표시변경
        // 날아간 생명은 반투명한 회색으로 표시하기
        // lifeImages[0].color
/*        switch (life)
        {
            case 2:
                lifeImages[3].color = new Color(0.4f,0.4f,0.4f);
                break;
            case 1:
                lifeImages[2].color = new Color(0.4f, 0.4f, 0.4f);
                break;
            case 0:
                lifeImages[1].color = new Color(0.4f, 0.4f, 0.4f);
                break;
            default:
                break;
        }*/

        for(int i = 0; i<life;i++)
        {
            lifeImages[i].color = Color.white;
        }
        for(int i = life; i<lifeImages.Length;i++)
        {
            lifeImages[i].color = disableColor;
        }
    }
}
