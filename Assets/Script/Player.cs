using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

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

    // Update is called once per frame
    void Update()
    {
        //게임이 정지한 상태라면 업데이트 중단
        if (!GameManager.instance.isLive) { return; }

        inputVec.x = Input.GetAxisRaw("Horizontal");
        inputVec.y = Input.GetAxisRaw("Vertical");
    }

    void FixedUpdate()
    {
        //게임이 정지한 상태라면 업데이트 중단
        if (!GameManager.instance.isLive) { return; }

        Vector2 nextVec = inputVec.normalized * speed * Time.fixedDeltaTime;
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

        GameManager.instance.health -= Time.deltaTime * 10;

        if(GameManager.instance.health < 0)
        {
            //player의 자식인 Shadow와 Area는 건너뛰고 선택하기 위해 index = 2 부터 시작
            for(int index = 2; index < transform.childCount; index++)
            {
                //자식들 비활성화
                transform.GetChild(index).gameObject.SetActive(false);
            }

            //Dead 애니메이션 재생
            anim.SetTrigger("Dead");
            //게임 오버 호출
            GameManager.instance.GameOver();
        }

    }
}
