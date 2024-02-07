using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        IAlive alive = other.GetComponent<IAlive>();    
        if(alive != null )
        {
            alive.Die();
        }
    }
}
