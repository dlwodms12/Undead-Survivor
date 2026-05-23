using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    //프리팹을 보관할 배열
    public GameObject[] prefabs;

    //풀 담당을 하는 리스트
    List<GameObject>[] pools;

    private void Awake()
    {
        //프리팹 종류 수만큼의 길이를 가지는 리스트를 생성
        pools = new List<GameObject>[prefabs.Length];

        //풀 리스트 초기화
        for(int index = 0; index < pools.Length; index++)
        {
            pools[index] = new List<GameObject>();
        }
    }

    public GameObject Get(int index)
    {
        GameObject select = null;

        //선택한 풀의 게임 오브젝트에 접근
        foreach(GameObject item in pools[index])
        {
            //오브젝트가 대기 중이라면
            if (!item.activeSelf)
            {
                //select 변수에 할당
                select = item;
                select.SetActive(true);
                break;
            }
        }

        //대기 중인 오브젝트가 없다면(item이 전부 활성화된 상태라면)
        if(!select)
        {
            //새로 생성해서 select 변수에 할당하고, 생성한 오브젝트는 PoolManager의 자식으로 둠
            select = Instantiate(prefabs[index], transform);
            //해당 오브젝트 풀 리스트에 Add 함수로 추가
            pools[index].Add(select);
        }
        return select;
    }
}
