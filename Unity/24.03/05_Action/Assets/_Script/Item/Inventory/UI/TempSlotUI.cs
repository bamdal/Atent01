using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TempSlotUI : SlotUI_Base
{
    InvenTempSlot tempSlot;

    Player owner;

    public uint FromIndex => tempSlot.FromIndex;

    private void Update()
    {
        transform.position = Mouse.current.position.ReadValue();
    }

    public override void InitializeSlot(InvenSlot slot)
    {
        base.InitializeSlot(slot);
        tempSlot = slot as InvenTempSlot;
        owner = GameManager.Instance.InventoryUI.Owner;
        Close();
    }

    public void Open()
    {
        transform.position = Mouse.current.position.ReadValue();
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    /*    public void SetFromIndex(uint index)
        {
            tempSlot.SetFromIndex(index);
        }*/

    /// <summary>
    /// 마우스 버튼이 인벤토리 영역 밖에서 떨어졌을 때 실행되는 함수
    /// </summary>
    /// <param name="screenPosition">마우스 커서의 스크린 좌표</param>
    public void OnDrop(Vector2 screenPosition)
    {
        // 아이템이 있을때만 처리
        // 스크린 좌표를 이용해 레이생성
        // 레이가 도착한 장소의 바닥에 드랍(Ground레이어에 있는 콜라이더랑 체크)
        // 아이템생성 후 임시 슬롯 비우기
        if (!InvenSlot.IsEmpty)
        {
             
      
            Ray ray = Camera.main.ScreenPointToRay(screenPosition);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, 1000.0f, LayerMask.GetMask("Ground")))
            {
                // 장비중인 아이템이면 버리기 전에 장비 해제
                if (InvenSlot.IsEquipped)
                {
                    ItemData_Equip itemData = InvenSlot.ItemData as ItemData_Equip;
                    owner.UnEquipItem(itemData.EquipType);
                }

                // Ground레이어에 있는 콜라이더랑 체크
                Ray MakeY = new Ray(new Vector3(hitInfo.point.x, 999, hitInfo.point.z), -Vector3.up);
                Vector3 dropPosition = hitInfo.point;
                RaycastHit MakeYHit;
                dropPosition.y = Physics.Raycast(MakeY, out MakeYHit, 5000.0f, LayerMask.GetMask("Ground")) ? MakeYHit.point.y : 0;

                Vector3 dropDir = dropPosition - owner.transform.position;
                if (dropDir.sqrMagnitude > owner.ItemPickupRange * owner.ItemPickupRange)
                {
                    dropPosition = dropDir.normalized * owner.ItemPickupRange + owner.transform.position;
                    dropPosition.y = Physics.Raycast(MakeY, out MakeYHit, 5000.0f, LayerMask.GetMask("Ground")) ? MakeYHit.point.y : 0;

                }

                // 아이템 생성
                Factory.Instance.MakeItems(InvenSlot.ItemData.code,InvenSlot.ItemCount, dropPosition,InvenSlot.ItemCount > 1);
                tempSlot.ClearSlotItem();
            }
        }
    }
}
