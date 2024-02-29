using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Slime : RecycleObject
{

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

    /// <summary>
    /// 페이즈가 끝남을 알리는 델리게이트
    /// </summary>
    Action onPhaseEnd;

    /// <summary>
    /// 디졸브가 끝남을 알리는 델리게이트
    /// </summary>
    Action onDissolveEnd;

    /// <summary>
    /// 슬라임이 이동할 경로
    /// </summary>
    List<Vector2Int> path;

    /// <summary>
    /// 슬라임이 이동할 경로를 그려주는 클래스
    /// </summary>
    PathLine pathLine;

    /// <summary>
    /// 슬라임이 움직일 그리드맵
    /// </summary>
    TileGridMap map;

    /// <summary>
    /// 슬라임의 그리드 좌표
    /// </summary>
    Vector2Int GridPosition => map.WorldToGrid(transform.position);

    /// <summary>
    /// 이 슬라임이 위치하고 있는 노드
    /// </summary>
    Node current = null;

    Node Current
    {
        get => current;
        set
        {

            if (current != value)
            {
                if (current != null)    // 이전 노드가 null이면 스킵
                {
                    current.nodeType = Node.NodeType.Plain; // 이전 노드를 Plain으로 되돌리기
                }
                current = value;
                if (current != null)    // 새 노드가 null이면 스킵
                {
                    current.nodeType = Node.NodeType.Slime; // 새로 이동한 노드는 Slime으로 변경하기
                }


                // 위에서 발생하는 에러의 원인을 확인하고 수정하기
                // 아래쪽에 있는 슬라임이 위에 그려지게 만들기
            }




        }
    }

    /// <summary>
    /// 슬라임의 이동 속도
    /// </summary>
    public float moveSpeed = 2.0f;

    /// <summary>
    /// 슬라임의 이동 활성화 표시용 변수(true면 움직이기 시작, false면 안움직임)
    /// </summary>
    bool isMoveActivate = false;

    /// <summary>
    /// 슬라임이 죽었음을 알리는 델리게이트
    /// </summary>
    public Action onDie;

    /// <summary>
    /// 슬라임이 생성된 풀의 트랜스폼
    /// </summary>
    Transform pool;

    /// <summary>
    /// pool에 단 한번만 값을 설정하는 프로퍼티
    /// </summary>
    public Transform Pool
    {
        set
        {
            if (pool == null)
            {
                pool = value;
            }
        }
    }

    /// <summary>
    /// 스프라이트 랜더러 (Order In Layer 수정용)
    /// </summary>
    SpriteRenderer spriteRenderer;

    /// <summary>
    /// 다른 슬라인에 의해 경로가 막혔을 때 기다린 시간
    /// </summary>
    float pathWaitTime = 0.0f;

    /// <summary>
    /// 경로가 막혔을 때 최대로 기다리는 시간
    /// </summary>
    const float MaxPathWaitTime = 1.0f;


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        mainMaterial = spriteRenderer.material;  // 머티리얼 가져오기
        onPhaseEnd += () =>
        {
            isMoveActivate = true;
        };
        onDissolveEnd += ReturnToPool;
    
        path = new List<Vector2Int>();
        pathLine = GetComponentInChildren<PathLine>();
    }


    private void Update()
    {
        if(isMoveActivate)
            MoveUpdate();
    }

    /// <summary>
    /// Update 함수에서 이동 처리하는 함수
    /// </summary>
    private void MoveUpdate()
    {
        if (isMoveActivate)
        {
            // 경로가 있고 남은 경로가 있고 오래 기다리지 않았을때
            if (path != null && path.Count > 0 && pathWaitTime < MaxPathWaitTime)
            {
                
                // path의 첫번째 위치로 계속 이동
                Vector2Int destGrid = path[0];                          // path의 첫번째 위치 가져오기
                
                // 다른 슬라임이 있는 칸에는 이동하지않는다
                // 슬라임으로 표시된 노드가 아니거나, 내가 있는 노드일 때만 움직이기
                if (!map.IsSlime(destGrid) || map.GetNode(destGrid) == Current)
                {

                    Vector3 destPosition = map.GridToWorld(destGrid);       // 목적지 월드좌표 구하기
                    Vector3 direction = destPosition - transform.position;   // 방향계산

                    if (direction.sqrMagnitude < 0.001f)                    // 방향벡터의 길이를 확인해서 도착했는지 확인
                    {
                        transform.position = destPosition;                  // 오차보정
                        path.RemoveAt(0);                                   // path의 첫번째 위치를 제거
                    }
                    else
                    {
                        //transform.Translate(Time.deltaTime * moveSpeed * direction);    //도착 안했으면 direction의 위치로 이동
                        transform.Translate(Time.deltaTime * moveSpeed * direction.normalized); // 아래쪽에 있는 슬라임이 위에 그려지게 하기
                        Current = map.GetNode(transform.position);  // Current 변경 시도 및 처리
                    }
                    spriteRenderer.sortingOrder = -Mathf.FloorToInt(transform.position.y * 100);

                    // 첫번째 위치에 도착하면 path의 첫번째 위치를 제거
                    //transform.position = Vector2.MoveTowards(transform.position, map.GridToWorld(path[1]), moveSpeed * Time.deltaTime);
                    pathWaitTime = 0.0f;    //기다리는 시간 초기화
                }
                else
                {
                    // 다른 슬라임이 있어서 기다리는중
                    pathWaitTime += Time.deltaTime; // 기다린 시간 누적
                }

            }
            else
            {
                // 목적지 도착 or 오래 기다렸음
                pathWaitTime = 0.0f;
                OnDestinationArrive();  // 도착지 자동 설정
            }

        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        ResetShaderProperty();
        StartCoroutine(StartPhase());
        isMoveActivate = false;
    }

    protected override void OnDisable()
    {
        if(path != null)
            path.Clear();
        pathLine.ClearPath();

        base.OnDisable();
    }

    /// <summary>
    /// 슬라임 초기화 함수(스폰직후에 실행)
    /// </summary>
    /// <param name="gridMap">슬라임이 존재할 타일맵</param>
    /// <param name="world">슬라임의 시작위치(월드 좌표)</param>
    public void Initialize(TileGridMap gridMap, Vector3 world)
    {
        map = gridMap;
        transform.position = map.GridToWorld(map.WorldToGrid(world));   // 셀의 가운데 위치에 배치
        Current = map.GetNode(world);
    }

    /// <summary>
    /// 슬라임이 죽을 때 실행되는 함수
    /// </summary>
    public void Die()
    {
        isMoveActivate = false;             // 이동 비활성화
        onDie?.Invoke();                    // 죽었음을 알리기
        onDie = null;                       // 연결된 함수 모두 제거
        StartCoroutine(StartDissolve());    // 디졸브만 실행(디졸브 코루틴안에서 비활성화까지 처리)
    }

    /// <summary>
    /// 비활성화 되면서 풀로 되돌린다
    /// </summary>
    public void ReturnToPool()
    {
        Current = null; 
        transform.SetParent(pool);
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

        onPhaseEnd?.Invoke();
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

    /// <summary>
    /// 슬라임의 목적지를 지정하는 함수
    /// </summary>
    /// <param name="destination">목적지의 그리드 좌표</param>
    public void SetDestination(Vector2Int destination)
    {
        // 목적지까지의 경로 저장
        // 경로를 그리기
        path = AStar.PathFine(map, GridPosition, destination);
        //pathLine.DrawPath(map, path);
        ShowPath(GameManager.Instance.showSlimePath);

    }



    /// <summary>
    /// 목적지에 도착했을 때 실행되는 함수
    /// </summary>
    void OnDestinationArrive()
    {
        SetDestination(map.GetRandomMoveablePosition());
    }

    /// <summary>
    /// 경로를 보여줄지 말지 결정하는 함수
    /// </summary>
    /// <param name="isShow">true면 보여주고 false면 안보여 준다</param>
    public void ShowPath(bool isShow = true)
    {
        //pathLine.gameObject.SetActive(isShow);
        if (isShow)
        {
            pathLine.DrawPath(map, path);

        }
        else
        {
            pathLine.ClearPath();
        }
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
