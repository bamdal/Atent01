using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{

    Player player;
    ParticleSystem[] ps;

    public Action onClear;
    private void Awake()
    {
        player = GameManager.Instance.Player;
        Transform child = transform.GetChild(2);
        ps = child.GetComponentsInChildren<ParticleSystem>();

    }

    private void OnTriggerEnter(Collider other)
    {
        for(int i = 0; i < ps.Length; i++)
        {
            ps[i].Play();
        }
        GameManager.Instance.GameClear();
    }



}
