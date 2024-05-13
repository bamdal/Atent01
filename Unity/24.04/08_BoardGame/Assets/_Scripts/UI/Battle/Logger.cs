using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Logger : MonoBehaviour
{
    ScrollView scrollView;

    private void Awake()
    {
        scrollView = GetComponentInChildren<ScrollView>();
    }

    private void Start()
    {
        
    }
}
