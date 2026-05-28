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
            Spawn();
            timer = 0;
        }
    }

    void Spawn()
    {
        GameObject enemy = GameManager.instance.pool.Get(0);
        //랜덤한 스폰 포인트에 적 오브젝트 생성 (본인(Spawner)를 제외하고 자식 오브젝트(Point)에 생성)
        enemy.transform.position = spawnPoint[Random.Range(1,spawnPoint.Length)].position;
        //레벨에 따라 다른 Enemy spawnData[]를 가져와 적용시킴
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
