using System.IO;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    //싱글톤 구조로 제작하여 필요한 곳에서 호출하여 저장
    public static DataManager instance;

    // 실제 게임에서 참조하고 수정할 데이터 객체
    public GameData gameData = new GameData();

    private string saveFilePath;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 씬이 바뀌어도 파괴되지 않음
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Application.persistentDataPath : OS 환경(PC, 모바일 등)에 맞춰 안전한 저장 경로를 자동으로 찾아줌
        saveFilePath = Path.Combine(Application.persistentDataPath, "SaveData.json");

        // 게임이 켜지자마자 기존 데이터를 로드
        LoadGame();
    }

    // C# 데이터를 JSON 텍스트 파일로 저장
    public void SaveGame()
    {
        // JsonUtility.ToJson : 두 번째 인자를 true로 주면 메모장으로 열었을 때 줄바꿈 처리
        string jsonText = JsonUtility.ToJson(gameData, true);

        // 경로에 실제 파일 작성
        File.WriteAllText(saveFilePath, jsonText);

        //Debug Code
        Debug.Log($"[Save 완료] 경로: {saveFilePath}");
    }

    // JSON 텍스트 파일을 C# 데이터로 로드
    public void LoadGame()
    {
        // 기존에 저장된 파일이 있다면 읽어옴
        if (File.Exists(saveFilePath))
        {
            string jsonText = File.ReadAllText(saveFilePath);
            gameData = JsonUtility.FromJson<GameData>(jsonText);

            //Debug Code
            Debug.Log("[Load 완료] 기존 데이터를 불러왔습니다.");
        }
        // 저장된 파일이 없다면
        else
        {
            gameData = new GameData(); // 새 데이터 생성
            SaveGame();                // 초기 파일 생성

            //Debug Code
            Debug.Log("[Load 실패] 저장 파일이 없어 초기 데이터를 생성했습니다.");
        }
    }
}
