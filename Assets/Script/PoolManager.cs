using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    //싱글톤 인스턴스
    public static PoolManager instance;

    //프리팹을 보관할 배열
    [Header("# Prefabs to Pool")]
    public GameObject[] prefabs;

    //큐를 선언
    private Queue<GameObject>[] pools;

    private void Awake()
    {
        instance = this;

        // 프리팹 종류 수만큼의 길이를 가지는 큐 배열 생성
        pools = new Queue<GameObject>[prefabs.Length];

        for (int index = 0; index < pools.Length; index++)
        {
            pools[index] = new Queue<GameObject>();
        }
    }

    public GameObject Get(int index)
    {
        GameObject select = null;

        // 큐에 대기 중인(비활성화된) 오브젝트가 있다면 루프 없이 즉시 꺼냄
        if (pools[index].Count > 0)
        {
            select = pools[index].Dequeue();
            select.SetActive(true);
        }
        // 대기 중인 오브젝트가 없다면 새로 생성(Instantiate)
        else
        {
            select = Instantiate(prefabs[index], transform);

            // 새로 만든 오브젝트에 자동으로 Poolable 컴포넌트를 붙여 관리를 자동화
            Poolable poolable = select.GetComponent<Poolable>();
            if (poolable == null)
            {
                poolable = select.AddComponent<Poolable>();
            }
            poolable.Setup(index); // 본인의 프리팹 인덱스 등록

            select.SetActive(true);
        }

        return select;
    }

    // Poolable 컴포넌트가 OnDisable 될 때 호출할 복귀 전용 함수
    public void ReturnToPool(int index, GameObject obj)
    {
        pools[index].Enqueue(obj);
    }

}
