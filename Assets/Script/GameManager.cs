using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("# Game Control")]
    public float gameTime;
    public float maxGameTime = 2 * 10f;
    public bool isLive;

    [Header("# Player Info")]
    public int playerId; //캐릭터 선택할 때 사용할 ID
    public float health;
    public float maxHealth = 100;
    public int level;
    public int kill;
    public int exp;
    //레벨업 하는데 필요한 경험치
    public int[] nextExp = { 3, 5, 10, 100, 150, 210, 280, 360, 450, 600 };

    [Header("# Game Object")]
    public PoolManager pool;
    public Player player;
    public LevelUp uiLevelUp;
    public Result uiResult; //게임 결과창 컨트롤
    public GameObject enemyCleaner;

    private void Awake()
    {
        instance = this;
    }

    //Start버튼에서 컨트롤 할 수 있도록 public 으로 선언
    public void GameStart(int id)
    {
        //캐릭터 선택
        playerId = id;

        //체력 초기화
        health = maxHealth;

        player.gameObject.SetActive(true);

        //기본 무기 지급
        //무기 종류가 두가지 뿐이므로 %2 연산을 통해 무기 범위를 0과 1로 제한
        uiLevelUp.Select(playerId % 2);

        Resume();
        
    }

    public void GameRetry()
    {
        //씬 다시 불러오기
        SceneManager.LoadScene("Main");

        
    }

    public void GameOver()
    {
        StartCoroutine(GameOverRoutine());
    }

    //게임 오버처리하기까지의 시간을 벌기 위한 코루틴
    IEnumerator GameOverRoutine()
    {
        isLive = false;

        yield return new WaitForSeconds(0.5f);

        uiResult.gameObject.SetActive(true);
        uiResult.Lose();

        Stop();
    }

    public void GameVictory()
    {
        StartCoroutine(GameVictoryRoutine());
    }

    //게임 오버처리하기까지의 시간을 벌기 위한 코루틴
    IEnumerator GameVictoryRoutine()
    {
        isLive = false;

        enemyCleaner.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        uiResult.gameObject.SetActive(true);
        uiResult.Win();

        Stop();
    }

    void Update()
    {
        //게임이 정지한 상태라면 업데이트 중단
        if (!isLive) { return; }

        gameTime += Time.deltaTime;

        if (gameTime > maxGameTime)
        {
            gameTime = maxGameTime;
            GameVictory();
        }
    }

    public void GetExp()
    {
        //게임이 정지한 상태라면 업데이트 중단
        if (!isLive) { return; }

        exp++;

        //최대레벨(10)을 초과할 경우 nextExp[9]를 유지
        if(exp >= nextExp[Mathf.Min(level, nextExp.Length-1)])
        {
            level++;
            exp = 0;
            uiLevelUp.Show();
        }
    }

    public void Stop()
    {
        //게임 정지
        isLive = false; 
        Time.timeScale = 0;
    }

    public void Resume()
    {
        //게임 재생
        isLive = true;
        Time.timeScale = 1;
    }
}
