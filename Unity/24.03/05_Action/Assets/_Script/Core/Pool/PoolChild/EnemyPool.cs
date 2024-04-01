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
    /// Ǯ���� ������� �ʴ� ������Ʈ�� �ϳ� ������ �����ϴ� �Լ�
    /// </summary>
    /// <param name="index">����� ��������Ʈ �ε���</param>
    /// <param name="position">��ġ�� ��ġ(������ǥ)</param>
    /// <param name="eulerAngle">��ġ�� ȸ��</param>
    /// <returns>��ȯ�� ������Ʈ</returns>
    public Enemy GetObject(int index, Vector3? position = null, Vector3? eulerAngle = null)
    {
        Enemy enemy = GetObject(position, eulerAngle);
        enemy.waypoints = waypoints[index];

        return enemy;
    }

    /// <summary>
    /// �ε����� ���������ʾ��� ��� �⺻ �ε�����
    /// </summary>
    /// <param name="comp">������ ��</param>
    protected override void OnGenerateObject(Enemy comp)
    {
        comp.waypoints = waypoints[0];
    }
}
    

