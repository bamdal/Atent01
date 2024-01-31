using System;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;


public class TurretTrace : TurretBase
{

    /// <summary>
    /// 시야 범위
    /// </summary>
    public float sightRange = 10.0f;

    /// <summary>
    /// 터렛머리 회전속도
    /// </summary>
    public float turnSpeed = 20.0f;

    /// <summary>
    /// 터렛이 총알을 발사를 시작하는 발사각 +-10도
    /// </summary>
    public float fireAngle = 10.0f;

    SphereCollider sightTrigger;


    /// <summary>
    /// 시야에 들어온 플레이어
    /// </summary>
    Player target;

    /// <summary>
    /// 발사중인지 검사
    /// </summary>
    bool isFiring = false;

#if UNITY_EDITOR

    /// <summary>
    /// 내 공격 영역 안에 플레이어가 있고 발사각 안에 플레이어가 있는 상태인지 아닌지 확인하는 프로퍼티
    /// </summary>
    bool IsRedstate => isFiring;

    /// <summary>
    /// 내 공격 영역안에 플레이어가 있는 상태인지 아닌지 확인하기 위한 프로퍼티
    /// </summary>
    bool IsOrangestate => IsTargetVisible;

    /// <summary>
    /// 플레이어가 보이는지 아닌지 표시해 놓은 변수(true면 tartget이 지정되어있다)
    /// </summary>
    bool IsTargetVisible = false;
#endif
    protected override void Awake()
    {
        base.Awake();
        sightTrigger = GetComponent<SphereCollider>();

    }

    private void Start()
    {
        sightTrigger.radius = sightRange;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == GameManager.Instance.Player.transform)
        {
            target = GameManager.Instance.Player;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform == GameManager.Instance.Player.transform)
        {
            target = null;
        }
    }

    private void Update()
    {
        LookTargetAndAttack();
    }

    private void LookTargetAndAttack()
    {
        bool isStartFire = false;
        if (target != null) // 플레이어가 내 트리거 안에 들어와싿
        {
            Vector3 dir = target.transform.position - transform.position; // 터렛에서 플레어어로 가는 방향 계산
            dir.y = 0.0f;

            if(IsVisivleTarget(dir))
            {

                Barrelbody.rotation = Quaternion.Slerp(
                    Barrelbody.rotation,
                    Quaternion.LookRotation(dir),
                    Time.deltaTime * turnSpeed);
                // Vector3.SignedAngle : 두 벡터의 사이각을 구하는데 방향을 고려하여 계산 왼손법칙따라 +-값이 나옴
                float angle = Vector3.Angle(Barrelbody.forward, dir);
                if (angle < fireAngle)
                {
                    isStartFire = true; // 발사 결정
                }

            }
        }
#if UNITY_EDITOR
        else
        {
            IsTargetVisible = false;
        }
#endif
        if(isStartFire) // 발사해야 되는 상황인지 확인
        {
            StartFire(); // 발사 시작
        }
        else
        {

            StopFire(); // 발사각 밖이면 발사 정지
        }
    }

    /// <summary>
    /// Target이 보이는지 확인하는 함수
    /// </summary>
    /// <param name="lookDirection">바라보는 방향</param>
    /// <returns>보이면 true 안보이면 false</returns>
    private bool IsVisivleTarget(Vector3 lookDirection)
    {
        bool result = false;

        Ray ray = new Ray(Barrelbody.position, lookDirection);

        // int layerMask = LayerMask.GetMask("Default","Player"); // RayCast를 할 때 특정 레이어만 체크하고 싶을 때 사용

        // out : 출력용 파라메터라고 알려주는 키워드, 무조건 함수가 실행되었을 때 초기화 된다.
        if (Physics.Raycast(ray ,out RaycastHit hitInfo, sightRange))
        {
            if(hitInfo.transform == target.transform) 
            {
                result =true;
            }
        }

#if UNITY_EDITOR
        IsTargetVisible = result;
#endif

        return result;
    }

    /// <summary>
    /// 총알을 발사하기 시작(중복 실행 없음)
    /// </summary>
    void StartFire()
    {
        if(!isFiring)
        {
            StartCoroutine(fireCoroutine);
            isFiring = true;
        }
    }

    /// <summary>
    /// 총알 발사 정지
    /// </summary>
    void StopFire()
    {
        if (isFiring)
        {
            StopCoroutine(fireCoroutine);
            isFiring = false;
        }
    }
#if UNITY_EDITOR
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        // sightRange 범위 그리기
        Handles.DrawWireDisc(transform.position, Vector3.up, sightRange);

       
            Barrelbody = transform.GetChild(2);
        

        Vector3 from = Barrelbody.position;
        Vector3 to = Barrelbody.position + Barrelbody.forward * sightRange;

        // 중심 선 그리기
        Handles.color = Color.yellow;
        Handles.DrawDottedLine(from, to, 2.0f);

        // 시야 각 내부 그리기
        Handles.color = Color.green;

        // 평소(녹색), 플레이어 쪽으로 머리 돌리는 중(주황색), 플레이어를 공격 (빨간색)
        Vector3 dir1 = Quaternion.AngleAxis(-fireAngle, transform.up) * Barrelbody.forward;
        Vector3 dir2 = Quaternion.AngleAxis(fireAngle, transform.up) * Barrelbody.forward;
        
        if(IsRedstate)
        {
            Handles.color = Color.red;
        }
        else if(IsOrangestate)
        {
            Handles.color = new Color(1.0f, 0.5f, 0.0f);
        }
        else
        {
            Handles.color = Color.green;
        }
            to = transform.position + dir1 * sightRange;
        Handles.DrawLine(transform.position, to, 2.0f);
        to = transform.position + dir2 * sightRange;
        Handles.DrawLine(transform.position, to, 2.0f);
      
        Handles.DrawWireArc(transform.position, transform.up  , dir1, fireAngle*2.0f, sightRange,3.0f);
    }
#endif

}

// 실습 : 추적용 터렛 만들기
// 1. 플레이어가 터렛으로 부터 일정 거리안에 있으면 플레이어 쪽으로 barrelbody가 플레이어쪽으로 회전(Y 축 회전만)
// 2. 플레이어가 터렛의 발사각 안에 있으면 총알을 주기적으로 발사
// 3. 플레이어가 터렛의 발사각 안에 없으면 총알 발사를 정지
// 4. Gizoms를 통해 시야범위와 발사각 보이기(Handles 추천)