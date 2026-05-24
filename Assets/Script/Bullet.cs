using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage; //총알 데미지
    public int per; //관통력

    Rigidbody2D rigid;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    //데미지와 관통력, 방향값을 전달받아 함수가 호출되면 총알을 발사
    public void Init(float damage, int per, Vector3 dir)
    {
        this.damage = damage;
        this.per = per;

        //근접 무기는 제외
        if(per > -1)
        {
            //총알에 벡터값 부여(방향+속도)
            rigid.velocity = dir*15f;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Enemy 태그가 아니거나 근접무기인 경우 아래 코드 실행X
        if (!collision.CompareTag("Enemy") || per == -1)
        {
            return;
        }

        //관통할 경우 관통값 --
        per--;

        //관통이 -1까지 낮아지면
        if(per == -1)
        {
            //벡터값을 0로 초기화하고
            rigid.velocity = Vector2.zero;
            //풀링에 반환
            gameObject.SetActive(false);
        }
    }

    //총알이 영역 바깥을 벗어날 경우 반환
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Area"))
        {
            rigid.velocity = Vector2.zero;
            gameObject.SetActive(false);
        }
    }

}
