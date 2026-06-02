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

    private float damage; //총알 대미지
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

                // 애니메이션 제어
                anim.SetBool("Run", true);
                break;

            case State.Fire:
                // [발사 상태] 즉시 이동을 멈추고 고정 위치 유지
                rigid.velocity = Vector2.zero;

                // 애니메이션 제어
                anim.SetBool("Run", false);

                // 본인이 죽거나 플레이어가 죽을 때까지 발사
                attackTimer += Time.fixedDeltaTime;
                if (attackTimer >= attackCooldown)
                {
                    attackTimer = 0f;
                    Fire(dirVec.normalized);
                }
                break;
        }

        // 방향 전환
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
            // 총알 세팅
            bullet.Init(damage, fireDirection);
        }
    }

    //Spawner에서 호출해서 사용
    public void InitRanged(RangedSpawnData data)
    {
        // 부모(Enemy)로부터 물려받은 기본 능력치 세팅
        maxHealth = data.health;
        health = data.health;
        speed = data.speed;

        // RangedEnemy 밸런스 변수 세팅
        damage = data.bulletDamage;     // 총알 대미지
        attackRange = data.attackRange; // 사거리
        attackCooldown = data.attackCooldown; // 공격 속도

        // 상태 초기화
        currentState = State.Pursue;
        attackTimer = 0f;
    }

    //원거리 몬스터용 코인 드랍 함수
    protected override void DropExpCoin()
    {
        // 풀매니저에서 원거리용 코인 인스턴스 팝업
        GameObject coinObj = PoolManager.instance.Get(expCoinPrefabId);
        coinObj.transform.position = transform.position;

        ExpCoin coin = coinObj.GetComponent<ExpCoin>();
        if (coin != null)
        {
            // 원거리 전용 데이터 초기화 호출
            coin.InitRanged();
        }
    }
}
