using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVCam : MonoBehaviour
{
   
    void Start()
    {
        CinemachineVirtualCamera vcam = GetComponent<CinemachineVirtualCamera>();
        vcam.m_Follow = GameManager.Instance.Player.transform;
    }

}
