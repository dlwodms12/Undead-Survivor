using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed;
    public float health;
    public float maxHealth;

    //애니메이션 컨트롤러 변수
    public RuntimeAnimatorController[] animCon;
    public Rigidbody2D target;

    bool isLive;

    Rigidbody2D rigid;
    SpriteRenderer spriter;
    Animator anim;
    WaitForFixedUpdate wait; //코루틴용

    // Start is called before the first frame update
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        wait = new WaitForFixedUpdate();
    }

    void FixedUpdate()
    {
        //살아 있지 않거나 or Hit 애니메이션이 재생중이라면 이동 물리를 적용하지 않음
        //GetCurrntAnimatorStateInfo(index) : 현재 실행중인 애니메이션 레이어 정보를 가져옴
        if(!isLive || anim.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
        {
            return;
        }

        //방향벡터 = 타겟(플레이어) 위치 - 내(적)위치
        Vector2 dirVec = target.position - rigid.position;

        //방향벡터(정규화) = 방향 X speed
        Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;

        //현재 위치에서 이동
        rigid.MovePosition(rigid.position + nextVec);

        //물리 속도가 이동에 영향을 주지 않도록 속도 제거
        rigid.velocity = Vector2.zero;
    }

    void LateUpdate()
    {
        if (!isLive)
        {
            return;
        }
        spriter.flipX = target.position.x < rigid.position.x;
    }

    //생성됐을 때 변수 초기화 실행
    void OnEnable()
    {
        target = GameManager.instance.player.GetComponent<Rigidbody2D>();
        isLive = true;
        health = maxHealth;
    }

    //레벨링에 따른 Enemy의 변화를 SpawnData를 이용해 반영
    //Spawner.cs에서 호출
    public void Init(SpawnData data)
    {
        anim.runtimeAnimatorController = animCon[data.spriteType];
        speed = data.speed;
        maxHealth = data.health;
        health = data.health;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //충돌한 것이 총알이 아니라면 무시
        if (!collision.CompareTag("Bullet"))
        {
            return;
        }

        //총알 데미지만큼 체력 감소
        health -= collision.GetComponent<Bullet>().damage;

        //넉백 적용
        StartCoroutine(KnockBack());

        if(health > 0)
        {
            //히트액션
            anim.SetTrigger("Hit");
        }

        else
        {
            Dead();
        }
    }

    //코루틴 반환형 인터페이스
    IEnumerator KnockBack()
    {
        yield return wait; // 다음 하나의 물리 프레임까지 대기
        Vector3 playerPos = GameManager.instance.player.transform.position;
        Vector3 dirVec = transform.position - playerPos;
        //플레이어 반대 방향으로 넉백
        rigid.AddForce(dirVec.normalized * 3, ForceMode2D.Impulse);

    }

    void Dead()
    {
        gameObject.SetActive(false);
    }

}
