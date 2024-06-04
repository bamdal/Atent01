using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.IO;
using System.Diagnostics;

public class Ranking_장민수 : MonoBehaviour
{
    readonly string url = "https://atentsexample.azurewebsites.net/rankdata";
    RankLine[] rankLines;
    Button rankGetButton;



    private void Start()
    {
        rankLines = GetComponentsInChildren<RankLine>();
        rankGetButton = transform.parent.GetChild(1).GetComponent<Button>();
        rankGetButton.onClick.AddListener(OnRankGetClick);
    }

    private void OnRankGetClick()
    {
        StartCoroutine(GetData());
    }
   
    IEnumerator GetData()
    {
        // using : C#에서 리소스 관리를 위해 사용된다
        // 특정 리소스를 사용한 후에 자동으로 정리(Clean-up)
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();
            RankData rankData;
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("서버에서 가져오지 못함");

                // json파일 찾기
                string jsonString = "";
                if (File.Exists("Assets/RankData/RankData.json"))
                {
                    jsonString = File.ReadAllText("Assets/RankData/RankData.json");
                    rankData = JsonUtility.FromJson<RankData>(jsonString);

                }
                else
                {
                    Debug.Log("Json파일이 없다");
                    yield break;
                }
            }
            else
            {
                // 서버에서 가져오는데 성공
                rankData = JsonUtility.FromJson<RankData>(www.downloadHandler.text);
                Debug.Log("서버에서 파일을 읽어오는데 성공");
            }

            int min = Mathf.min(rankLines.Length, rankData.rankerName.Length, rankData.highScore.Length);
            // 화면에 출력
            for (int i = 0; i < min; i++)
            {
                rankLines[i].SetRankerName(rankData.rankerName[i]);
                rankLines[i].SetHighScore(rankData.highScore[i]);
            }
        }

    }
}
