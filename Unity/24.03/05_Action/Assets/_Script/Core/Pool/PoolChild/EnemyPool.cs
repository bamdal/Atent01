using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : ObjectPool<Enemy>
{
    public Waypoints[] waypoints;

    private void Awake()
    {
        Transform child = transform.GetChild(0);
        waypoints = child.GetComponentsInChildren<Waypoints>();
    }


    /// <summary>
    /// 풀에서 사용하지 않는 오브젝트를 하나 꺼낸후 리턴하는 함수
    /// </summary>
    /// <param name="index">사용할 웨이포인트 인덱스</param>
    /// <param name="position">배치될 위치(월드좌표)</param>
    /// <param name="eulerAngle">배치될 회전</param>
    /// <returns>소환된 오브젝트</returns>
    public Enemy GetObject(int index, Vector3? position = null, Vector3? eulerAngle = null)
    {
        Enemy enemy = GetObject(position, eulerAngle);
        enemy.waypoints = waypoints[index];

        return enemy;
    }


    /// <summary>
    /// 인덱스를 설정하지않았을 경우 기본 인덱스값
    /// </summary>
    /// <param name="comp">생성될 적</param>
    protected override void OnGenerateObject(Enemy comp)
    {
        comp.waypoints = waypoints[0];
    }
}
    

