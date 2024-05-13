using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShipsInfo : MonoBehaviour
{
    public PlayerBase player;


    TextMeshProUGUI[] shipHPText;

    private void Start()
    {
        shipHPText = GetComponentsInChildren<TextMeshProUGUI>();
        foreach (var ship in player.Ships) 
        {
            ship.onHit += (hit) => { OnHit(hit); };
            ship.onSink += (sink) => { OnSink(sink); };
            shipHPText[(int)ship.Type - 1].text = $"{ship.HP}/{ship.Size}";
        }
    }

    private void OnSink(Ship sink)
    {
        shipHPText[(int)sink.Type - 1].color = Color.red;
        shipHPText[(int)sink.Type - 1].fontSize = 40.0f;
        shipHPText[(int)sink.Type - 1].text = $"Destroy!!";

    }

    private void OnHit(Ship hit)
    {
        shipHPText[(int)hit.Type - 1].text = $"{hit.HP}/{hit.Size}";
    }
}
