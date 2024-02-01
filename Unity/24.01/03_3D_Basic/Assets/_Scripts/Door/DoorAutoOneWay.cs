using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorAutoOneWay : DoorAuto
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            Vector3 playerToDoor = transform.position - other.transform.position; // 플레이어에서 문으로 향하는 방향벡터

            float angle = Vector3.Angle(transform.forward, playerToDoor);

            if(angle > 90.0f) 
            {
                Open();
            
            }

        }
    }
}
