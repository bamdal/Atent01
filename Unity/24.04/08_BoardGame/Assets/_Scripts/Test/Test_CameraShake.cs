using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test_CameraShake : TestBase
{
    public CinemachineImpulseSource source;

    [Range(0,5)]
    public float force;

    private void Start()
    {
        source = FindAnyObjectByType<CinemachineImpulseSource>();
    }

    protected override void OnTest1(InputAction.CallbackContext context)
    {
        source.GenerateImpulse();
    }

    protected override void OnTest2(InputAction.CallbackContext context)
    {
        source.GenerateImpulseAtPositionWithVelocity(transform.position,Random.insideUnitCircle.normalized);    // 랜덤방향으로 흔들리기 z축 제외
        source.GenerateImpulse();
    }

    protected override void OnTest3(InputAction.CallbackContext context)
    {
        GameManager.Instance.CameraShake(force);
    }
}
