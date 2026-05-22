using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reposition : MonoBehaviour
{
    //트리거 바깥으로 나가면 실행
    private void OnTriggerExit2D(Collider2D collision)
    {
        //바깥쪽으로 나간 콜리전의 태그가 Area가 아니라면(Player 외의 오브젝트가 나간거라면)
        if(!collision.CompareTag("Area"))
        {
            return;
        }

        //플레이어의 좌표
        Vector3 playerPos = GameManager.instance.player.transform.position;
        //타일의 좌표
        Vector3 myPos = transform.position;

        //좌표 간의 거리를 절댓값으로 구해서 저장
        float diffx = Mathf.Abs(playerPos.x - myPos.x);
        float diffy = Mathf.Abs(playerPos.y - myPos.y);

        //플레이어의 방향
        Vector3 playerDir = GameManager.instance.player.inputVec;
        //방향이 음수를 향하고 있다면 -1을 반환, 그렇지 않다면 1을 반환
        float dirX = playerDir.x < 0 ? -1 : 1;
        float dirY = playerDir.y < 0 ? -1 : 1;

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
            case "Enemy":

                break;
        }
    }
}
