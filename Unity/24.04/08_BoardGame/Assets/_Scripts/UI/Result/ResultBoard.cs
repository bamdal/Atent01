using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultBoard : MonoBehaviour
{
    public Material victory;
    public Material defeat;

    TextMeshProUGUI resultText;

    ResultAnalysis[] resultAnalyses;
    private void Awake()
    {
        resultText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        resultAnalyses = GetComponentsInChildren<ResultAnalysis>();
    }

    /// <summary>
    /// 열고 닫기용 토글
    /// </summary>
    public void Toggle()
    {
        // board여닫기
        gameObject.SetActive(!gameObject.activeSelf);
        

    }

    /// <summary>
    /// 승리/패배시 글자와 머티리얼 수정
    /// </summary>
    /// <param name="isVictory">true승리, false패배</param>
    public void SetVictoryDefeat(bool isVictory)
    {
        if (isVictory)
        {
            resultText.text = "승리!";
            resultText.fontMaterial = victory;

        }else
        {
            resultText.text = "패배!";
            resultText.fontMaterial = defeat;
        }


    }
}
