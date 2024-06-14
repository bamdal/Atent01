using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class DataPanel_장민수 : MonoBehaviour
{
    enum SortType
    {
        ByName = 0,
        ByScore,
        ByRatio,
    }

    List<Data> dataList;

    MyTest inputActions;

    TextMeshProUGUI[] names;
    TextMeshProUGUI[] scores;
    TextMeshProUGUI[] ratios;

    private void Awake()
    {
        Transform child = transform.GetChild(1);    //Slots
        names = new TextMeshProUGUI[child.childCount];
        scores = new TextMeshProUGUI[child.childCount];
        ratios = new TextMeshProUGUI[child.childCount];

        dataList = new List<Data>(child.childCount);
        for (int i = 0; i < child.childCount; i++)
        {
            names[i] = child.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>();
            scores[i] = child.GetChild(i).GetChild(1).GetComponent<TextMeshProUGUI>();
            ratios[i] = child.GetChild(i).GetChild(2).GetComponent<TextMeshProUGUI>();
        }

        inputActions = new MyTest();
        Initialize();
        Refresh();
    }

    private void OnEnable()
    {
        inputActions.Test.Enable();
        inputActions.Test.Test1.performed += OnTest1;
        inputActions.Test.Test2.performed += OnTest2;
        inputActions.Test.Test3.performed += OnTest3;
    }

    private void OnDisable()
    {
        inputActions.Test.Test3.performed -= OnTest3;
        inputActions.Test.Test2.performed -= OnTest2;
        inputActions.Test.Test1.performed -= OnTest1;

        inputActions.Test.Disable();
    }

    void Initialize()
    {
        Data d1 = new("AAA", 30, 0.5f);
        Data d2 = new("BB", 10, 0.2f);
        Data d3 = new("CCCC", 40, 0.1f);
        Data d4 = new("DDD", 20, 0.4f);
        Data d5 = new("EE", 50, 0.3f);

        dataList.Add(d1);
        dataList.Add(d2);
        dataList.Add(d3);
        dataList.Add(d4);
        dataList.Add(d5);
    }

    void Sort(SortType type)
    {
        switch (type)
        {
            case SortType.ByName:
                dataList = dataList.OrderBy(d => d.name).ToList();
                break;
            case SortType.ByScore:
                dataList = dataList.OrderBy(d => d.score).ToList();
                break;
            case SortType.ByRatio:
                dataList = dataList.OrderByDescending(d => d.ratio).ToList();
                break;
        }
     
    }

    void Refresh()
    {
        for(int i = 0; i < dataList.Count; i++)
        {
            names[i].text = dataList[i].name;
            scores[i].text = dataList[i].score.ToString();
            ratios[i].text = dataList[i].ratio.ToString();
        }
    }

    private void OnTest1(InputAction.CallbackContext context)
    {
        Debug.Log("1번버튼클릭 name기준으로 오름차순 정렬");
        Sort(SortType.ByName);
        Refresh();
    }

    private void OnTest2(InputAction.CallbackContext context)
    {
        Debug.Log("2번버튼클릭 Score기준으로 오름차순 정렬");
        Sort(SortType.ByScore);
        Refresh();
    }

    private void OnTest3(InputAction.CallbackContext context)
    {
        Debug.Log("3번버튼클릭 ratio기준으로 내림차순 정렬");
        Sort(SortType.ByRatio);
        Refresh();
    }
}