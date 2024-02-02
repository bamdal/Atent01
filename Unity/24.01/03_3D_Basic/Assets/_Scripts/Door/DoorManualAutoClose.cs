using System.Collections;
using TMPro;
using UnityEngine;

public class DoorManualAutoClose : DoorManual, IInteracable
{
    public float autoCloseTime = 3.0f;


    public new void Use()   // 
    {
        Open();
        StopAllCoroutines();
        StartCoroutine(AutoClose());
    }
    IEnumerator AutoClose()
    {
        yield return new WaitForSeconds(autoCloseTime);
        Close();
    }

}
