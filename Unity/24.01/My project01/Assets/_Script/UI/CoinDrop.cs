using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinDrop : MonoBehaviour
{
    public GameObject coinPrefab;
    Transform spawnPoint;
    Transform destination;

    float maxX = 8.0f;
    float minX = -8.0f;
    int maxCoins = 8;
    List<GameObject> coins = new List<GameObject>();

    float dropSpeed = 3.0f;
    float dropcooltiom = 0.7f;

    private void Awake()
    {
        spawnPoint = transform.GetChild(0);
        destination = transform.GetChild(1);
    }

    private void Start()
    {
        StartCoroutine( SpawnCoins());
    }

    void Update()
    {
        MoveCoins();
    }

    IEnumerator SpawnCoins()
    {
        for (int i = 0; i < maxCoins; i++)
        {
            yield return new WaitForSeconds(dropcooltiom);
            GameObject coin = Instantiate(coinPrefab, spawnPoint.position, Quaternion.identity);
            coin.SetActive(false);
            coins.Add(coin);
        }
    }

    void MoveCoins()
    {
        foreach (var coin in coins)
        {
            if (!coin.activeSelf)
            {
                coin.transform.position = new Vector3(Random.Range(minX, maxX), spawnPoint.position.y) ;
                coin.SetActive(true);
            }
            else
            {
                coin.transform.Translate(Vector3.down * dropSpeed * Time.deltaTime);

                if (coin.transform.position.y <= destination.position.y)
                {
                    coin.SetActive(false);
                }
            }
        }
    }
}