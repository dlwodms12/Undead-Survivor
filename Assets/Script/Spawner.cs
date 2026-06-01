using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Transform[] spawnPoint;
    public SpawnData[] spawnData;

    public float levelTime;

    int level;
    float timer;

    [Header("# Ranged Spawn Settings")]
    public int rangedPrefabId; // PoolManager에서 원거리 몬스터가 몇 번 프리팹인지 지정
    float rangedTimer;         // 원거리 몬스터 전용 타이머
    public RangedSpawnData[] rangedSpawnData; //원거리 몬스터 밸런스 설정을 위한 배열

    void Awake()
    {
        //Spawner의 본인 + 자식 컴포넌트(스폰 포인트)를 가져와서 배열에 할당
        spawnPoint = GetComponentsInChildren<Transform>();

        //게임 최대 시간을 몬스터 데이터 크기로 나누어 자동으로 레벨 구간을 계산
        levelTime = GameManager.instance.maxGameTime / spawnData.Length;
    }

    // Update is called once per frame
    void Update()
    {
        //게임이 정지한 상태라면 업데이트 중단
        if (!GameManager.instance.isLive) { return; }

        timer += Time.deltaTime;
        //시간에 흐름에 따라 레벨이 올라감
        level = Mathf.Min(Mathf.FloorToInt(GameManager.instance.gameTime / levelTime), spawnData.Length - 1);

        //레벨에 따른 스폰 타임 변경
        if (timer > spawnData[level].spawnTime)
        {
            SpawnMelee();
            timer = 0;
        }

        //원거리 몬스터 스폰
        HandleRangedSpawn();
    }

    void SpawnMelee()
    {
        //근접 몬스터 풀에서 꺼내오기
        GameObject enemy = PoolManager.instance.Get(0);

        //랜덤한 스폰 포인트에 적 오브젝트 생성 (본인(Spawner)를 제외하고 자식 오브젝트(Point)에 생성)
        enemy.transform.position = spawnPoint[Random.Range(1, spawnPoint.Length)].position;
        //레벨에 따라 다른 Enemy spawnData[]를 가져와 적용시킴
        enemy.GetComponent<Enemy>().Init(spawnData[level]);
    }

    void HandleRangedSpawn()
    {
        //데이터 세팅되지 않았을 경우 실행X
        if (rangedSpawnData == null || rangedSpawnData.Length == 0) return;

        float gameTime = GameManager.instance.gameTime;

        // 첫 번째 단계 시간보다 전이면 아예 스폰하지 않음
        if (gameTime < rangedSpawnData[0].appearanceTime) return;

        //시간에 따른 스폰 주기(Interval) 결정
        int currentTier = 0;
        for (int i = 0; i < rangedSpawnData.Length; i++)
        {
            if (gameTime >= rangedSpawnData[i].appearanceTime)
            {
                currentTier = i; // 매칭되는 가장 높은 단계를 선택
            }
            else
            {
                break; // 시간을 초과하면 루프 종료
            }
        }

        // 현재 단계의 밸런스 데이터 가져오기
        RangedSpawnData currentData = rangedSpawnData[currentTier];

        rangedTimer += Time.deltaTime;

        // 인스펙터에 적어둔 스폰 주기를 사용합니다!
        if (rangedTimer >= currentData.spawnInterval)
        {
            SpawnRanged(currentData); // 데이터를 소환 함수로 토스!
            rangedTimer = 0f;
        }

        // 원거리 몬스터 실제 생성 함수
        void SpawnRanged(RangedSpawnData data)
        {
            GameObject enemy = PoolManager.instance.Get(rangedPrefabId);
            enemy.transform.position = spawnPoint[Random.Range(1, spawnPoint.Length)].position;

            // 원거리 몬스터 전용 초기화 함수 호출
            enemy.GetComponent<RangedEnemy>().InitRanged(data);
        }
    }
}
//근거리 몬스터
[System.Serializable]
public class SpawnData
{
    public float spawnTime;
    public int spriteType; 
    public int health;
    public float speed;
}

//원거리 몬스터 구조체
[System.Serializable]
public struct RangedSpawnData
{
    public float appearanceTime; // 출현 시작 시간
    public float spawnInterval;  // 스폰 주기
    public float health;         // 원거리 몬스터 체력
    public float speed;          // 원거리 몬스터 이동 속도
    public float bulletDamage;   // 총알의 데미지
    public float attackRange;    // 공격 사거리
    public float attackCooldown; // 공격 쿨타임
}
