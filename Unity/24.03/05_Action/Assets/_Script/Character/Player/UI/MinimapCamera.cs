using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    //플레이어를 따라다니는 느낌으로 작성하기
    Player player;
    Vector3 offset;
    public float smoothness = 3.0f;
    private void Start()
    {
        player = GameManager.Instance.Player;
        transform.position = player.transform.position+ Vector3.up*30;
        offset = transform.position - player.transform.position;
    }

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position,player.transform.position+offset,smoothness);
    }
}
