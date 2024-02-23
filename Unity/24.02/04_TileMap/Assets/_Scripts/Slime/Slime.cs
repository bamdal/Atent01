using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : RecycleObject
{

    private float maxHp = 3.0f;

    /// <summary>
    /// 페이즈 진행시간
    /// </summary>
    public float phaseDuration = 0.5f;

    /// <summary>
    /// 디졸브 진행시간
    /// </summary>
    public float dissolveDuration = 1.0f;


    /// <summary>
    /// 슬라임의 머티리얼
    /// </summary>
    Material mainMaterial;

    /// <summary>
    /// 아웃라인이 보일 때의 두깨
    /// </summary>
    const float VisibleOutlineThickness = 0.004f;

    /// <summary>
    /// 페이즈가 보일 때의 두깨
    /// </summary>
    const float VisiblePhaseThickness = 0.1f;

    // 쉐이더 프로퍼티 아이디들
    readonly int OutlineThicknessID = Shader.PropertyToID("_SlimeOutLineThickness");
    readonly int PhaseSplitID = Shader.PropertyToID("_SlimePhaseReverseSplit");
    readonly int PhaseThicknessID = Shader.PropertyToID("_SlimePhaseReverseThickness");
    readonly int DissolveFadeID = Shader.PropertyToID("_SlimeDissolveFade");

    Action onDissolveEnd;

    float hp;

    public float Hp 
    { 
        get => hp;
        set
        {

            hp = Mathf.Clamp(value, 0.0f, maxHp);
            if(hp == 0)
            {
                Die();
            }
        }
    }

    private void Awake()
    {
        Renderer spriteRenderer = GetComponent<Renderer>();
        mainMaterial = spriteRenderer.material;  // 머티리얼 가져오기
        onDissolveEnd += ReturnToPool;
        hp = maxHp;
    }



    private void Start()
    {
    }
    protected override void OnEnable()
    {
        base.OnEnable();

        ResetShaderProperty();
        StartCoroutine(StartPhase());

    }

    /// <summary>
    /// 슬라임이 죽을 때 실행되는 함수
    /// </summary>
    public void Die()
    {
        StartCoroutine(StartDissolve());    // 디졸브만 실행(디졸브 코루틴안에서 비활성화까지 처리)
    }

    /// <summary>
    /// 비활성화 되면서 풀로 되돌린다
    /// </summary>
    private void ReturnToPool()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 쉐이더용 프로퍼티 전부 초기화
    /// </summary>
    void ResetShaderProperty()
    {
        //  - 리셋
        ShowOutline(false);                         // 아웃라인 끄고
        mainMaterial.SetFloat(PhaseThicknessID, 0); // 페이즈 선 안보이게 하기
        mainMaterial.SetFloat(PhaseSplitID, 1);     // 전신 보이게 하기
        mainMaterial.SetFloat(DissolveFadeID, 1);   // 디졸브 안보이게 하기
    }
    /// <summary>
    /// 아웃라인 켜고 끄는 함수
    /// </summary>
    /// <param name="isShow">true면 보이고 false면 보이지 않는다.</param>
    public void ShowOutline(bool isShow = true)
    {
        //  - Outline on/off
        if (isShow)
        {
            mainMaterial.SetFloat(OutlineThicknessID, VisibleOutlineThickness); // 보이는 것은 두께를 설정하는 것으로 보이게 만듬
            
        }
        else
        {
            mainMaterial.SetFloat(OutlineThicknessID, 0);   // 안보이는 것은 두께를 0으로 만들어서 안보이게 만듬
        }
    }

    /// <summary>
    /// 페이즈 진행하는 코루틴(안보기->보이기)
    /// </summary>
    /// <returns></returns>
    IEnumerator StartPhase()
    {
        //  - PhaseReverse로 안보이는 상태에서 보이게 만들기 (1->0)


        float phaseNormalize = 1.0f / phaseDuration;    // 나누기 계산을 줄이기 위해 미리 계산

        float timeElapsed = 0.0f;   // 시간 누적용

        mainMaterial.SetFloat(PhaseThicknessID, VisiblePhaseThickness); // 페이즈 선을 보이게 만들기

        while (timeElapsed < phaseDuration)  // 시간진행에 따라 처리
        {
            timeElapsed += Time.deltaTime;  // 시간 누적

            //mainMaterial.SetFloat(PhaseSplitID,  1 - (timeElapsed / dissolveDuration));
            mainMaterial.SetFloat(PhaseSplitID, 1 - (timeElapsed * phaseNormalize));   // split 값을 누적한 시간에 따라 변경

            yield return null;
        }

        mainMaterial.SetFloat(PhaseThicknessID, 0); // 페이즈 선 안보이게 만들기
        mainMaterial.SetFloat(PhaseSplitID, 0);     // 숫자를 깔끔하게 정리하기 위한 것
    }

    /// <summary>
    /// 디졸브 진행하는 코루틴
    /// </summary>
    /// <returns></returns>
    IEnumerator StartDissolve()
    {
        //  - Dissolve 실행시키기(1->0)
        float dissolveNormalize = 1.0f / dissolveDuration;

        float timeElapsed = 0.0f;

        while (timeElapsed < dissolveDuration)
        {
            timeElapsed += Time.deltaTime;

            mainMaterial.SetFloat(DissolveFadeID, 1 - (timeElapsed * dissolveNormalize));

            yield return null;
        }
        mainMaterial.SetFloat(PhaseSplitID, 0);
        onDissolveEnd?.Invoke();
    }

#if UNITY_EDITOR
    public void TestShader(int index)
    {
        switch (index)
        {
            case 0:
                StartCoroutine(StartPhase());
                break;
            case 1:
                StartCoroutine(StartDissolve());
                break;
            case 2:
                ShowOutline(true);
                break;
            case 3:
                ShowOutline(false);
                break;
            case 4:
                ResetShaderProperty();
                break;
        }
    }

    public void Test_Die()
    {
        Die();
    }

#endif
}
