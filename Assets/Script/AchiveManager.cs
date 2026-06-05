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
        wait = new WaitForSecondsRealtime(5);
    }

    //데이터 초기화
    void Init()
    {
        // 첫 플레이가 끝났음을 데이터에 기록
        DataManager.instance.gameData.isFirstPlay = false;

        // 초기화 시 모든 업적을 잠금(false) 상태로 세팅
        for (int i = 0; i < DataManager.instance.gameData.achievementUnlocked.Count; i++)
        {
            DataManager.instance.gameData.achievementUnlocked[i] = false;
        }

        DataManager.instance.SaveGame(); // 변경사항 저장
    }

    void Start()
    {
        // DataManager의 데이터를 검사
        if (DataManager.instance.gameData.isFirstPlay)
        {
            Init();
        }
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
        // 현재 업적이 false(미해금) 상태이고, 10킬 이상일 때만 실행
        bool isPotatoUnlocked = DataManager.instance.gameData.achievementUnlocked[(int)Achive.UnlockPotato];

        if (currentKill >= 10 && !isPotatoUnlocked)
        {
            UnlockAchive(Achive.UnlockPotato);
        }
    }

    //게임에서 승리했을 때만 실행
    void CheckStatusAchive()
    {
        // 현재 업적이 false(미해금) 상태면 실행
        bool isBeanUnlocked = DataManager.instance.gameData.achievementUnlocked[(int)Achive.UnlockBean];

        if (!isBeanUnlocked)
        {
            UnlockAchive(Achive.UnlockBean);
        }
    }

    //업적 검사 후 UI 출력
    void UnlockAchive(Achive achive)
    {
        // 해당 업적 인덱스를 true로 변경하고 저장
        int achiveIndex = (int)achive;
        DataManager.instance.gameData.achievementUnlocked[achiveIndex] = true;
        DataManager.instance.SaveGame(); // 디스크에 실시간 반영

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
            // 리스트에서 해금 여부 참조
            bool isUnlock = DataManager.instance.gameData.achievementUnlocked[index];

            //잠긴 캐릭터 항목에서 해당 캐릭터를 비활성화(잠금해제 됐으므로)
            lockCharacter[index].SetActive(!isUnlock);
            //잠금 해제된 캐릭터 항목에서 해당 캐릭터를 활성화
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
