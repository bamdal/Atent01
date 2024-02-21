using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test_Slime : TestBase
{
    /// <summary>
    /// 슬라임들의 렌더러(0 아웃라인, 1 페이즈 , 2 리버스페이즈, 3 인라인, 4 디졸브
    /// </summary>
    public Renderer[] slimes;

    /// <summary>
    /// 슬라임 머티리얼
    /// </summary>
    Material[] materials;

    /// <summary>
    /// 쉐이더 프로퍼티 변경속도
    /// </summary>
    public float speed = 0.5f;

    /// <summary>
    /// 시간 누적용(삼각함수에 사용)
    /// </summary>
    float timeElapsed = 0.0f;

    //쉐이더 프로퍼티용
    public bool outLineThicknessChange = false;
    public bool phaseSplitChange = false;
    public bool phaseThicknessChange = false;
    public bool inLineThicknessChange = false;
    public bool dissloveChange = false;


    /// <summary>
    /// split 정도(페이즈, 페이즈 리버스)
    /// </summary>
    [Range(0f, 1f)]
    public float split = 0.0f;

    /// <summary>
    /// 페이즈류 두께
    /// </summary>
    [Range(0.1f, 0.2f)]
    public float pahseThickness = 0.1f;

    /// <summary>
    /// 아웃라인의 두께
    /// </summary>
    [Range(0f, 0.01f)]
    public float outlineThickness= 0.0015f;

    // 프로퍼티 ID를 숫자로 번경
    readonly int idSplit = Shader.PropertyToID("_Split");
    readonly int idSplitReverse = Shader.PropertyToID("_ReverseSplit");

    readonly int idThickness = Shader.PropertyToID("_PhaseThickness");
    readonly int idThicknessReverse = Shader.PropertyToID("_ReverseThickness");

    readonly int idOutlineThickness = Shader.PropertyToID("_Thickness");
    readonly int idInlineThickness = Shader.PropertyToID("_InThickness");

    readonly int idDissolveFade = Shader.PropertyToID("_DissolveFade");



    private void Start()
    {
        materials = new Material[slimes.Length];    // 머티리얼 미리 찾아 저장
        for(int i=0; i<slimes.Length; i++)
        {
            materials[i] = slimes[i].material;
        }
    }


    private void Update()
    {
        timeElapsed += Time.deltaTime;
        float num = (Mathf.Cos(timeElapsed * speed) + 1.0f) * 0.5f; // 시간 변화에 따라 num이 0~1로 계속 핑퐁된다

        if(outLineThicknessChange)
        {
            
            float min = 0.0f;
            float max = 0.01f;
            float result = min + (max - min) * num; //num값에 따라 최소~최대로 변경
            materials[0].SetFloat(idOutlineThickness, result);
            //materials[3].SetFloat(idInlineThickness, result);
        }
        if(phaseSplitChange)
        {
            materials[1].SetFloat(idSplit, num);
            materials[2].SetFloat(idSplitReverse, num);
            split = num;
        }
        if(phaseThicknessChange)
        {
            float min = 0.1f;
            float max = 0.2f;
            float result = min + (max - min) * num; //num값에 따라 최소~최대로 변경
            materials[1].SetFloat(idThickness, result);
            materials[2].SetFloat(idThicknessReverse, result);
        }
        if (inLineThicknessChange)
        {

            float min = 0.0f;
            float max = 0.01f;
            float result = min + (max - min) * num; //num값에 따라 최소~최대로 변경
            materials[3].SetFloat(idInlineThickness, result);
        }
        if (dissloveChange)
        {
            materials[4].SetFloat(idDissolveFade, num);
        }


    }

    protected override void OnTest1(InputAction.CallbackContext context)
    {




        //material.SetFloat("_Split", split);
        //materials[0].SetFloat(idSplit, split);



        // 아웃라인의 두께 변경해보기

        outLineThicknessChange = !outLineThicknessChange;
    }

    protected override void OnTest2(InputAction.CallbackContext context)
    {
        // Phase와 ReversePhase split 둘다 적용하기
        phaseSplitChange = !phaseSplitChange;
    }

    protected override void OnTest3(InputAction.CallbackContext context)
    {
        // Phase 와 ReversePhase의 두깨 변경하기
        phaseThicknessChange = !phaseThicknessChange;
    }

    protected override void OnTest4(InputAction.CallbackContext context)
    {
        // InnerLine 두께 조정
        inLineThicknessChange = !inLineThicknessChange;
    }

    protected override void OnTest5(InputAction.CallbackContext context)
    {
        // Dissolve의 fade조정하기
        dissloveChange = !dissloveChange;
    }
}
