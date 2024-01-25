using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRandomize : MonoBehaviour
{

    public bool reroll = false;

    private void Awake()
    {
        Randomize();
    }

    private void OnValidate()
    {
        if (reroll)
        {
            Randomize();
        }
    }

    void Randomize()
    {
        transform.localScale = new Vector3(
    1 + Random.Range(-0.15f, 0.15f),
    1 + Random.Range(-0.15f, 0.15f),
    1 + Random.Range(-0.15f, 0.15f));

        transform.Rotate(0, Random.Range(0, 360.0f), 0);
    }
}
