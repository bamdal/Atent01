using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class FireSetter : MonoBehaviour
{
    public GameObject firePrefab;

    public void SetFire(Ship ship)
    {
        foreach (var pos in ship.Positions)
        {
            GameObject inst = Instantiate(firePrefab, transform);           // 프리펩을 자식으로 하고
            Vector3 world = Vector3.zero;
            world.x = pos.x;
            world.z = -pos.y;                             // y는 보드위치로
            inst.transform.position = world + transform.position;  // 위치설정


        }
    }

    public void ResetFire()
    {
        while (transform.childCount > 0)             // 자식이 남아있으면 반복
        {
            Transform child = transform.GetChild(0);// 첫번째 자식 선택
            child.SetParent(null);                  // 부모 제거(Destroy가 즉시 실행되지 않기에 필요)
            Destroy(child.gameObject);              // 자식 삭제
        }
    }
}
