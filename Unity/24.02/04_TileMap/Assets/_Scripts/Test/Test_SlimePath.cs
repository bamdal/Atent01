using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class Test_SlimePath : TestBase
{
    public Tilemap background;
    public Tilemap obstacle;
    public Slime[] slime;

    TileGridMap map;

    private void Start()
    {
        map = new TileGridMap(background, obstacle);
        foreach (Slime sl in slime)
        {
            sl.Initialize(map, sl.transform.position);

        }
    }

    protected override void OnTestLClick(InputAction.CallbackContext _)
    {
        //Mouse;
        //Keyboard;
        //Gamepad.current;
        Vector2 sceenPos =  Mouse.current.position.ReadValue();
        Vector3 WorldPos = Camera.main.ScreenToWorldPoint(sceenPos);
        Vector2Int gridPos = map.WorldToGrid(WorldPos);

        if (map.IsValidPosition(gridPos) && map.IsPlain(gridPos))
        {
            foreach (Slime sl in slime)
            {
                sl.SetDestination(gridPos);

            }
            
        }
    }

}

// 1. 슬라임이 목적지에 도착하면 새로운 목적지를 랜덤으로 설정한다.
// 2. 페이즈나 디졸브 도중엔 움직이지 않아야한다.
