using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSensor : MonoBehaviour
{
    Slime slime;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (slime = collision.GetComponent<Slime>())
        {
            slime.ShowOutline(true);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (slime = collision.GetComponent<Slime>())
        {
            slime.ShowOutline(false);
        }
    }
}
