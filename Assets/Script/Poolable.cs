using UnityEngine;

public class Poolable : MonoBehaviour
{
    private int prefabId;
    private bool isInitialized = false;

    // 풀매니저가 생성될 때 ID를 부여
    public void Setup(int id)
    {
        prefabId = id;
        isInitialized = true;
    }

    // 오브젝트가 대미지를 받거나 화면 밖으로 나가서 SetActive(false)가 되는 순간 호출
    private void OnDisable()
    {
        // 게임이 종료되거나 씬이 바뀔 때 풀매니저가 먼저 파괴되어 발생하는 에러를 방지
        if (!isInitialized || PoolManager.instance == null) return;

        // 비활성화되는 순간, 자동으로 풀매니저의 해당 큐로 복귀
        PoolManager.instance.ReturnToPool(prefabId, gameObject);
    }
}
