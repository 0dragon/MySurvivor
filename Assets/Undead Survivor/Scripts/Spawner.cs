using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Transform[] spawnPoint;
    public SpawnData[] spawnData;
    public float levelTime;

    int level;
    float timer;

    void Awake()
    {
        spawnPoint =GetComponentsInChildren<Transform>();
        levelTime = GameManager.Instance.maxGameTime / spawnData.Length;
    }
    void Update()
    {
        if (!GameManager.Instance.isLive)
            return;

        timer += Time.deltaTime;

        level = Mathf.Min(Mathf.FloorToInt(GameManager.Instance.gameTime / levelTime), spawnData.Length -1); // 실수형을 버림(올림은CeilToInt)으로 정수형 변환

        if (timer > spawnData[level].spawnTime) // 스포너의 스폰 데이터에 따라 레벨별로 알아서 다르게 스폰
        {
            timer = 0;
            Spawn();
        }
    }
    void Spawn()
    {
        GameObject enemy = GameManager.Instance.pool.Get(0); // 레벨에 다른 몬스터 선택 스폰
        enemy.transform.position = spawnPoint[Random.Range(1,spawnPoint.Length)].position;
        enemy.GetComponent<Enemy>().Init(spawnData[level]);
    }
}

[System.Serializable] // 직렬화
// 직렬화를 하지 않으면 인스펙터 창에서 해당 데이터들이 보이지 않는다.
// 복잡한 구조를 가진 타입은 직렬화를 통해 정리해서 볼 수 있다.
public class SpawnData
{
    public float spawnTime;
    public int spriteType;
    public int health;
    public float speed;
}