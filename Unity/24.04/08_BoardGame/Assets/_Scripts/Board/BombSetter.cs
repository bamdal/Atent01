using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombSetter : MonoBehaviour
{
    /// <summary>
    /// 공격이 성공했을 때 보여질 표시
    /// </summary>
    public GameObject sucessPrefab;

    /// <summary>
    /// 공격이 실패했을 때 보여질 표수
    /// </summary>
    public GameObject failPrefab;

   
    


    /// <summary>
    /// 공격받은 위치에 포탄 명중 여부 표시해주는 함수
    /// </summary>
    /// <param name="world">공격받은 위치(월드좌표,그리드 가운데)</param>
    /// <param name="isSuccess">공격성공시 true 아니면 false</param>
    public void SetBombMark(Vector3 world, bool isSuccess)
    {
        GameObject prefab = isSuccess ? sucessPrefab : failPrefab;  // 프리펩 결정
        GameObject inst = Instantiate(prefab, transform);           // 프리펩을 자식으로 하고

        world.y = transform.position.y;                             // y는 보드위치로
        inst.transform.position = world+transform.parent.position;  // 위치설정

        

    }


    /// <summary>
    /// 모든 폭탄 표시 제거하는 함수
    /// </summary>
    public void ResetBombMarks()
    {
        while(transform.childCount > 0)             // 자식이 남아있으면 반복
        {
            Transform child = transform.GetChild(0);// 첫번째 자식 선택
            child.SetParent(null);                  // 부모 제거(Destroy가 즉시 실행되지 않기에 필요)
            Destroy(child.gameObject);              // 자식 삭제
        }
    }
}
