using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class Test_NavMesh : MonoBehaviour
{

    NavMeshAgent agent;
    TestInputActions action;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        action = new TestInputActions();
    }

    private void OnEnable()
    {
        action.Test.Enable();
        action.Test.LClick.performed += OnLClick;

        // agent.hasPath 경로를 가졌는가
        // agent.remainingDistance 남은 거리
        // agent.pathPending 경로가 계산중인 상황
        // agent.autoRepath 다시 길 찾기 할지 말지
    }



    private void OnDisable()
    {
        action.Test.RClick.performed -= OnRClick;
        action.Test.LClick.performed -= OnLClick;
        action.Test.Disable();
    }



    private void OnLClick(InputAction.CallbackContext context)
    {
        Vector2 screen = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(screen);
        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            agent.SetDestination(hitInfo.point);
        }
    }

    private void OnRClick(InputAction.CallbackContext context)
    {
        Vector2 screen = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(screen);
        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            agent.Warp(hitInfo.point);
        }
    }
}
