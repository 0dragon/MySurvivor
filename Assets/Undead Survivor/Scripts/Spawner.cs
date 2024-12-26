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

        level = Mathf.Min(Mathf.FloorToInt(GameManager.Instance.gameTime / levelTime), spawnData.Length -1); // �Ǽ����� ����(�ø���CeilToInt)���� ������ ��ȯ

        if (timer > spawnData[level].spawnTime) // �������� ���� �����Ϳ� ���� �������� �˾Ƽ� �ٸ��� ����
        {
            timer = 0;
            Spawn();
        }
    }
    void Spawn()
    {
        GameObject enemy = GameManager.Instance.pool.Get(0); // ������ �ٸ� ���� ���� ����
        enemy.transform.position = spawnPoint[Random.Range(1,spawnPoint.Length)].position;
        enemy.GetComponent<Enemy>().Init(spawnData[level]);
    }
}

[System.Serializable] // ����ȭ
// ����ȭ�� ���� ������ �ν����� â���� �ش� �����͵��� ������ �ʴ´�.
// ������ ������ ���� Ÿ���� ����ȭ�� ���� �����ؼ� �� �� �ִ�.
public class SpawnData
{
    public float spawnTime;
    public int spriteType;
    public int health;
    public float speed;
}