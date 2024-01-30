using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test_Turret : TestBase
{
    public GameObject bulletPrefab;
    public float interval = 0.1f;
    public PoolObjectType type;

    Transform FireTransform;
    public Transform TurretFireTransform;

    private void Start()
    {
        FireTransform = transform.GetChild(0);
    }

    protected override void OnTest1(InputAction.CallbackContext context)
    {
        Factory.Instance.GetObject(type,FireTransform.position);

    }

    protected override void OnTest2(InputAction.CallbackContext context)
    {

        StartCoroutine(FireCountinuos());
    }

    IEnumerator FireCountinuos()
    {
        while (true)
        {
            Factory.Instance.GetObject(type, FireTransform.position);
            yield return new WaitForSeconds(interval);
        }
    }
}
