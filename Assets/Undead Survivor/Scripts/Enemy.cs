using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed;
    public float health;
    public float maxHealth;
    public RuntimeAnimatorController[] animCon;
    public Rigidbody2D target;

    bool isLive;

    Rigidbody2D rigid;
    Collider2D coll;
    Animator anim;
    SpriteRenderer spriter;
    WaitForFixedUpdate wait;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        spriter = GetComponent<SpriteRenderer>();
        wait = new WaitForFixedUpdate();
    }
    void FixedUpdate()
    {
        if (!GameManager.Instance.isLive)
            return;

        if (!isLive || anim.GetCurrentAnimatorStateInfo(0).IsName("Hit")) // 현재 상태 정보를 가져오는 함수. Hit 상태가 켜져있을 때 해당 로직 스킵.
        {
            return;
        }
        Vector2 dirVec = target.position - rigid.position;
        Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime; // 적이 플레이어 따라오게 함.
        rigid.MovePosition(rigid.position + nextVec);
        rigid.velocity = Vector2.zero;
    }

    void LateUpdate()
    {
        if (!GameManager.Instance.isLive)
            return;

        if (!isLive)
        {
            return;
        }
        spriter.flipX = target.position.x < rigid.position.x;
    }

    // 프리펩 상태의 몹은 게임 내의 플레이어를 타겟으로 참조할 수 없다.
    // 따라서 프리펩의 몹이 인게임으로 소환되면 플레이어를 자동으로 타켓으로 정하게 해야한다.
    void OnEnable()
    {
        target = GameManager.Instance.player.GetComponent<Rigidbody2D>();
        isLive = true;
        coll.enabled = true;
        rigid.simulated = true;
        spriter.sortingOrder = 2; // 스프라이트 렌더러에 Order in Layer.
        anim.SetBool("Dead", false);
        health = maxHealth; // 죽었다가 다시 호출되어 소환되었을 때 체력 최대 체력으로 초기화. -방지
    }
    public void Init(SpawnData data)
    {
        anim.runtimeAnimatorController = animCon[data.spriteType];
        speed = data.speed;
        maxHealth = data.health;
        health = data.health;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Bullet") || !isLive)
            return;

        health -= collision.GetComponent<Bullet>().damage;
        StartCoroutine(KnockBack()); // StartCoroutine("KonckBack"); 도 가능

        if(health > 0)
        {
            // 살았고, hit 액션
            anim.SetTrigger("Hit");

            AudioManager.instance.PlaySfx(AudioManager.Sfx.Hit);
        }
        else
        {
            isLive = false;
            coll.enabled = false;
            rigid.simulated = false;
            spriter.sortingOrder = 1;
            anim.SetBool("Dead", true);
            GameManager.Instance.kill++;
            GameManager.Instance.GetExp();

            if (GameManager.Instance.isLive == true)
            {
                AudioManager.instance.PlaySfx(AudioManager.Sfx.Dead);
            }
                
        }
    }

    // I로 시작하는건 인터페이스. 코루틴
    IEnumerator KnockBack()
    {
        yield return wait; // 다음 하나의 물리 프레임 딜레이
        Vector3 playerPos = GameManager.Instance.player.transform.position;
        Vector3 dirVec = transform.position - playerPos;
        rigid.AddForce(dirVec.normalized * 3, ForceMode2D.Impulse); // 밀리는 정도 3. 즉발적인 힘 Impulse
    }

    void Dead()
    {
        gameObject.SetActive(false);
    }
}
