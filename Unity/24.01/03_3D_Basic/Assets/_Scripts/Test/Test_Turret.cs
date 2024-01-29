using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test_Turret : TestBase
{
    public GameObject bulletPrefab;
    public float interval = 0.1f;

    Transform FireTransform;
    public Transform TurretFireTransform;

    private void Start()
    {
        FireTransform = transform.GetChild(0);
    }

    protected override void OnTest1(InputAction.CallbackContext context)
    {
        Factory.Instance.GetBullet(FireTransform.position);

    }

    protected override void OnTest2(InputAction.CallbackContext context)
    {

        StartCoroutine(FireCountinuos());
    }

    IEnumerator FireCountinuos()
    {
        while (true)
        {
            Factory.Instance.GetBullet(TurretFireTransform.position);
            yield return new WaitForSeconds(interval);
        }
    }
}
