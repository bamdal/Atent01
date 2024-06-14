using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    CinemachineVirtualCamera followCamara;

    public CinemachineVirtualCamera FollowCamara => followCamara;

    Player player;

    public Player Player
    {
        get
        {
            if (player == null)
            {
                player = FindAnyObjectByType<Player>();
            }
            return player;
        }
    }

    /// <summary>
    /// 시작시 생성용 미로 크기
    /// </summary>
    public int mazeWidth = 20;
    public int mazeHeight = 20;


    public int MazeWidth => mazeWidth;


    public int MazeHeight => mazeHeight;


    /// <summary>
    /// 미로 생성기
    /// </summary>
    MazeGenerator mazeGenerator;

    public Maze Maze => mazeGenerator.Maze;

    int killCount = 0;
    float playTime = 0.0f;

    protected override void OnInitialize()
    {
        player = FindAnyObjectByType<Player>();
        followCamara = GameObject.FindWithTag("FollowCamera").GetComponent<CinemachineVirtualCamera>();

        mazeGenerator = FindAnyObjectByType<MazeGenerator>();
        if(mazeGenerator != null)
        {

            mazeGenerator.Generate(MazeWidth, MazeHeight);
            mazeGenerator.onMazeGenerated += () =>
            {
                Vector3 centerPos = MazeVisualizer.GridToWorld(mazeWidth / 2, mazeHeight / 2);
                player.transform.position = centerPos;

                playTime = 0;   // 플레이 시간 초기화
                killCount = 0;
            };
        }

        Goal goal = FindAnyObjectByType<Goal>();
        ResultPanel resultPanel = FindAnyObjectByType<ResultPanel>();
        resultPanel.gameObject.SetActive(false);
        Crosshair crosshair = FindAnyObjectByType<Crosshair>();
        // Time.timeSinceLevelLoad 씬이 로딩되고 지난 시간
        goal.onGameClear += () => 
        {
            crosshair.gameObject.SetActive(false);
            resultPanel.Open(true, killCount, playTime);
            player.InputDisable();

        };

        Cursor.lockState = CursorLockMode.Locked;
    }

    public void IncreaseKillCount()
    {
        killCount++;
    }

    private void Update()
    {
        playTime += Time.deltaTime;
    }
}
