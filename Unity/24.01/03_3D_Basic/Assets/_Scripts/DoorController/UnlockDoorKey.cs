using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockDoorKey : MonoBehaviour
{
    public DoorAutoLock target;
    public float rotationSpeed = 5.0f;
    Transform child;

    private void Awake()
    {
        child = transform.GetChild(0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
     
            gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        child.transform.Rotate(Time.deltaTime * rotationSpeed * transform.up);
    }
}
