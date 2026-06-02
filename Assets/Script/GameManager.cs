using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    //킬 수가 변경되었을 때 외부로 쓸 이벤트
    public static event Action<int> OnKillCountingChanged;
    public static event Action OnGameVictory;

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
    //public PoolManager pool;
    public Player player;
    public LevelUp uiLevelUp;
    public Result uiResult; //게임 결과창 컨트롤
    public GameObject enemyCleaner;
    public Transform uiJoy;

    private void Awake()
    {
        instance = this;
        //60프레임 설정
        Application.targetFrameRate = 60;
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

        //오디오 재생 시작
        AudioManager.instance.PlayBgm(true);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
        
    }

    public void GameRetry()
    {
        //씬 다시 불러오기
        SceneManager.LoadScene("Main");
    }
    //게임 종료
    public void GameQuit()
    {
        Application.Quit();
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

        //오디오 재생 시작
        AudioManager.instance.PlayBgm(false);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Lose);
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

        //게임 승리 이벤트 발생
        OnGameVictory?.Invoke();

        yield return new WaitForSeconds(0.5f);

        uiResult.gameObject.SetActive(true);
        uiResult.Win();

        Stop();

        //오디오 재생 시작
        AudioManager.instance.PlayBgm(false);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Win);
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

    public void GetExp(int amount)
    {
        //게임이 정지한 상태라면 업데이트 중단
        if (!isLive) { return; }

        //코인이 주는 고유 경험치(amount)를 추가
        exp += amount;

        //최대레벨(10)을 초과할 경우 nextExp[9]를 유지
        //대량으로 코인을 습득해 연속으로 레벨업 하는 상황을 대비해 while로 검사
        while (exp >= nextExp[Mathf.Min(level, nextExp.Length - 1)])
        {
            exp -= nextExp[Mathf.Min(level, nextExp.Length - 1)];
            level++;
            uiLevelUp.Show();
        }
    }

    public void Stop()
    {
        //게임 정지
        isLive = false; 
        Time.timeScale = 0;
        uiJoy.localScale = Vector3.zero;
    }

    public void Resume()
    {
        //게임 재생
        isLive = true;
        Time.timeScale = 1;
        uiJoy.localScale = Vector3.one;
    }

    //킬 이벤트
    public void AddKill()
    {
        kill++;
        //이벤트 발생
        OnKillCountingChanged?.Invoke(kill);
    }
}
