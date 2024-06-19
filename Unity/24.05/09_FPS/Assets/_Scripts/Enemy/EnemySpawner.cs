using System;
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

    Enemy[] enemies;

    public Action onSpawnCompleted;
    private void Awake()
    {
        enemies = new Enemy[enenmyCount];

    }
    private void Start()
    {

        mazeWidth = GameManager.Instance.MazeWidth;
        mazeHeight = GameManager.Instance.MazeHeight;

        player = GameManager.Instance.Player;

        GameManager.Instance.onGameStart += EnemyAll_Play;
        GameManager.Instance.onGameEnd += (_) => EnemyAll_Stop();

        

        
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
        float halfSize = Mathf.Min(mazeWidth, mazeHeight) * 0.5f;
        do
        {
            // 플레이어의 위치에서 +-5 범위 안
            int index = UnityEngine.Random.Range(0, mazeHeight * mazeWidth);
            x = index / mazeWidth;
            y = index % mazeHeight;

            limit--;
            if (limit < 1)
            {
                break;
            }
        }while(!(x<playerPosition.x + halfSize && x>playerPosition.x- halfSize && y<playerPosition.y + halfSize && y > playerPosition.y- halfSize));

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

    /// <summary>
    /// 적 모두 생성 (맵생성후 호출)
    /// </summary>
    public void EnemyAll_Spawn()
    {
        // 적 생성
        for (int i = 0; i < enenmyCount; i++)
        {
            GameObject obj = Instantiate(enenmyPrefab, transform);
            obj.name = $"Enemy_{i}";
            enemies[i] = obj.GetComponent<Enemy>();
            enemies[i].onDie += (target) =>
            {
                StartCoroutine(Respawn(target));
                GameManager.Instance.IncreaseKillCount();
            };

            enemies[i].Respawn(GetRandomSpawnPosition(true),true);
        }

        onSpawnCompleted?.Invoke();// 스폰이 완료되면알림
    }

    /// <summary>
    /// 모든 적을 움직이게 만들기
    /// </summary>
    void EnemyAll_Play()
    {
        foreach (var enemy in enemies)
        {
            enemy.Play();   // Wander상태로 변경
        }
    }

    /// <summary>
    /// 모든 적을 일시정지
    /// </summary>
    private void EnemyAll_Stop()
    {
        foreach (var enemy in enemies)
        {
            enemy.Stop();   // Idle 상태로 변경
        }
    }
}
