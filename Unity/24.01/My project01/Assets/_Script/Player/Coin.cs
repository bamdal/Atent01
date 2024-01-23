using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public int score = 10;
    Player player;

    private void Awake()
    {
        player = GameManager.Instance.Player;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            
            player.AddScore(score);
            transform.parent.gameObject.SetActive(false);
        }

    }


}
