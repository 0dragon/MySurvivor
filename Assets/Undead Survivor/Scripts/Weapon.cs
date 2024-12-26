using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public int id;
    public int prefabId;
    public float damage;
    public int count;
    public float speed;

    float timer;
    Player player;

    private void Awake()
    {
        player = GameManager.Instance.player;
    }

    void Update()
    {
        if (!GameManager.Instance.isLive)
            return;

        switch (id)
        {
            case 0:
                transform.Rotate(Vector3.back * speed * Time.deltaTime); // Vector3.back -> (0,0,-1) z축. deltaTime은 1프레임이 소비한 시간
                break;
            default:
                timer += Time.deltaTime;

                if (timer > speed){
                    timer = 0f;
                    Fire();
                }
                break;
        }

        // 테스트 코드
        /*
        if (Input.GetButtonDown("Jump"))
        {
            LevelUp(10, 1);
        }
        */
    }
    public void LevelUp(float damage, int count)
    {
        this.damage = damage * Character.Damage;
        this.count += count;

        if (id == 0)
            Batch();

        player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);
    }

    public void Init(ItemData data)
    {
        // Basic Set
        name = "Weapon " + data.itemId;
        transform.parent = player.transform;
        transform.localPosition = Vector3.zero;

        // Property Set
        id = data.itemId;
        damage = data.baseDamage * Character.Damage;
        count = data.baseCount + Character.Count;

        for(int index = 0; index<GameManager.Instance.pool.prefabs.Length; index++)
        {
            if(data.projectile == GameManager.Instance.pool.prefabs[index]){
                prefabId = index;
                break;
            }
        }

        switch (id)
        {
            case 0:
                speed = 150 * Character.WeaponSpeed;
                Batch();
                break;
            default:
                speed = 0.3f * Character.WeaponRate; // 0.3초마다 1번 발사. 총알은 speed 낮을수록 연사력 좋음.
                break;
        }

        // Hand Set
        Hand hand = player.hands[(int)data.itemType];
        hand.spriter.sprite = data.hand;
        hand.gameObject.SetActive(true);

        player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver); // 특정 함수 호출을 모든 자식에게 방송하는 함수
    }

    void Batch()
    {
        for(int index=0; index<count; index++)
        {
            Transform bullet;
            if(index < transform.childCount) // 기존 오브젝트를 먼저 활용하고 모자란 것은 폴링에서 가져오기
            {
                bullet = transform.GetChild(index);
            }
            else
            {
                bullet = GameManager.Instance.pool.Get(prefabId).transform;
                bullet.parent = transform; // 플레이어 자식의 Weapon 0에 들어간다.
            }

            bullet.localPosition = Vector3.zero;
            bullet.localRotation = Quaternion.identity; // 회전 값인 Quaternion의 초기값은 identity.

            Vector3 rotVec = Vector3.forward * 360 * index / count;
            bullet.Rotate(rotVec);
            bullet.Translate(bullet.up * 1.2f, Space.World);
            bullet.GetComponent<Bullet>().Init(damage, -100, Vector3.zero); // per 자리에 -100. -100은 무한 관통. -> 근접무기라서
        }
    }

    void Fire()
    {
        if (!player.scanner.nearestTarget)
            return;

        Vector3 targetPos = player.scanner.nearestTarget.position;
        Vector3 dir = targetPos - transform.position;
        dir = dir.normalized;

        Transform bullet = GameManager.Instance.pool.Get(prefabId).transform;
        bullet.position = transform.position;
        bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir); // 지정된 축을 중심으로 목표를 향해 회전
        bullet.GetComponent<Bullet>().Init(damage, count, dir);

        AudioManager.instance.PlaySfx(AudioManager.Sfx.Range);
    }
}
