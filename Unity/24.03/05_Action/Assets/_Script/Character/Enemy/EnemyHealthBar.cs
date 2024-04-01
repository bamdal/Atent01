using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthBar : MonoBehaviour
{
    // ���� HP ����� ���� HP ������ ǥ��
    // 2. ���� ���������� ���̰� �ϱ�
    // HP�� �������� �پ��� ���� ������ �Ѵ�
    // �����忩�� �Ѵ�. - �׻� ī�޶� �������� �ٶ󺸾ƾ� �Ѵ�

    /// <summary>
    /// fill�� �Ǻ��� �� Ʈ������
    /// </summary>
    Transform fillPivot;
    private void Awake()
    {
        fillPivot = transform.GetChild(1);  // fill �Ǻ� ã��

        IHealth target = GetComponentInParent<IHealth>();
        target.onHealthChange += Refresh;   // �θ𿡼� IHealthã�Ƽ� ��������Ʈ ����
    }


    private void LateUpdate()
    {
        // ������� ����� ���� ī�޶��� �������� ���̰� �ϱ�
        transform.forward =  transform.position - Camera.main.transform.position ;
    }

    /// <summary>
    /// �θ��� HP�� ����Ǹ� ����Ǵ� �Լ�
    /// </summary>
    /// <param name="ratio">HP ���� (hp/maxHP)</param>
    private void Refresh(float ratio)
    {
        fillPivot.localScale = new(ratio, 1, 1);
    }
}
