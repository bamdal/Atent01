using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundStar : Background
{
    // 실습
    // MoveRight가 실행될때마다 Sprite Renderer의 filp을 랜덤으로 실행

    public SpriteRenderer[] SpriteRenderer;
    bool spriteFilp = true;

    protected override void Awake()
    {
        base.Awake();
        SpriteRenderer = new SpriteRenderer[transform.childCount];
        for (int i = 0; i < SpriteRenderer.Length; i++)
        {
            SpriteRenderer[i] = transform.GetChild(i).GetComponentInChildren<SpriteRenderer>();
           
        }
    }
    protected override void MoveRight(int index)
    {
        base.MoveRight(index);

        SpriteRenderer[index].flipX =Filp();
        SpriteRenderer[index].flipY= Filp();
    }

    bool Filp()
    {
        spriteFilp = Random.value > 0.5f;
        return spriteFilp;
    }
}
