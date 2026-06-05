using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUp : MonoBehaviour
{
    RectTransform rect;
    Item[] items;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        items = GetComponentsInChildren<Item>(true);
    }

    public void Show()
    {
        Next();
        rect.localScale = Vector3.one;
        GameManager.instance.Stop();

        //오디오 재생 시작
        AudioManager.instance.PlaySfx(AudioManager.Sfx.LevelUp);
        AudioManager.instance.EffectBgm(true);
    }

    public void Hide()
    {
        rect.localScale = Vector3.zero;
        GameManager.instance.Resume();

        //오디오 재생 시작
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
        AudioManager.instance.EffectBgm(false);
    }
    //기본 무기 장착을 위한 선택 함수
    public void Select(int index)
    {
        items[index].OnClick();
    }

    //아이템 랜덤 활성화 함수
    void Next()
    {
        // 모든 아이템 비활성화
        foreach (Item item in items)
        {
            item.gameObject.SetActive(false);
        }

        //모든 아이템의 인덱스를 담을 임시 리스트 생성
        List<int> shuffleList = new List<int>();
        for (int i = 0; i < items.Length; i++)
        {
            shuffleList.Add(i);
        }

        //리스트 섞기
        for (int i = shuffleList.Count - 1; i > 0; i--)
        {
            // 0부터 i까지의 범위 중 랜덤 인덱스 선택
            int randomIndex = Random.Range(0, i + 1);

            // 두 요소의 위치를 Swap
            int temp = shuffleList[i];
            shuffleList[i] = shuffleList[randomIndex];
            shuffleList[randomIndex] = temp;
        }

        // 리스트의 맨 앞 3개를 순서대로 가져옴
        int[] ran = new int[3];
        for (int i = 0; i < ran.Length; i++)
        {
            ran[i] = shuffleList[i];
        }

        for (int index = 0;  index < ran.Length; index++)
        {
            //뽑힌 랜덤 숫자에 해당하는 아이템 배열 칸을 찾아감
            Item ranItem = items[ran[index]];

            // 만렙 아이템의 경우 (damages는 레벨별 데미지를 저장하는 배열이므로 전체 길이 == 만렙)
            if(ranItem.level == ranItem.data.damages.Length)
            {
                //소비 아이템으로 대체
                items[4].gameObject.SetActive(true);

                //소비 아이템이 여러개인 경우
                /*
                 items[Random.Range(4, 아이템 개수].gameObject.SetActive(true);
                 */
            }
            //만렙 아이템이 아닌 경우
            else
            {
                ranItem.gameObject.SetActive(true);
            }
        }
    }
}
