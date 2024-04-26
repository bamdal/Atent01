using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GridPainter : MonoBehaviour
{
    public GameObject LinePrefab;
    public GameObject LetterPrefab;

    char A = 'A';
    

    public int hightcount = 11;
    public int weightcount = 11;
    
    
    private void Start()
    {
        DrawGridLines();

        DrawGridLetters();
    }


    private void DrawGridLines()
    {
        for (int i = 0; i < weightcount; i++)
        {
            Instantiate(LinePrefab, new Vector3(transform.position.x + i, transform.position.y, transform.position.z - 5), Quaternion.identity, transform);

        }
        for (int i = 0; i < hightcount; i++)
        {
            Instantiate(LinePrefab, new Vector3(transform.position.x + 4, transform.position.y, transform.position.z - i), Quaternion.Euler(0, 90, 0), transform);

        }
    }

    private void DrawGridLetters()
    {
        for (int i = 0; i < weightcount - 1; i++)
        {
            TextMeshPro LineText = LetterPrefab.GetComponent<TextMeshPro>();
            LineText.text = ((char)(A + i)).ToString();
            Instantiate(LetterPrefab, new Vector3(transform.position.x + i + 0.5f, transform.position.y, transform.position.z + 0.5f), Quaternion.Euler(90, 0, 0), transform);

        }
        for (int i = 0; i < hightcount - 1; i++)
        {
            TextMeshPro LineText = LetterPrefab.GetComponent<TextMeshPro>();
            LineText.text = (i + 1).ToString();
            if (i > 8)
            {
                LineText.fontSize = 7;
            }
            Instantiate(LetterPrefab, new Vector3(transform.position.x - 0.5f, transform.position.y, transform.position.z - i - 0.5f), Quaternion.Euler(90, 0, 0), transform);
            LineText.fontSize = 10;
        }
    }

}
