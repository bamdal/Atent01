using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundStar : Background
{
    // 실습
    // MoveRight가 실행될때마다 Sprite Renderer의 filp을 랜덤으로 실행

    /*    public SpriteRenderer[] SpriteRenderer;
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
        }*/

    SpriteRenderer[] spriteRenderer;

    protected override void Awake()
    {
        base.Awake();

        spriteRenderer = /*GetComponentInChildren<SpriteRenderer>();*/
        GetComponentsInChildren<SpriteRenderer>();
    }

    protected override void MoveRight(int index)
    {
        base.MoveRight(index);
        // C#에서 숫자앞에 0b_ 를 붙이면 2진수
        // C#에서 숫자앞에 0x_를 붙이면 16진수

        int rand = Random.Range(0, 4); // 랜덤 결과 0~3

        // 0(0b_00), 1(0b_01), 2(0b_10), 3(0b_11)

        spriteRenderer[index].flipX = (rand & 0b_01) != 0; // rand 의 첫번째 비트가 1이면 true 아니면 false
        spriteRenderer[index].flipY = (rand & 0b_10) != 0; // rand 의 두번째 비트가 1이면 true 아니면 false
    }
}
