using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RetryUI : MonoBehaviour
{
    public void Retry()
    {
        GameManager.Instance.Addscore1 = 0;
        GameManager.Instance.ClearScore = 100;
        SceneManager.LoadScene("StartScene");
    }
}
