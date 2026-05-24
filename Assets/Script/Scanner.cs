using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scanner : MonoBehaviour
{
    public float scanRange; //스캔 범위
    public LayerMask targetLayer; //타겟으로 하는 레이어
    public RaycastHit2D[] targets; // 스캔 결과 배열
    public Transform nearestTarget; // 플레이어와 가장 가까운 적

    private void FixedUpdate()
    {
        //원형형태로 검색(캐스팅 시작 위치, 원의 반지름, 캐스팅 방향, 캐스팅 길이, 대상 레이어)
        targets = Physics2D.CircleCastAll(transform.position, scanRange, Vector2.zero, 0, targetLayer);
        nearestTarget = GetNearest();
    }

    Transform GetNearest()
    {
        Transform result = null;

        float diff = 100;

        //스캔 결과 순회
        foreach (RaycastHit2D target in targets)
        {
            Vector3 myPos = transform.position;
            Vector3 targetPos = target.transform.position;
            //자신과 타겟간의 거리를 계산
            float curDiff = Vector3.Distance(myPos, targetPos);
            //타겟과의 거리가 사거리보다 짧다면
            if(curDiff < diff)
            {
                //사거리를 타겟과의 거리로 업데이트
                diff = curDiff;
                //조준한 위치를 반환
                result = target.transform;
            }
        }

        return result;
    }

}
