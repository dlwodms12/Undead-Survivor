using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class RangedEnemy : Enemy
{
    //원거리 몬스터용 상태 정의
    //Pursue : 사거리 바깥의 플레이어를 추격
    //Fire : 사거리 안으로 들어오면 정지한 채 사격
    public enum State { Pursue, Fire }

    [Header("# Ranged AI Settings")]
    public State currentState = State.Pursue;  // 기본 상태는 추격
    public float attackRange = 6f;             // 플레이어 감지 레이더 사거리
    public float attackCooldown = 1.2f;        // 총알 발사 주기
    public int projectilePrefabId;             // PoolManager에서 가져올 몬스터 총알 ID

    private float attackTimer; //총알 발사 후 얼마나 시간이 지났는지 저장

    // 부모의 Init을 오버라이드하여 상태를 초기화
    public override void Init(SpawnData data)
    {
        base.Init(data); // 부모의 기본 초기화 (체력, 속도 등) 호출

        // 스폰 시 상태 초기화
        currentState = State.Pursue;
        attackTimer = 0f;
    }

    // 부모의 물리 업데이트를 오버라이드하여 상태 머신 가동
    protected override void FixedUpdate()
    {
        // 게임이 정지한 상태라면 업데이트 중단
        if (!GameManager.instance.isLive){ return; }
        // 살아있지 않거나 or Hit 애니메이션이 재생중이라면 이동 물리를 적용하지 않음
        if (!isLive || anim.GetCurrentAnimatorStateInfo(0).IsName("Hit")) { return; }

        // 플레이어(target)와의 거리 계산
        float distance = Vector2.Distance(target.position, rigid.position);
        Vector2 dirVec = target.position - rigid.position;

        // 거리 조건에 따른 상태 전환
        if (distance <= attackRange)
        {
            // 사거리 안으로 들어오면 발사 상태로 전환
            currentState = State.Fire;
        }
        else
        {
            // 사거리 밖으로 나가면 다시 추격 상태로 전환
            currentState = State.Pursue;
        }

        // 현재 상태에 따른 행동 수행
        switch (currentState)
        {
            case State.Pursue:
                // 플레이어를 향해 이동
                Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
                rigid.MovePosition(rigid.position + nextVec);

                // 애니메이션 제어 (걷기 켜기)
                anim.SetBool("Run", true);
                break;

            case State.Fire:
                // [발사 상태] 즉시 이동을 멈추고 고정 위치 유지
                rigid.velocity = Vector2.zero;

                // 애니메이션 제어 (걷기 끄기)
                anim.SetBool("Run", false);

                // 제자리에서 죽거나 플레이어가 죽을 때까지 타이머 기반 무한 발사
                attackTimer += Time.fixedDeltaTime;
                if (attackTimer >= attackCooldown)
                {
                    attackTimer = 0f;
                    Fire(dirVec.normalized);
                }
                break;
        }

        // 방향 전환 (어떤 상태든 항상 플레이어 쪽을 바라봄)
        spriter.flipX = target.position.x < rigid.position.x;
    }

    // 투사체 발사 로직
    void Fire(Vector2 fireDirection)
    {
        // 풀매니저에서 몬스터 총알 생성
        GameObject projectile = PoolManager.instance.Get(projectilePrefabId);
        projectile.transform.position = transform.position;

        // 몬스터 총알 컴포넌트를 가져와서 방향 전달
        EnemyBullet bullet = projectile.GetComponent<EnemyBullet>();
        if (bullet != null)
        {
            // 데미지는 하드코딩하지 않고 데이터화하거나 임의 지정 (예: 5)
            bullet.Init(5f, fireDirection);
        }
    }
}
