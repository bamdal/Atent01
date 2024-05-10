using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FinishDeploymentButton : MonoBehaviour
{
    Button button;


    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() => { Debug.Log("넘어가기"); });
    }
    public void Activate()
    {
        button.interactable = true;
    }
}
