using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorAuto : DoorBase
{
    protected virtual void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Player"))
        {
            Open();
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Close();
        }
    }
}
