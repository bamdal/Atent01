
using Unity.Collections;
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

    // 이펙트 ----------------------------------------------------------------

    NetworkVariable<bool> netEffectState = new NetworkVariable<bool>(false);
    readonly int EmissonIntensity_Hash = Shader.PropertyToID("_EmissonIntensity");

    public bool IsEffectOn
    {
        get => netEffectState.Value;
        set
        {
            if (netEffectState.Value != value)
            {
                if (IsServer)
                {

                    netEffectState.Value = value;
                }
                else
                {
                    UpdateEffectStateServerRpc(value);
                }
            }


        }
    }

    private void Awake()
    {
        playerRenderer = GetComponentInChildren<Renderer>();
        bodyMaterial = playerRenderer.material; // 머터리얼중 첫번째걸로 가져옴

        bodyColor.OnValueChanged += onBodyColorChange;
        userName.OnValueChanged += onNameSet;
        netEffectState.OnValueChanged += OnEffectStateChange;


        namePlate = GetComponentInChildren<NamePlate>();
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



    // 색상 설정용 ------------------------------------------------

    public void SetColor(Color color)
    {
        if (IsOwner)
        {
            if (IsServer)
            {
                bodyColor.Value = color;
            }
            else
            {
                RequestBodyColorChangeServerRpc(color);
            }
        }
    }

    [ServerRpc]
    void RequestBodyColorChangeServerRpc(Color color)
    {
        bodyColor.Value = color;
    }

    private void onBodyColorChange(Color previousValue, Color newValue)
    {
        bodyMaterial.SetColor(BaseColor_Hash, newValue);
    }

    // 이펙트용 ------------------------------------------------------------

    private void OnEffectStateChange(bool previousValue, bool newValue)
    {
        if (newValue)
        {
            bodyMaterial.SetFloat(EmissonIntensity_Hash, 0.4f);
        }
        else
        {
            bodyMaterial.SetFloat(EmissonIntensity_Hash, 0.0f);

        }
    }

    [ServerRpc]
    void UpdateEffectStateServerRpc(bool isOn)
    {
        netEffectState.Value = isOn;
    }

}
