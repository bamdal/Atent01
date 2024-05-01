using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    CinemachineImpulseSource cameraInpulseSource;

    Board board;

    public Board Board
    {
        get => board;
    }


    protected override void OnPreInitialize()
    {
        base.OnPreInitialize();
        board = FindAnyObjectByType<Board>();
        cameraInpulseSource = GetComponentInChildren<CinemachineImpulseSource>();
    }

    public void CameraShake(float force)
    {
        cameraInpulseSource.GenerateImpulseWithVelocity(force * Random.insideUnitCircle.normalized);
    }

   
}
