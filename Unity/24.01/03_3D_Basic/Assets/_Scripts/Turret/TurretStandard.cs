using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretStandard : TurretBase
{

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        StartCoroutine(PeriodFire());
    }

    IEnumerator PeriodFire()
    {
        while (true)
        {
            yield return new WaitForSeconds(fireInterval);
            Factory.Instance.GetObject(bulletType, fireTransform.position, fireTransform.rotation.eulerAngles);

        }
    }
}
