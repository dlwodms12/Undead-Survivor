using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public int id; //무기 ID
    public int prefabId; //무기 프리팹 ID
    public float damage;  //무기 데미지
    public int count; //소환할 무기 개수
    public float speed; //무기 회전 속도 or 연사속도

    float timer;

    Player player;

    void Awake()
    {
        player = GameManager.instance.player;
    }

    // Update is called once per frame
    void Update()
    {
        switch (id)
        {
            case 0:
                transform.Rotate(Vector3.back * speed * Time.deltaTime);
                break;

            default:
                //시간을 누적해서 speed보다 시간이 많이 흘렀다면 발사
                timer += Time.deltaTime;
                if(timer > speed)
                {
                    timer = 0;
                    Fire();
                }

                break;
        }

        //Test Code
        if (Input.GetButtonDown("Jump"))
        {
            LevelUp(10, 1);
        }

    }

    public void LevelUp(float damage, int count)
    {
        this.damage = damage;
        this.count += count;

        if(id == 0)
        {
            Batch();
        }
    }

    public void Init(ItemData data)
    {
        //무기를 부모(Player)에 붙임
        name = "Weapon" + data.itemId;
        transform.parent = player.transform;
        transform.localPosition = Vector3.zero;

        //Property Set
        id = data.itemId;
        damage = data.baseDamage;
        count = data.baseCount;

        //풀 길이에 맞춰 돌면서
        for(int index = 0; index < GameManager.instance.pool.prefabs.Length; index++)
        {
            //무기 오브젝트를 풀에서 찾았다면
            if(data.projectile == GameManager.instance.pool.prefabs[index])
            {
                //프리팹 아이디를 저장
                prefabId = index;
                break;  
            }
        }

        switch (id)
        {
            case 0:
                speed = 150;
                Batch();

                break;

            default:
                speed = 0.3f; //연사속도
                break;
        }
    }

    void Batch()
    {
        for(int index = 0; index < count; index++)
        {
            //무기를 받을 지역변수
            Transform bullet;
            
            //자식을 가지고 있다면(이미 무기가 있다면)
            if(index < transform.childCount)
            {
                //기존에 있는 무기를 먼저 사용함
                bullet = transform.GetChild(index);
            }
            else
            {
                //모자란 무기는 풀링 호출해서 지역 변수로 받아서 
                bullet = GameManager.instance.pool.Get(prefabId).transform;
                //자식으로 추가
                bullet.parent = transform;
            }

            //위치 초기화
            bullet.localPosition = Vector3.zero; //플레이어의 위치
            bullet.localRotation = Quaternion.identity;

            //무기 회전 값
            Vector3 rotVec = Vector3.forward * 360 * index / count;
            //무기 회전
            bullet.Rotate(rotVec);
            //무기를 월드 좌표 기준으로 이동(위로)
            bullet.Translate(bullet.up * 1.5f, Space.World);
            //무기 초기화
            bullet.GetComponent<Bullet>().Init(damage,-1, Vector3.zero);  //근접무기이므로 관통력을 -1로 처리
        }
    }

    void Fire()
    {
        //주변에 타겟이 없다면 실행X
        if (!player.scanner.nearestTarget)
        {
            return;
        }

        //타겟 위치
        Vector3 targetPos = player.scanner.nearestTarget.position;
        //타겟 방향
        Vector3 dir = targetPos - transform.position;
        //정규화
        dir = dir.normalized;

        //풀링 호출해서 프리팹 추가
        Transform bullet = GameManager.instance.pool.Get(prefabId).transform;
        //총알 소환
        bullet.position = transform.position;
        //총알 회전(기준 축, 회전 방향)
        bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);
        //데미지와 관통력, 방향값을 Bullet.cs의 Init 함수에 전달
        bullet.GetComponent<Bullet>().Init(damage, count, dir);
    }
}
