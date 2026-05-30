using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AchiveManager : MonoBehaviour
{
    public GameObject[] lockCharacter;
    public GameObject[] unlockCharacter;
    //업적해제 알림
    public GameObject uiNotice;

    //업적 종류
    enum Achive { UnlockPotato, UnlockBean }

    //업적 알림에 쓰이는 코루틴 변수
    WaitForSecondsRealtime wait;

    //초기화
    private void Awake()
    {
        //코루틴 변수 초기화
        wait = new WaitForSecondsRealtime(5);

        //PlayerPrefs에 MyData 값이 0이라면(게임을 처음 실행했다면)
        if (!PlayerPrefs.HasKey("MyData"))
        {
            Init();
        }
    }

    //데이터 초기화
    void Init()
    {
        PlayerPrefs.SetInt("MyData", 1);
        //Enum.GetValues : 열거형의 모든 데이터를 가져오는 함수
        foreach (Achive achive in (Achive[])Enum.GetValues(typeof(Achive)))
        {
            PlayerPrefs.SetInt(achive.ToString(), 0);
        }
    }

    void Start()
    {
        UnlockCharacter();
        //GameManager 이벤트 구독
        GameManager.OnKillCountingChanged += CheckKillAchive;
        GameManager.OnGameVictory += CheckStatusAchive; // 시간 만료 승리 시 실행
    }

    private void OnDestroy()
    {
        //이벤트 구독 해제
        GameManager.OnKillCountingChanged -= CheckKillAchive;
        GameManager.OnGameVictory -= CheckStatusAchive;
    }

    //몬스터가 죽을 때만 실행
    void CheckKillAchive(int currentKill)
    {
        if (currentKill >= 10 && PlayerPrefs.GetInt(Achive.UnlockPotato.ToString()) == 0)
        {
            UnlockAchive(Achive.UnlockPotato);
        }
    }

    //게임에서 승리했을 때만 실행
    void CheckStatusAchive()
    {
        if (PlayerPrefs.GetInt(Achive.UnlockBean.ToString()) == 0)
        {
            UnlockAchive(Achive.UnlockBean);
        }
    }

    //업적 검사 후 UI 출력
    void UnlockAchive(Achive achive)
    {
        PlayerPrefs.SetInt(achive.ToString(), 1);

        // 알림 UI 순번에 맞게 켜주기
        for (int index = 0; index < uiNotice.transform.childCount; index++)
        {
            bool isActive = index == (int)achive;
            uiNotice.transform.GetChild(index).gameObject.SetActive(isActive);
        }

        StartCoroutine(NoticeRoutine());
    }

    //캐릭터 잠금해제
    void UnlockCharacter()
    {
        //잠겨진 캐릭터들 순회
        for (int index = 0; index < lockCharacter.Length; index++)
        {
            //업적 이름 받아오기
            string achiveName = ((Achive)index).ToString();
            //플레이어 데이터에서 해당 업적이 1(해금됐는지) 확인하는 변수
            bool isUnlock = PlayerPrefs.GetInt(achiveName) == 1;
            //잠겨진 캐릭터 목록에서 비활성화(캐릭터 해금)
            lockCharacter[index].SetActive(!isUnlock);
            //해금된 캐릭터 목록에서 활성화(캐릭터 해금)
            unlockCharacter[index].SetActive(isUnlock);
        }
    }

    IEnumerator NoticeRoutine()
    {
        uiNotice.SetActive(true);

        //오디오 재생 시작
        AudioManager.instance.PlaySfx(AudioManager.Sfx.LevelUp);

        yield return wait;

        uiNotice.SetActive(false);
    }
}
