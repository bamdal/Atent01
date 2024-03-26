using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;


public class InvenSlotUI : SlotUI_Base, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerClickHandler ,IPointerEnterHandler ,IPointerExitHandler, IPointerMoveHandler
{

    /// <summary>
    /// 장비여부 표시용 텍스트
    /// </summary>
    TextMeshProUGUI equiptext;

    /// <summary>
    /// 드래그 시작을 알리는 델리게이트(uint : 드래그 시작슬롯의 인덱스)
    /// </summary>
    public Action<uint> onDragBegin;

    /// <summary>
    /// 드래그 종료를 알리는 델리게이트(uint: 드래그가 끝난 슬롯의 인덱스, bool : 슬롯에서 끝났으면 true 아니면 false
    /// </summary>
    public Action<uint, bool> onDragEnd;

    /// <summary>
    /// 마우스 클릭을 알리는 델리게이트(uint : 클릭이 된 슬롯의 인덱스
    /// </summary>
    public Action<uint> onClick;

    /// <summary>
    /// 마우스 커서가 슬롯 위로 올라왔다 (uint : 들어간 슬롯의 인덱스)
    /// </summary>
    public Action<uint> onPointerEnter;

    /// <summary>
    /// 마우스 커서가 슬롯위에서 움직인다.(Vector2: 마우스 포인터의 스크린 좌표)
    /// </summary>
    public Action onPointerExit;

    /// <summary>
    /// 움직인 마우스 좌표 위치
    /// </summary>
    public Action<Vector2> onPointerMove;

    protected override void Awake()
    {
        base.Awake();
        Transform child = transform.GetChild(2);
        equiptext = child.GetComponent<TextMeshProUGUI>();
    }

    protected override void OnRefresh()
    {
        if (InvenSlot.IsEquipped)   // 화면 갱신할 때 장비 여부에 따라 장비 글자 색 변경
        {
            equiptext.color = Color.red;    // 장비했을때는 빨간색
        }
        else                        
        {
            equiptext.color= Color.clear;   // 장비하지 않았을 때는 투명
        }

    }



    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log($"드래그 시작 : {Index}번 슬롯");
        onDragBegin?.Invoke(Index);
    }

    public void OnDrag(PointerEventData eventData)
    {
        // OnBeginDrag와 OnEndDrag를 쓰기위해 존재
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        GameObject obj = eventData.pointerCurrentRaycast.gameObject;    // UI대상 레이캐스트
        if(obj != null )
        {
            // 마우스 위치에 어떤 UI가 있다
            InvenSlotUI endSlot = obj.GetComponent<InvenSlotUI>();
            if(endSlot != null)
            {
                // 슬롯이다
                Debug.Log($"드래그 종료 : [{endSlot.Index}]번 슬롯");
                onDragEnd?.Invoke(endSlot.Index,true);
            }
            else
            {
                // 슬롯이 아니다 => 원래대로 되돌리기
                Debug.Log($"{obj.gameObject.name}은 슬롯이 아닙니다.");
                onDragEnd?.Invoke(Index,false);
            }

        }
        else
        {
            // 마우스 위치에 어떤 UI도 없다
            Debug.Log("어떤 UI도 없다");
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        onClick?.Invoke(Index);
    
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        onPointerEnter?.Invoke(Index);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        onPointerExit?.Invoke();
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        onPointerMove?.Invoke(eventData.position);
    }
}
