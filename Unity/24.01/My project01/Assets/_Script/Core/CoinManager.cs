using System.Collections;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    public GameObject coinPrefab;
    public int maxCoins = 10; // 최대 코인 수
    public float spawnInterval = 5f;
    public Collider2D spawnAreaCollider; // 코인이 생성될 범위를 설정하는 콜라이더
    public LayerMask excludedLayer; // 레이어를 제외할 때 사용할 레이어 마스크

    float block = 3.0f;
 

    private void Start()
    {
        StartCoroutine(SpawnCoinsCoroutine());
    }


    private IEnumerator SpawnCoinsCoroutine()
    {

        for(int i = 0; i < maxCoins; i++)
        {
            GameObject coin = Instantiate(coinPrefab, Vector3.zero, Quaternion.identity);
            coin.transform.parent = transform;
            coin.SetActive(false);


        }
        while (true)
        {
            if (FindInactiveCoin() != null)
            {
                GameObject coin = FindInactiveCoin();
                PlaceCoinRandomly(coin);
                coin.SetActive(true);
            }
            yield return new WaitForSeconds(spawnInterval);
        }
        
    }




    private GameObject FindInactiveCoin()
    {
        // 씬에서 비활성화된 코인 찾기
        if (transform != null)
        {
            foreach (Transform child in transform)
            {
                if (!child.gameObject.activeSelf)
                {
                    return child.gameObject;
                }
            }
        }

        return null;
    }

    private void PlaceCoinRandomly(GameObject coin)
    {
        // 랜덤한 위치로 이동
        Collider2D collider = coin.GetComponentInChildren<Collider2D>();
        if (collider != null && spawnAreaCollider != null)
        {
            Vector2 randomPosition = GetRandomPositionInsideCollider(collider, spawnAreaCollider);
            coin.transform.position = randomPosition;
        }
    }

    private Vector2 GetRandomPositionInsideCollider(Collider2D collider, Collider2D spawnAreaCollider)
    {
        // 이전과 동일한 랜덤 위치 생성 메서드 활용
        Vector2 randomPosition = new Vector2(
            Random.Range(spawnAreaCollider.bounds.min.x+ block, spawnAreaCollider.bounds.max.x- block),
            Random.Range(spawnAreaCollider.bounds.min.y+ block, spawnAreaCollider.bounds.max.y- block)
        );

        // Wall 레이어에 해당하는 오브젝트가 있는지 체크
        if (Physics2D.OverlapPoint(randomPosition, excludedLayer) != null)
        {
            return GetRandomPositionInsideCollider(collider, spawnAreaCollider);
        }


        return randomPosition;
    }
}
