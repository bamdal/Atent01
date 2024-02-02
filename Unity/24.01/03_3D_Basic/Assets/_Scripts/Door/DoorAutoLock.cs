using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DoorAutoLock : DoorAuto
{


    bool locking = false;

    public bool Locking
    {
        get => locking;
        set
        {
            if (locking != value)
            {
                locking = value;
                if (locking)
                {
                    //잠그기
                    doorMaterial.color = lockColor;
                    seccor.enabled = false;
                }
                else
                {
                    //열기
                    doorMaterial.color = unlockColor;
                    seccor.enabled = true;
                }
            }
        }
    }

    BoxCollider seccor;

    public Color lockColor = new Color(1, 0, 0, 0.7f);
    public Color unlockColor = new Color(0, 1, 0, 0.2f);

    Material doorMaterial;

    protected override void Awake()
    {
        base.Awake();
        seccor = GetComponent<BoxCollider>();

        Transform door = transform.GetChild(1);
        door = door.GetChild(0);

        MeshRenderer meshRenderer = door.GetComponent<MeshRenderer>();
        doorMaterial = meshRenderer.material;

    }

    protected override void Start()
    {
        
        Locking = true;

    }

    protected override void OnKeyUsed()
    {
        
        Locking = false;
    }


}
