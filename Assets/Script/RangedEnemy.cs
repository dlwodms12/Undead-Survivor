using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using static RangedEnemy;

public class RangedEnemy : Enemy
{
    //상태들이 공통으로 가지는 인터페이스
    public interface IRangedEnemyState
    {
        void Enter();    // 상태에 진입할 때 1번 실행
        void Execute();  // FixedUpdate에서 매 프레임 실행
        void Exit();     // 상태에서 빠져나갈 때 1번 실행
    }

    //사용할 상태 객체들 변수와 현재 상태 변수
    private IRangedEnemyState pursueState;
    private IRangedEnemyState fireState;
    private IRangedEnemyState currentState;

    [Header("# Ranged AI Settings")]
    public float attackRange = 6f;             // 플레이어 감지 레이더 사거리
    public float attackCooldown = 1.2f;        // 총알 발사 주기
    public int projectilePrefabId;             // PoolManager에서 가져올 몬스터 총알 ID

    private float damage; //총알 대미지

    // 부모의 Init을 오버라이드하여 상태를 초기화
    public override void Init(SpawnData data)
    {
        base.Init(data); // 부모의 기본 초기화 (체력, 속도 등) 호출

        // 스폰 시 상태 초기화
        SetupStates();
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

        SetupStates();
    }

    // 상태 클래스 생성 및 초기 상태 지정 함수
    private void SetupStates()
    {
        // 상태 객체가 없다면 처음 한 번만 생성
        if (pursueState == null) pursueState = new RangedPursueState(this);
        if (fireState == null) fireState = new RangedFireState(this);

        // 최초 상태는 추격으로 설정
        ChangeState(pursueState);
    }

    // 상태를 전환해주는 함수
    public void ChangeState(IRangedEnemyState newState)
    {
        //요청이 들어온 상태가 현재 상태와 동일하다면 무시
        if (currentState == newState) return;

        // 새로운 상태로 바꾸기 전에, 기존 상태의 Exit()를 호출
        // 현재는 따로 정리할 데이터가 없음
        // 추후 정리할 데이터가 생기면 작동
        if (currentState != null)
            currentState.Exit();

        // 상태 교체
        currentState = newState;

        // 새 상태의 Enter 호출
        if (currentState != null)
            currentState.Enter();
    }

    // 부모의 물리 업데이트를 오버라이드하여 상태 머신 가동
    protected override void FixedUpdate()
    {
        // 게임이 정지한 상태라면 업데이트 중단
        if (!GameManager.instance.isLive){ return; }
        // 살아있지 않거나 or Hit 애니메이션이 재생중이라면 이동 물리를 적용하지 않음
        if (!isLive || anim.GetCurrentAnimatorStateInfo(0).IsName("Hit")) { return; }

        //현재 상태에게 행동을 위임
        if (currentState != null)
        {
            currentState.Execute();
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

    //중첩 클래스

    //추격 상태
    public class RangedPursueState : IRangedEnemyState
    {
        private RangedEnemy enemy;
        public RangedPursueState(RangedEnemy enemy) { this.enemy = enemy; }

        public void Enter()
        {
            // 추후 애니메이션 추가 대비
            //enemy.anim.SetBool("Run", true);
        }

        public void Execute()
        {
            Vector2 dirVec = (Vector2)enemy.target.position - enemy.rigid.position;
            float distance = dirVec.magnitude;

            // [상태 전환 조건] 사거리 안으로 들어오면 사격 상태로 변경
            if (distance <= enemy.attackRange)
            {
                enemy.ChangeState(enemy.fireState);
                return;
            }

            // 플레이어 추격 이동 로직
            Vector2 nextVec = dirVec.normalized * enemy.speed * Time.fixedDeltaTime;
            enemy.rigid.MovePosition(enemy.rigid.position + nextVec);
        }

        public void Exit() { }
    }

    //사격 상태
    public class RangedFireState : IRangedEnemyState
    {
        private RangedEnemy enemy;
        private float attackTimer;

        public RangedFireState(RangedEnemy enemy) { this.enemy = enemy; }

        public void Enter()
        {
            // 사격 시작 시 정지 및 애니메이션 해제, 타이머 초기화
            enemy.rigid.velocity = Vector2.zero;
            //enemy.anim.SetBool("Run", false);
            attackTimer = 0f;
        }

        public void Execute()
        {
            Vector2 dirVec = (Vector2)enemy.target.position - enemy.rigid.position;
            float distance = dirVec.magnitude;

            // [상태 전환 조건] 사거리 밖으로 멀어지면 다시 추격 상태로 변경
            if (distance > enemy.attackRange)
            {
                enemy.ChangeState(enemy.pursueState);
                return;
            }

            // 공격 쿨다운 타이머 계산 및 발사
            attackTimer += Time.fixedDeltaTime;
            if (attackTimer >= enemy.attackCooldown)
            {
                attackTimer = 0f;
                enemy.Fire(dirVec.normalized);
            }
        }

        public void Exit() { }
    }
}


