using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reposition : MonoBehaviour
{
    Collider2D coll;

    void Awake()
    {
        coll = GetComponent<Collider2D>();
    }

    //트리거 바깥으로 나가면 실행
    private void OnTriggerExit2D(Collider2D collision)
    {
        //바깥쪽으로 나간 콜리전의 태그가 Area가 아니라면
        if(!collision.CompareTag("Area"))
        {
            return;
        }

        //플레이어의 좌표
        Vector3 playerPos = GameManager.instance.player.transform.position;
        //플레이어의 방향
        Vector3 playerDir = GameManager.instance.player.inputVec;
        //타일의 좌표
        Vector3 myPos = transform.position;

        //플레이어와 타일 간 거리를 절댓값으로 구해서 저장
        float diffx = Mathf.Abs(playerPos.x - myPos.x);
        float diffy = Mathf.Abs(playerPos.y - myPos.y);

        //플레이어의 방향을 저장
        float dirX = playerPos.x - myPos.x;
        float dirY = playerPos.y - myPos.y;

        //방향이 양의 방향이라면 1, 아니라면 -1을 반환
        dirX = dirX > 0 ? 1 : -1;
        dirY = dirY > 0 ? 1 : -1;

        switch (transform.tag)
        {
            //자신의 태그가 Ground 라면
            case "Ground":
                //플레이어가 x축 방향으로 더 많이 이동했다면
                if(diffx > diffy)
                {
                    //플레이어 방향으로 x축을 따라 40만큼 현재 타일을 이동
                    transform.Translate(Vector3.right * dirX * 40);
                }
                //플레이어가 y축 방향으로 더 많이 이동했다면
                else if(diffx < diffy)
                {
                    transform.Translate(Vector3.up * dirY * 40);
                }
                break;
            //자신의 태그가 Enemy 라면
            case "Enemy":
                //콜라이더가 살아있는 경우(생존 상태인 경우)
                if (coll.enabled)
                {
                    //플레이어 방향으로 20가량 이동(상세 위치는 조금씩 다르게 랜덤 지정)
                    transform.Translate(playerDir * 20 + new Vector3(Random.Range(-3f,3f), Random.Range(-3f, 3f), 0));
                }
                break;
        }
    }
}
