using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AchiveManager : MonoBehaviour
{
    public GameObject[] lockCharacter;
    public GameObject[] unlockCharacter;

    //업적 종류
    enum Achive { UnlockPotato, UnlockBean }

    //업적 데이터를 저장
    Achive[] achives;

    //초기화
    private void Awake()
    {
        //Enum.GetValues : 열거형의 모든 데이터를 가져오는 함수
        achives = (Achive[])Enum.GetValues(typeof(Achive));

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

        foreach (Achive achive in achives)
        {
            PlayerPrefs.SetInt(achive.ToString(), 0);
        }
    }

    void Start()
    {
        UnlockCharacter();
    }

    //캐릭터 잠금해제
    void UnlockCharacter()
    {
        //잠겨진 캐릭터들 순회
        for (int index = 0; index < lockCharacter.Length; index++)
        {
            //업적 이름 받아오기
            string achiveName = achives[index].ToString();
            //플레이어 데이터에서 해당 업적이 1(해금됐는지) 확인하는 변수
            bool isUnlock = PlayerPrefs.GetInt(achiveName) == 1;
            //잠겨진 캐릭터의 업적이 해금됐다면 잠겨진 캐릭터 활성화
            lockCharacter[index].SetActive(!isUnlock);
            //업적이 잠겨있다면 해제된 캐릭터를 다시 잠금
            unlockCharacter[index].SetActive(isUnlock);
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        foreach(Achive achive in achives)
        {
            CheckAchive(achive);
        }
    }

    void CheckAchive(Achive achive)
    {
        bool isAchive = false;

        //각 조건에 따른 업적 해금 여부 판단
        switch (achive)
        {
            case Achive.UnlockPotato:
                isAchive = GameManager.instance.kill >= 10;
                break;

            case Achive.UnlockBean:
                isAchive = GameManager.instance.gameTime == GameManager.instance.maxGameTime;
                break;
        }

        //조건을 충족했고 && 플레이어 데이터에서 아직 업적이 깨지지 않은 상태라면
        if(isAchive && PlayerPrefs.GetInt(achive.ToString()) == 0)
        {
            //업적 해금
            PlayerPrefs.SetInt(achive.ToString(), 1);
        }
    }
}
