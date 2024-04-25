using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GridPainter : MonoBehaviour
{
    public GameObject Line;
    public GameObject Letter;

    char A = 'A';
    

    int hightcount = 11;
    int weightcount = 11;
    
    
    private void Start()
    {
        for(int i = 0; i < weightcount; i++)
        {
            Instantiate(Line, new Vector3(transform.position.x+i, transform.position.y, transform.position.z- 5), Quaternion.identity, transform);
            
        }
        for (int i = 0; i < hightcount; i++)
        {
            Instantiate(Line, new Vector3(transform.position.x+4, transform.position.y, transform.position.z - i), Quaternion.Euler(0,90,0), transform);

        }

        for (int i = 0; i < weightcount - 1; i++)
        {
            TextMeshPro LineText = Letter.GetComponent<TextMeshPro>();
            LineText.text = ((char)((int)A+i)).ToString();
            Instantiate(Letter, new Vector3(transform.position.x + i+0.5f, transform.position.y, transform.position.z +0.5f), Quaternion.Euler(90,0, 0), transform);

        }
        for (int i = 0; i < hightcount - 1; i++)
        {
            TextMeshPro LineText = Letter.GetComponent<TextMeshPro>();
            LineText.text = (i+1).ToString();
            Instantiate(Letter, new Vector3(transform.position.x -0.5f, transform.position.y, transform.position.z-i -0.5f), Quaternion.Euler(90, 0, 0), transform);

        }
    }
}
