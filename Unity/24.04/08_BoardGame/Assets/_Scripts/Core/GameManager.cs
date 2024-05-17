using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public enum GameState : byte
{
    Title = 0,      // 타이틀 씬
    ShipDeployment, // 함선 배치 씬
    Battle,         // 전투 씬
    GameEnd         // 전투씬에서 게임이 끝난 상황
}

[RequireComponent(typeof(TurnController))]
[RequireComponent(typeof(InputController))]
public class GameManager : Singleton<GameManager>
{

    // 게임 상태 ==============================================================================================================================

    /// <summary>
    /// 현재 게임 상태
    /// </summary>
    GameState gamestate = GameState.Title;

    /// <summary>
    /// 현재 게임 상태를 확인하고 설정하기 위한 프로퍼티
    /// </summary>
    public GameState GameState
    {
        get => gamestate;
        set
        {
            if (gamestate != value)                 // 변경이 있을 때만 실행
            {
                gamestate = value;
                InputController.ResetBind();    // 기존에 바인딩 되어 있던 입력 제거
                onStateChange?.Invoke(gamestate);   // 게임 상태가 변경됨을 알림 
            }

        }
    }

    /// <summary>
    /// 게임 변경 상태를 알리는 델리게이트
    /// </summary>
    public Action<GameState> onStateChange;

    // 플레이어 ==============================================================================================================================
    /// <summary>
    /// 사용자 플레이어(왼쪽)
    /// </summary>
    UserPlayer user;

    public UserPlayer UserPlayer => user;

    /// <summary>
    /// 적 플레이어(왼쪽)
    /// </summary>
    EnemyPlayer enemy;

    public EnemyPlayer EnemyPlayer => enemy;



    /// <summary>
    /// 테스트 모드인지 표시용
    /// </summary>
    public bool IsTestMode = false;

    // 컨트롤러 ==============================================================================================================================

    /// <summary>
    /// 턴 컨트롤러
    /// </summary>
    TurnController turnController;

    public TurnController TurnController => turnController;

    InputController inputController;

    public InputController InputController => inputController;

    // 기타 ==============================================================================================================================

    /// <summary>
    /// 카메라 진동 소스
    /// </summary>
    CinemachineImpulseSource cameraInpulseSource;
    protected override void OnPreInitialize()
    {
        base.OnPreInitialize();

        inputController = GetComponent<InputController>();

        turnController = GetComponent<TurnController>();

        cameraInpulseSource = GetComponentInChildren<CinemachineImpulseSource>();
    }

    protected override void OnInitialize()
    {
        user = FindAnyObjectByType<UserPlayer>();
        enemy = FindAnyObjectByType<EnemyPlayer>();
        turnController.OnInitialize(user, enemy);
    }

    /// <summary>
    /// 카메라를 흔드는 함수
    /// </summary>
    /// <param name="force">흔드는 힘의 크기</param>
    public void CameraShake(float force = 0.5f)
    {
        cameraInpulseSource.GenerateImpulseWithVelocity(force * UnityEngine.Random.insideUnitCircle.normalized);
    }

    // 저장 및 불러오기 -----------------------------------------------------------------------------------------

    /// <summary>
    /// 함선 배치 정보
    /// </summary>
    ShipDeployData[] shipDeployDatas;

    /// <summary>
    /// 함선 배치 정보 저장용 함수
    /// </summary>
    /// <returns>true면 저장 성공, false면 저장 실패</returns>
    public bool SaveShipDeployData()
    {
        bool result = false;
        Ship[] ships = user.Ships;
        if (user.IsAllDeployed) // 전부 배치되었을때만 저장
        {
            shipDeployDatas = new ShipDeployData[user.Ships.Length];
            for (int i = 0; i < shipDeployDatas.Length; i++)
            {
                shipDeployDatas[i] = new ShipDeployData(ships[i].Direction, ships[i].Positions[0]); // 정보 저장
            }
            result = true;
        }

        return result;
    }

    /// <summary>
    /// 함선 배치 로드용 함수
    /// </summary>
    /// <returns>true면 로드 성공, false면 로드 실패</returns>
    public bool LoadeShipDeployData()
    {
        bool result = false;


        if (shipDeployDatas != null)    // 이전 정보가 있다면 로드
        {
            user.UndoAllShipDeployment();
            Ship[] ships = user.Ships;
            for (int i = 0; i < shipDeployDatas.Length; i++)
            {
                ships[i].Direction = shipDeployDatas[i].Direction;
                if (!user.Board.ShipDeployment(ships[i], shipDeployDatas[i].Position))  // 저장된데로 배를 배치함
                {
                    return result;
                }
                ships[i].gameObject.SetActive(true);

            }
            result = true;

        }

        return result;
    }

}
