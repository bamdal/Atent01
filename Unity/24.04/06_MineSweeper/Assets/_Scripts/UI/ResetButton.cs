using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResetButton : MonoBehaviour
{
    enum ButtonState
    {
        Nomal = 0,
        Surprise,
        GameClear,
        GameOver
    };

    ButtonState state = ButtonState.Nomal;

    ButtonState State
    {
        get => state;
        set
        {
            if (state != value)
            {
                state = value;
                button.image.sprite = buttonSprite[(int)state];

            }
        }
    }

    Button button;
    public Sprite[] buttonSprite;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    private void Start()
    {
        button.onClick.AddListener(OnClick);
        GameManager gameManager = GameManager.Instance;
        gameManager.Board.onBoardLeftPress += () => 
        {
            State = ButtonState.Surprise;
        };
        gameManager.Board.onBoardLeftRelease += () =>
        {
            State = ButtonState.Nomal;
        };

        gameManager.onGameOver += () => State = ButtonState.GameOver;
        gameManager.onGameClear += () => State = ButtonState.GameClear;

    }


    private void OnClick()
    {
        State = ButtonState.Nomal;
        GameManager.Instance.GameReset();
    }
}

// 셀을 누르면 리셋버튼의 이미지가 surprise로 
// 게임 오버면 gameover로
// 클리어면 clear로 