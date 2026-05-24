using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public int id; //무기 ID
    public int prefabId; //무기 프리팹 ID
    public float damage;  //무기 데미지
    public int count; //소환할 무기 개수
    public float speed; //무기 회전 속도

    private void Start()
    {
        Init();
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

                break;
        }
    }

    public void Init()
    {
        switch (id)
        {
            case 0:
                speed = -150;
                Batch();

                break;

            default:

                break;
        }
    }

    void Batch()
    {
        for(int index = 0; index < count; index++)
        {
            //지역 변수로 받아서 부모 변경(Weapon으로) 후 추가
            Transform bullet = GameManager.instance.pool.Get(prefabId).transform;
            bullet.parent = transform;
            
            //무기 회전 값
            Vector3 rotVec = Vector3.forward * 360 * index / count;
            //무기 회전
            bullet.Rotate(rotVec);
            //무기를 월드 좌표 기준으로 이동(위로)
            bullet.Translate(bullet.up * 1.5f, Space.World);
            //무기 초기화
            bullet.GetComponent<Bullet>().Init(damage,-1);  //근접무기이므로 관통력을 -1로 처리
        }
    }
}
