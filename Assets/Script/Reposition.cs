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

        switch (transform.tag)
        {
            //자신의 태그가 Ground 라면
            case "Ground":

                //플레이어와 타일 간 거리 저장
                float diffx = playerPos.x - myPos.x;
                float diffy = playerPos.y - myPos.y;

                //플레이어가 타일보다 왼쪽에 있을 경우 음수, 아니라면 양수를 저장
                float dirX = diffx < 0 ? -1 : 1;
                float dirY = diffy < 0 ? -1 : 1;

                //거리를 절대값으로 저장
                diffx = Mathf.Abs(diffx);
                diffy = Mathf.Abs(diffy);

                //플레이어가 x축 방향으로 더 많이 이동했다면
                if (diffx > diffy)
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
                    Vector3 dist = playerPos - myPos;
                    Vector3 ran = new Vector3(Random.Range(-3, 3), Random.Range(-3, 3), 0);
                    //타일의 이동 거리만큼 몬스터도 함께 이동(약간의 랜덤 위치 포함)
                    transform.Translate(ran + dist * 2);
                }
                break;
        }
    }
}
