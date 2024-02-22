using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.U2D;

public class Test_Slime2 : TestBase
{
    /// <summary>
    /// 코드로 이펙트를 조정할 머티리얼을 가진 랜더러
    /// </summary>
    public Renderer Slime;

    /// <summary>
    /// 코드로 조정할 머티리얼
    /// </summary>
    Material material;

    readonly int idSlimeOutLineThickness = Shader.PropertyToID("_SlimeOutLineThickness");
    readonly int idSlimePhaseReverseSplit = Shader.PropertyToID("_SlimePhaseReverseSplit");
    readonly int idSlimeDissolveFade = Shader.PropertyToID("_SlimeDissolveFade");
    readonly int idSlimePhaseReverseThickness = Shader.PropertyToID("_SlimePhaseReverseThickness");
    readonly int idOnOutLine = Shader.PropertyToID("_OnOutLine");

    float OnOutLine = 1.0f;

    /// <summary>
    /// 쉐이더 프로퍼티 변경속도
    /// </summary>
    public float speed = 3.0f;



    /// <summary>
    /// 보여질 아웃라인 두께
    /// </summary>
    const float VisibleOutlineThickness = 0.003f;

    const float VisiblePhaseThickness = 0.1f;


    private void Start()
    {

        material = Slime.material;

    }


    protected override void OnTest1(InputAction.CallbackContext context)
    {
        // 리셋
        ResetShaderProperty();
    }

    private void ResetShaderProperty()
    {
        material.SetFloat(idSlimeOutLineThickness, VisibleOutlineThickness);
        material.SetFloat(idSlimePhaseReverseSplit, 0f);
        material.SetFloat(idSlimeDissolveFade, 1f);
        material.SetFloat(idSlimePhaseReverseThickness, 0.1f);
        material.SetFloat(idOnOutLine, 1f);
    }



    protected override void OnTest2(InputAction.CallbackContext context)
    {
        // Outline on/off
        if(OnOutLine == 1.0f)
        {
            OnOutLine = 0.0f;
        }
        else
        {
            OnOutLine = 1.0f;
        }
        material.SetFloat(idOnOutLine, OnOutLine);

    }

    protected override void OnTest3(InputAction.CallbackContext context)
    {
        // PhaseReverse로 안보이는 상태에서 보이게 만들기 (1 -> 0)
        StopAllCoroutines();
        StartCoroutine(StartPhase());
    }

    /// <summary>
    /// 페이즈 진행 코루틴(안보이기 -> 보이기)
    /// </summary>
    /// <returns></returns>
    IEnumerator StartPhase()
    {
        float phseDuration = 0.5f;                  // 페이즈 진행시간
        float phaseNormalize = 1.0f / phseDuration; // 나누기 계산량 줄이기용으로 미리 계산

        float timeElapsed = 0.0f;   // 시간 누적용
        material.SetFloat(idSlimePhaseReverseThickness, VisiblePhaseThickness); // 페이즈 선을 보이게 만들기

        while (timeElapsed < phseDuration)  // 시간진행에 따라 처리
        {
            timeElapsed += Time.deltaTime;  // 시간 누적
            //material.SetFloat(idSlimePhaseReverseSplit, 1-(timeElapsed/phseDuration));
            material.SetFloat(idSlimePhaseReverseSplit, 1 - (timeElapsed * phaseNormalize));    // 페이즈 실행
            yield return null;

        }

        material.SetFloat(idSlimePhaseReverseThickness, 0);
        material.SetFloat(idSlimePhaseReverseSplit, 0);


    }

    protected override void OnTest4(InputAction.CallbackContext context)
    {
        // Dissolve 실행시키기 (1 -> 0)
        StopAllCoroutines();
        StartCoroutine(StartDissolve());
    }

    IEnumerator StartDissolve()
    {
        float phseDuration = 0.5f;
        float phaseNormalize = 1.0f / phseDuration;

        float timeElapsed = 0.0f;

        while (timeElapsed < phseDuration)
        {
            timeElapsed += Time.deltaTime;
            //material.SetFloat(idSlimePhaseReverseSplit, 1-(timeElapsed/phseDuration));
            material.SetFloat(idSlimeDissolveFade, 1 - (timeElapsed * phaseNormalize));
            yield return null;

        }

        material.SetFloat(idSlimeDissolveFade, 0);



    }
}
