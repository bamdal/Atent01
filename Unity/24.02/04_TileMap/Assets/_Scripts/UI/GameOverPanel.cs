using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverPanel : MonoBehaviour
{
    TextMeshProUGUI playTime;
    TextMeshProUGUI killCount;
    public float alphaChageSpeed = 1.0f;

    Button restart;

    CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        Transform child = transform.GetChild(1);
        playTime = child.GetComponent<TextMeshProUGUI>();
        child = transform.GetChild(2);
        killCount = child.GetComponent<TextMeshProUGUI>();
        child = transform.GetChild(3);
        restart = child.GetComponent<Button>();
    }

    private void Start()
    {
        canvasGroup.alpha = 0.0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        Player player = GameManager.Instance.Player;
        player.onDie += OnPlayerDie;

        restart.onClick.AddListener(() =>
        {
            StartCoroutine(WaitUnloadAll());   
        });
       
    }

    private void OnPlayerDie(float totalPlayTime, int totalKillCount)
    {
        playTime.text = $"Total Play Time \n\r<{totalPlayTime:f1}Sec>";
        killCount.text = $"Total Kill Count \n\r<{totalKillCount}Kill>";
        StartCoroutine(StartAlphaChange());
    }

    IEnumerator StartAlphaChange()
    {
        while(canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += Time.deltaTime * alphaChageSpeed;
            yield return null;
        }
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    IEnumerator WaitUnloadAll()
    {
        WorldManager world = GameManager.Instance.World;
        while (!world.IsUnloadAll)  // 플레이어가 죽었을 때 모든 맵을 로딩해제 요청을 하니까 다 될때 까지 대기
        {
            yield return null;

        }
        SceneManager.LoadScene("AsyncLoadingScene");
    }

}
