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
                transform.Rotate(Vector3.back * speed * Time.deltaTime); // Vector3.back -> (0,0,-1) z��. deltaTime�� 1�������� �Һ��� �ð�
                break;
            default:
                timer += Time.deltaTime;

                if (timer > speed){
                    timer = 0f;
                    Fire();
                }
                break;
        }

        // �׽�Ʈ �ڵ�
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
                speed = 0.3f * Character.WeaponRate; // 0.3�ʸ��� 1�� �߻�. �Ѿ��� speed �������� ����� ����.
                break;
        }

        // Hand Set
        Hand hand = player.hands[(int)data.itemType];
        hand.spriter.sprite = data.hand;
        hand.gameObject.SetActive(true);

        player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver); // Ư�� �Լ� ȣ���� ��� �ڽĿ��� ����ϴ� �Լ�
    }

    void Batch()
    {
        for(int index=0; index<count; index++)
        {
            Transform bullet;
            if(index < transform.childCount) // ���� ������Ʈ�� ���� Ȱ���ϰ� ���ڶ� ���� �������� ��������
            {
                bullet = transform.GetChild(index);
            }
            else
            {
                bullet = GameManager.Instance.pool.Get(prefabId).transform;
                bullet.parent = transform; // �÷��̾� �ڽ��� Weapon 0�� ����.
            }

            bullet.localPosition = Vector3.zero;
            bullet.localRotation = Quaternion.identity; // ȸ�� ���� Quaternion�� �ʱⰪ�� identity.

            Vector3 rotVec = Vector3.forward * 360 * index / count;
            bullet.Rotate(rotVec);
            bullet.Translate(bullet.up * 1.2f, Space.World);
            bullet.GetComponent<Bullet>().Init(damage, -100, Vector3.zero); // per �ڸ��� -100. -100�� ���� ����. -> ���������
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
        bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir); // ������ ���� �߽����� ��ǥ�� ���� ȸ��
        bullet.GetComponent<Bullet>().Init(damage, count, dir);

        AudioManager.instance.PlaySfx(AudioManager.Sfx.Range);
    }
}
