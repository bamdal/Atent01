using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    CinemachineVirtualCamera followCamara;

    public CinemachineVirtualCamera FollowCamara => followCamara;

    /// <summary>
    /// 적 스포너
    /// </summary>
    EnemySpawner enemySpawner;
    public EnemySpawner Spawner => enemySpawner;

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

    /// <summary>
    /// 킬카운트
    /// </summary>
    int killCount = 0;

    /// <summary>
    /// 플레이 타임
    /// </summary>
    float playTime = 0.0f;

    /// <summary>
    /// 게임시작을 알리는 델리게이트
    /// </summary>
    public Action onGameStart;

    /// <summary>
    /// 게임 종료를 알리는 델리게이트(true면 게임 클리어 false면 게임오버)
    /// </summary>
    public Action<bool> onGameEnd;

    protected override void OnInitialize()
    {
        onGameStart = null;
        onGameEnd = null;
        player = FindAnyObjectByType<Player>();
        player.onDie += GameOver;
/*        Vector3 centerPos = MazeVisualizer.GridToWorld(MazeWidth / 2, MazeHeight / 2);
        player.transform.position = centerPos;*/

        MinimapCamera minimapCamera = FindAnyObjectByType<MinimapCamera>();
        minimapCamera.Initialize(player);

        LoadingScreen loadingScreen = FindAnyObjectByType<LoadingScreen>();
        loadingScreen.Initialized();


        followCamara = GameObject.FindWithTag("FollowCamera").GetComponent<CinemachineVirtualCamera>();

        enemySpawner = FindAnyObjectByType<EnemySpawner>();
        enemySpawner.onSpawnCompleted += () =>
        {
            loadingScreen.OnLoadingProgress(1.0f);
            player.Spawn();
        };


        mazeGenerator = FindAnyObjectByType<MazeGenerator>();


        if(mazeGenerator != null)
        {

            mazeGenerator.Generate(MazeWidth, MazeHeight);
            mazeGenerator.onMazeGenerated += () =>
            {
                loadingScreen.OnLoadingProgress(0.7f);

                // 적 스폰
                Spawner?.EnemyAll_Spawn();


                playTime = 0;   // 플레이 시간 초기화
                killCount = 0;
            };
        }

        ResultPanel resultPanel = FindAnyObjectByType<ResultPanel>();
        resultPanel.gameObject.SetActive(false);
        Crosshair crosshair = FindAnyObjectByType<Crosshair>();
        // Time.timeSinceLevelLoad 씬이 로딩되고 지난 시간
        onGameEnd += (isClear) => 
        {
            
            crosshair.gameObject.SetActive(false);
            resultPanel.Open(isClear, killCount, playTime);
         
            

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

    /// <summary>
    /// 게임 시작시 실행될 함수
    /// </summary>
    public void GameStart()
    {
        onGameStart?.Invoke();
    }

    /// <summary>
    /// 게임클리어시 호출 될 함수
    /// </summary>
    public void GameClear()
    {
        onGameEnd?.Invoke(true);
    }

    /// <summary>
    /// 게임 오버함수
    /// </summary>
    public void GameOver()
    {
        onGameEnd?.Invoke(false);
    }
}
