using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class Test_NavMesh : MonoBehaviour
{


    public NavMeshAgent agent;
    public Vector3 targetPosition;

    void Start()

    {

        targetPosition = new Vector3(10, 10, 0);

        agent = GetComponent<NavMeshAgent>();

        agent.SetDestination(targetPosition);

    }
}
