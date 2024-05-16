using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResultAnalysis : MonoBehaviour
{
    TextMeshProUGUI[] values;

    private void Awake()
    {
        values = transform.GetChild(1).GetComponentsInChildren<TextMeshProUGUI>(true);
    }

    public int TotalAttackCount
    {
        set
        {
            // value의 첫번째 자식 텍스트 수정
            values[0].text = $"<b>{value}</b>회";
        }
    }

    public int SuccessAttackCount
    {
        set
        {
            // value의 두번째 자식 텍스트 수정
            values[1].text = $"<b>{value}</b>회";
        }
    }

    public int FailAttackCount
    {
        set
        {
            // value의 세번째 자식 텍스트 수정
            values[2].text = $"<b>{value}</b>회";
        }
    }

    public float SuccessAttackRate
    {
        set
        {
            // value의 네번째 자식 텍스트 수정
            values[3].text = $"<b>{value*100.0f:f1}</b>%";
        }
    }
}
