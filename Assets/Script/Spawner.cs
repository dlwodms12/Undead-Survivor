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
        level = Mathf.Min(Mathf.FloorToInt(GameManager.instance.gameTime / levelTime),spawnData.Length-1);

        //레벨에 따른 스폰 타임 변경
        if(timer > spawnData[level].spawnTime)
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
        enemy.transform.position = spawnPoint[Random.Range(1,spawnPoint.Length)].position;
        //레벨에 따라 다른 Enemy spawnData[]를 가져와 적용시킴
        enemy.GetComponent<Enemy>().Init(spawnData[level]);
    }

    void HandleRangedSpawn()
    {
        float gameTime = GameManager.instance.gameTime;

        //1분(60초) 미만일 때는 아예 스폰하지 않음
        if (gameTime < 60f) return;

        //시간에 따른 스폰 주기(Interval) 결정
        float spawnInterval = 2f; // 기본값: 1분~2분 사이에는 2초에 한 마리

        if (gameTime >= 120f)
        {
            spawnInterval = 1f;   // 2분이 넘어가면 1초에 한 마리
        }

        // 원거리 전용 타이머 작동
        rangedTimer += Time.deltaTime;

        if (rangedTimer >= spawnInterval)
        {
            SpawnRanged();
            rangedTimer = 0f; // 타이머 초기화
        }
    }

    // 원거리 몬스터 실제 생성 함수
    void SpawnRanged()
    {
        // 원거리 프리팹 ID로 풀링 호출
        GameObject enemy = PoolManager.instance.Get(rangedPrefabId);

        // 랜덤한 위치에 배치
        enemy.transform.position = spawnPoint[Random.Range(1, spawnPoint.Length)].position;

        // 레벨에 따른 spawnData 연동하여 생성
        enemy.GetComponent<Enemy>().Init(spawnData[level]);
    }
}

//직렬화
[System.Serializable]
public class SpawnData
{
    public float spawnTime;
    public int spriteType; 
    public int health;
    public float speed;
}
