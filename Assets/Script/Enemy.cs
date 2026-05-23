using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed;
    public Rigidbody2D target;

    bool isLive = true;

    Rigidbody2D rigid;
    SpriteRenderer spriter;

    // Start is called before the first frame update
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {

        if(!isLive)
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

    void OnEnable()
    {
        target = GameManager.instance.player.GetComponent<Rigidbody2D>();
    }
}
