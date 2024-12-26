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
        scanner = GetComponent<Scanner>(); // ���� ���� ��ũ��Ʈ�� ������Ʈó�� ������ �� �ִ�.
        hands = GetComponentsInChildren<Hand>(true); // true == ��Ȱ��ȭ�� ������Ʈ�� GetComponent ����
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

        // Input Manager ���
        // inputVec.x = Input.GetAxisRaw("Horizontal"); // Raw�� ���̸� ������ �������� �������� ������ �� �̲������� �� ����.
        // inputVec.y = Input.GetAxisRaw("Vertical");
    }

    void FixedUpdate()
    {
        if (!GameManager.Instance.isLive)
            return;

        /*
        // �÷��̾� �̵� ���.
        // 1. ���� �ش�.
        rigid.AddForce(inputVec);
        // 2. �ӵ� ����.
        rigid.velocity = inputVec;
        // 3. ��ġ �̵�.
        rigid.MovePosition(rigid.position + inputVec);
        */

        Vector2 nextVec = inputVec.normalized * speed * Time.fixedDeltaTime; // fixedDeltaTime == ���� ������ �ϳ��� �Һ��� �ð�.
        rigid.MovePosition(rigid.position + nextVec);
    }
    private void LateUpdate() // �������� ����Ǳ� ���� ����Ǵ� �����ֱ� �Լ�
    {
        if (!GameManager.Instance.isLive)
            return;

        anim.SetFloat("Speed", inputVec.magnitude);

        if (inputVec.x != 0)
        {
            spriter.flipX = inputVec.x < 0; // inputVec�� x���� ������ �� true ���� ��. -> SpriteRenderer�� flip x���� üũ��.
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
