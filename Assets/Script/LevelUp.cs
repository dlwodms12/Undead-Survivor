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
    }

    public void Hide()
    {
        rect.localScale = Vector3.zero;
        GameManager.instance.Resume();
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

        // 그 중에서 랜덤하게 3개 아이템 활성화
        int[] ran = new int[3];

        while (true)
        {
            //3개의 변수에 랜덤 숫자 저장
            ran[0] = Random.Range(0, items.Length);
            ran[1] = Random.Range(0, items.Length);
            ran[2] = Random.Range(0, items.Length);

            //3개의 숫자가 중복되지 않는다면
            if (ran[0] != ran[1] && ran[1] != ran[2] && ran[0] != ran[2])
            {
                break;
            }
        }

        for(int index = 0;  index < ran.Length; index++)
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
