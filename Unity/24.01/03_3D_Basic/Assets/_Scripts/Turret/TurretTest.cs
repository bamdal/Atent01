/*using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TurretTest : TurretBase
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
        if (target != null)
        {
            Vector3 dir = target.transform.position - transform.position;
            dir.y = 0.0f;
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
        if (isStartFire) // 발사해야 되는 상황인지 확인
        {
            StartFire(); // 발사 시작
        }
        else
        {

            StopFire(); // 발사각 밖이면 발사 정지
        }
    }


    /// <summary>
    /// 총알을 발사하기 시작(중복 실행 없음)
    /// </summary>
    void StartFire()
    {
        if (!isFiring)
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
        Handles.DrawWireDisc(transform.position, Vector3.up, sightRange);


        Barrelbody = transform.GetChild(2);


        Vector3 from = Barrelbody.position;
        Vector3 to = Barrelbody.position + Barrelbody.forward * sightRange;
        Handles.color = Color.yellow;
        Handles.DrawDottedLine(from, to, 2.0f);

        Handles.color = Color.red;
        Vector3 dir1 = Quaternion.AngleAxis(-fireAngle, transform.up) * Barrelbody.forward;
        Vector3 dir2 = Quaternion.AngleAxis(fireAngle, transform.up) * Barrelbody.forward;

        to = transform.position + dir1 * sightRange;
        Handles.DrawLine(from, to, 2.0f);
        to = transform.position + dir2 * sightRange;
        Handles.DrawLine(from, to, 2.0f);

    }
#endif

}
*/