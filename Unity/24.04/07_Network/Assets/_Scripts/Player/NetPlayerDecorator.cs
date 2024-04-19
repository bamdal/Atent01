using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Netcode;
using UnityEngine;

public class NetPlayerDecorator : NetworkBehaviour
{
    // 몸색 --------------------------------------------------------------------
    NetworkVariable<Color> bodyColor = new NetworkVariable<Color>();

    Renderer playerRenderer;
    Material bodyMaterial;

    readonly int BaseColor_Hash = Shader.PropertyToID("_BaseColor");

    // 이름 -------------------------------------------------------------------
    NetworkVariable<FixedString32Bytes> userName = new NetworkVariable<FixedString32Bytes>();
    NamePlate namePlate;

    private void Awake()
    {
        playerRenderer = GetComponentInChildren<Renderer>();
        bodyMaterial = playerRenderer.material; // 머터리얼중 첫번째걸로 가져옴

        bodyColor.OnValueChanged += onBodyColorChange;
        GameManager.Instance.onUserColorChange += SetPlayerColorChange;
        userName.OnValueChanged += onNameSet;

        namePlate = GetComponentInChildren<NamePlate>();
    }



    private void onBodyColorChange(Color previousValue, Color newValue)
    {
        bodyMaterial.SetColor(BaseColor_Hash, newValue);
    }

    public override void OnNetworkSpawn()
    {
        if(IsServer)
        {
            bodyColor.Value = Random.ColorHSV(0.0f,1.0f,1.0f,1.0f,1.0f,1.0f);
        }
        bodyMaterial.SetColor(BaseColor_Hash, bodyColor.Value);
    }

    public void SetName(string name)
    {
        if (IsOwner)
        {
            if (IsServer)
            {
                userName.Value = name;
            }
            else
            {
                RequestUserNameChangeServerRpc(name);
            }
        }
    }

    [ServerRpc]
    void RequestUserNameChangeServerRpc(string name)
    {
        userName.Value = name;
    }

    private void onNameSet(FixedString32Bytes previousValue, FixedString32Bytes newValue)
    {
        namePlate.SetName(newValue.ToString());
    }

    public void RefreshNamePlate()
    {
        namePlate.SetName(userName.Value.ToString());
        
    }

    public void SetPlayerColorChange(Color color)
    {
        if (IsServer)
        {
            bodyColor.Value = color;
        }
        bodyMaterial.SetColor(BaseColor_Hash, bodyColor.Value);
    }
}
