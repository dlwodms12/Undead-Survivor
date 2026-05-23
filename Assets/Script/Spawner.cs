using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{

    public Transform[] spawnPoint;
    public SpawnData[] spawnData;

    int level;
    float timer;

    void Awake()
    {
        //Spawner의 본인 + 자식 컴포넌트(스폰 포인트)를 가져와서 배열에 할당
        spawnPoint = GetComponentsInChildren<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        //시간에 흐름에 따라 레벨이 올라감
        level = Mathf.FloorToInt(GameManager.instance.gameTime / 10f);

        if(timer > (level == 0 ? 0.5f : 0.2f))
        {
            Spawn();
            timer = 0;
        }
    }

    void Spawn()
    {
        //적 오브젝트를 게임 매니저를 통해 가져와서 할당
        //레벨이 변하면 할당하는 오브젝트도 달라짐
        GameObject enemy = GameManager.instance.pool.Get(level);
        //랜덤한 스폰 포인트에 적 오브젝트 생성 (본인(Spawner)를 제외하고 자식 오브젝트(Point)에 생성)
        enemy.transform.position = spawnPoint[Random.Range(1,spawnPoint.Length)].position;
    }
}

//직렬화
[System.Serializable]
public class SpawnData
{
    public int spriteType;
    public float spawnTime;
    public int health;
    public float speed;
}
