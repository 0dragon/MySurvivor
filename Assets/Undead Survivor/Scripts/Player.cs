using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public Vector2 inputVec;
    public float speed;
    public Scanner scanner;
    public Hand[] hands;
    public RuntimeAnimatorController[] animCon;

    Rigidbody2D rigid;
    SpriteRenderer spriter;
    Animator anim;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        scanner = GetComponent<Scanner>(); // 내가 만든 스크립트도 컴포넌트처럼 가져올 수 있다.
        hands = GetComponentsInChildren<Hand>(true); // true == 비활성화된 오브젝트도 GetComponent 가능
    }

    void OnEnable()
    {
        speed = speed * Character.Speed; // Character.cs
        anim.runtimeAnimatorController = animCon[GameManager.Instance.playerId];
    }

    void Update()
    {
        if (!GameManager.Instance.isLive)
            return;

        // Input Manager 방식
        // inputVec.x = Input.GetAxisRaw("Horizontal"); // Raw를 붙이면 보정이 없어져서 움직임을 멈췄을 때 미끄러지는 것 방지.
        // inputVec.y = Input.GetAxisRaw("Vertical");
    }

    void FixedUpdate()
    {
        if (!GameManager.Instance.isLive)
            return;

        /*
        // 플레이어 이동 방법.
        // 1. 힘을 준다.
        rigid.AddForce(inputVec);
        // 2. 속도 제어.
        rigid.velocity = inputVec;
        // 3. 위치 이동.
        rigid.MovePosition(rigid.position + inputVec);
        */

        Vector2 nextVec = inputVec.normalized * speed * Time.fixedDeltaTime; // fixedDeltaTime == 물리 프레임 하나가 소비한 시간.
        rigid.MovePosition(rigid.position + nextVec);
    }
    private void LateUpdate() // 프레임이 종료되기 직전 실행되는 생명주기 함수
    {
        if (!GameManager.Instance.isLive)
            return;

        anim.SetFloat("Speed", inputVec.magnitude);

        if (inputVec.x != 0)
        {
            spriter.flipX = inputVec.x < 0; // inputVec의 x값이 음수일 때 true 값이 됨. -> SpriteRenderer의 flip x값이 체크됨.
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (!GameManager.Instance.isLive)
            return;

        GameManager.Instance.health -= Time.deltaTime * 10;

        if(GameManager.Instance.health < 0)
        {
             for(int i=2; i<transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }

            anim.SetTrigger("Dead");
            GameManager.Instance.GameOver();
        }
    }

    void OnMove(InputValue value)
    {
        inputVec = value.Get<Vector2>();
    }
}
