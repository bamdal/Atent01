using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    //플레이어를 따라다니는 느낌으로 작성하기
    Player player;
    private void Start()
    {
        player = GameManager.Instance.Player;
    }

    private void Update()
    {
        transform.position = new (Mathf.Lerp(transform.position.x, player.transform.position.x,0.1f), transform.position.y, Mathf.Lerp(transform.position.z, player.transform.position.z, 0.1f));
    }
}
