using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitUI : MonoBehaviour
{
    public void ExitGame()
    {
       // UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }

}
