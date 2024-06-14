using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public int enenmyCount = 50;

    public GameObject enenmyPrefab;

    Player player;

    int mazeWidth;
    int mazeHeight;

    private void Start()
    {

        mazeWidth = GameManager.Instance.MazeWidth;
        mazeHeight = GameManager.Instance.MazeHeight;

        player = GameManager.Instance.Player;

        // 적 생성
        for (int i = 0;i < enenmyCount; i++)
        {
            GameObject obj = Instantiate(enenmyPrefab, transform);
            obj.name = $"Enemy_{i}";
            Enemy enemy = obj.GetComponent<Enemy>();
            enemy.onDie += (target) =>
            {
                StartCoroutine(Respawn(target));
                GameManager.Instance.IncreaseKillCount();
            };

            enemy.Respawn(GetRandomSpawnPosition());
        }

  
    }

    /// <summary>
    /// 랜덤한 스폰 위치를 구하는 함수
    /// </summary>
    /// <returns>스폰 위치(미로 한 셀의 가운데 지점)</returns>
    Vector3 GetRandomSpawnPosition(bool init = false)
    {
        Vector2Int playerPosition;  // 플레이어의 그리드 위치

        if (init)
        {
            // 플레이어가 정상적으로 있다는 보장이 없는 경우 미로의 가운데 위치
            playerPosition = new(mazeWidth / 2, mazeHeight / 2);    
        }
        else
        {
            // 일반 플레이 중의 플레이어 그리드 위치
            playerPosition = MazeVisualizer.WorldToGrid(player.transform.position);
        }

        int x;
        int y;
        int limit = 100;
        do
        {
            // 플레이어의 위치에서 +-5 범위 안
            int index = Random.Range(0, mazeHeight * mazeWidth);
            x = index / mazeWidth;
            y = index % mazeHeight;

            limit--;
            if (limit < 1)
            {
                break;
            }
        }while(!(x<playerPosition.x +5 && x>playerPosition.x-5 && y<playerPosition.y +5 && y > playerPosition.y-5));

        Vector3 world = MazeVisualizer.GridToWorld(x, y);
        return world;
    }

    /// <summary>
    /// 일정시간 후에 target을 리스폰 시키는 코루틴
    /// </summary>
    /// <param name="target">리스폰 대상</param>
    /// <returns></returns>
    IEnumerator Respawn(Enemy target)
    {
        yield return new WaitForSeconds(3);
        target.Respawn(GetRandomSpawnPosition());
    }
}
