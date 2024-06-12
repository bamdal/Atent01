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
    int mazeWidth = 20;
    int mazeHeight = 20;


    public int MazeWidth => mazeWidth;


    public int MazeHeight => mazeHeight;


    /// <summary>
    /// 미로 생성기
    /// </summary>
    MazeGenerator mazeGenerator;

    public Maze Maze => mazeGenerator.Maze;

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
            };
        }
    }


}
