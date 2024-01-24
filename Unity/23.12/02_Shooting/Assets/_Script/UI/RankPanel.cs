using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class RankPanel : MonoBehaviour
{
    /// <summary>
    /// 패널에서 표시되는 랭크 한 줄
    /// </summary>
    RankLine[] rankLines;

    /// <summary>
    /// 최고 득점(1등~5등)
    /// </summary>
    int[] highScores;

    /// <summary>
    /// 최고 득점자 이름(1등~5등)
    /// </summary>
    string[] rankerNames;

    /// <summary>
    /// 여기서 표시할 랭크 수
    /// </summary>
    const int rankCount = 5;

    /// <summary>
    /// 이름 입력을 받기 위한 인풋 필드
    /// </summary>
    TMP_InputField inputField;

    /// <summary>
    /// 갱신된 랭킹의 인덱스
    /// </summary>
    int updatedIndex = -1;


    private void Awake()
    {
        rankLines = GetComponentsInChildren<RankLine>();
        highScores = new int[rankCount];
        rankerNames = new string[rankCount];
        inputField = GetComponentInChildren<TMP_InputField>(true);  // true로 비활성화된것도 찾기
        inputField.onEndEdit.AddListener(OnNameInputEnd);

    }

    private void Start()
    {
        GameManager.Instance.Player.onDie += UpdateRankData;

        LoadRankdata();
    }

    // 랭킹을 파일로 저장할 수 있어야 한다.
    // 랭킹을 세이브파일에서 불러오기
    // 랭킹 업데이트
    // 랭킹보드 초기화


    /// <summary>
    /// 랭킹 데이터를 초기값으로 모두 설정하는 함수
    /// </summary>
    void SetDefaultData()
    {
       for(int i = 0; i < rankCount; i++)
        {
            /*            highScores[i] = (int)(1000000/Mathf.Pow(10,i));
                        rankerNames[i] =$"{((char)(65 + i)).ToString()}{((char)(65 + i)).ToString()}{((char)(65 + i)).ToString()}" ;*/

            char temp = 'A';
            temp = (char)((byte)temp + (byte)i);
            rankerNames[i] = $"{temp}{temp}{temp}";

            int score = 10;
            for(int j=rankCount-i;j>0;j--)
            {
                score *= 10;
            }
            highScores[i] = score;
        }

        // 1st AAA 1000000
        // 2nd BBB 100000
        // 3rd CCC 10000
        // 4th DDD 1000
        // 5th EEE 100

        RefreshRankLines(); // UI 갱신
    }

    /// <summary>
    /// 랭킹 데이터를 파일에 저장하는 함수
    /// </summary>
    void SaveRankData()
    {
        SaveData data = new SaveData();             // 저장용 클래스 인스턴스 만들기
        data.rankerNames = rankerNames;             // 저장용 객체에 데이터 넣기
        data.highScores = highScores;
        string jsonText = JsonUtility.ToJson(data); // 저장용 객체의 내용을 json형식의 문자열로 변경

        string path = $"{Application.dataPath}/Save/";
        //string path2 = @"\Save";
        if(!Directory.Exists(path)) // Exists : true면 폴더가 있다. false면 폴더가 없다.
        {
            //path 폴더가 없다.
            Directory.CreateDirectory(path);    // path에 지정된 폴더를 만든다.
        }

        /*        string fullpath = $"{path}Save.txt";
                File.WriteAllText(fullpath, "Test저장");
        */
        // json
        string fullpath = $"{path}Save.json";   // 전체 경로 만들기
        File.WriteAllText(fullpath,jsonText);   // 파일로 저장
    }

    /// <summary>
    /// 저장된 랭킹 데이터를 불러오는 함수
    /// </summary>
    /// <returns>성공여부(true면 성공, false면 실패)</returns>
    bool LoadRankdata()
    {
        bool result = false;

        string path = $"{Application.dataPath}/Save/";
        if (Directory.Exists(path)) // Exists : true면 폴더가 있다. false면 폴더가 없다.
        {
            string fullpath = $"{path}Save.json";   // 전체 경로 만들기
            if (File.Exists(fullpath))
            {
                string json = File.ReadAllText(fullpath);
                SaveData loadData = JsonUtility.FromJson<SaveData>(json);   

                highScores = loadData.highScores;
                rankerNames = loadData.rankerNames;

                result = true;
            }
        }

        if (!result)    // 로딩 실패
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            SetDefaultData();   // 기본 데이터 설정;
            
        }

        RefreshRankLines();

        return result;
    }

    /// <summary>
    /// 랭킹 데이터를 업데이트하는 함수
    /// </summary>
    /// <param name="score">새 점수</param>
    void UpdateRankData(int score)
    {
        for(int i = 0; i<rankCount; i++)
        {
            if (highScores[i] < score)  // i번째 등수에 끼어 들면 된다.
            {
                for(int j = rankCount-1;j>i;j--)
                {
                    highScores[j] = highScores[j-1];
                    rankerNames[j]= rankerNames[j-1];
                    rankLines[j].SetData(rankerNames[j], highScores[j]);
                }
                highScores[i] = score;                              // 점수 기록
                
                rankLines[i].SetData("새 랭커", score);              // 
                updatedIndex = i;                                   // 업데이트된 인덱스

                Vector3 newPos = inputField.transform.position;     // 인풋필드의 위치값
                newPos.y = rankLines[i].transform.position.y;       // 이동해야할 Y값
                inputField.transform.position = newPos;             // 인풋필드 이동

                inputField.gameObject.SetActive(true);              // 인풋필드 활성화
                inputField.Select();
                break;
            }
        }
    }

    /// <summary>
    /// 현재 설정된 랭킹 데이터에 맞게 UI 갱신
    /// </summary>
    void RefreshRankLines()
    {
        for (int i = 0;i < rankCount; i++)
        {
            rankLines[i].SetData(rankerNames[i], highScores[i]);
        }
    }

    /// <summary>
    /// 인풋필드에 입력이 끝났을 때 실행될 함수
    /// </summary>
    /// <param name="text">인풋 필드에 기록되어 있는 문자열</param>
    private void OnNameInputEnd(string text)
    {
        inputField.gameObject.SetActive(false); // 입력 완료되었으니 인풋필드 안보이게 만들기
        if (text == "")
            text = "AAA";
        rankerNames[updatedIndex] = text;       // 랭커 이름 설정
        RefreshRankLines() ;                    // UI 갱신
        SaveRankData();                         // 저장
    }

#if UNITY_EDITOR
    public void Test_DefaultRankPanel()
    {
        SetDefaultData();
        RefreshRankLines();
    }

    public void Test_SaveRankData()
    {
        SetDefaultData();
        RefreshRankLines();
        SaveRankData();
    }

    public void Test_LoadRankPanel()
    {
        LoadRankdata();
    }

    public void Test_UpdateRankPanel(int score)
    {
        UpdateRankData(score);
    }
#endif
}
