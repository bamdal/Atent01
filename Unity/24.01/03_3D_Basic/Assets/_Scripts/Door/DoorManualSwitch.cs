using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorManualSwitch : DoorBase, IInteracable
{
    bool isOpen = false;
    public void Use()
    {
        if(!isOpen)
        {
            Open();
            isOpen = true;
        }
        else if(isOpen) 
        {
            Close();
            isOpen = false;
        }
    }
    protected override void OnOpen()
    {
        
    }

    protected override void OnClose()
    {
        
    }
}
