using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem;


public class Player : MonoBehaviour
{
    public Vector2 inputVec;
    public float speed;
    public Scanner scanner;
    public Hand[] hands;
    public RuntimeAnimatorController[] animcon;


    Rigidbody2D rigid;
    SpriteRenderer spriter;
    Animator anim;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        scanner = GetComponent<Scanner>();
        //불러오면서 비활성화된 오브젝트 활성화
        hands = GetComponentsInChildren<Hand>(true);
    }

    //활성화될 때 캐릭터 ID에 맞는 애니메이션 컨트롤러 작동
    private void OnEnable()
    {
        //Character 에 따라 speed 값을 조정
        speed *= Character.Speed;
        
        anim.runtimeAnimatorController = animcon[GameManager.instance.playerId];

    }

    void FixedUpdate()
    {
        //게임이 정지한 상태라면 업데이트 중단
        if (!GameManager.instance.isLive) { return; }

        Vector2 nextVec = inputVec * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
    }

    void LateUpdate()
    {
        //게임이 정지한 상태라면 업데이트 중단
        if (!GameManager.instance.isLive) { return; }

        //애니메이션 파라미터 값 할당
        anim.SetFloat("Speed", inputVec.magnitude);

        //반대 방향키가 눌릴 경우 이미지를 좌우반전(flip)
        if(inputVec.x != 0)
        {
            spriter.flipX = inputVec.x < 0;
        }
    }

    //피격 판정
    private void OnCollisionStay2D(Collision2D collision)
    {
        if(!GameManager.instance.isLive) { return; }

        //피격 함수 호출
        TakeDamage(Time.deltaTime * 10);
    }

    public void TakeDamage(float damage)
    {
        if (!GameManager.instance.isLive) { return; }

        // 체력 차감
        GameManager.instance.health -= damage;

        // 사망 조건 체크
        if (GameManager.instance.health <= 0)
        {
            // 체력이 마이너스로 내려가지 않게 0으로 고정
            GameManager.instance.health = 0;

            // 자식들(무기 등) 비활성화
            for (int index = 2; index < transform.childCount; index++)
            {
                transform.GetChild(index).gameObject.SetActive(false);
            }

            // Dead 애니메이션 재생 및 게임오버 호출
            anim.SetTrigger("Dead");
            GameManager.instance.GameOver();
        }
    }

    private void OnMove(InputValue value)
    {
        inputVec = value.Get<Vector2>();
    }
}
