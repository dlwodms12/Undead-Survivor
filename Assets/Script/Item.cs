using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public ItemData data;
    public int level;
    public Weapon weapon;
    public Gear gear;

    Image icon;
    Text textLevel;

    private void Awake()
    {
        //배열의 두번째 컴포넌트를 가져오기(첫번째는 자기 자신)
        //Item 스크립트는 자식으로 TextLevel과 Icon을 가지고 있음
        icon = GetComponentsInChildren<Image>()[1];
        //호출된 아이템에 맞는 아이콘 가져오기(ItemData)
        icon.sprite = data.itemIcon;

        Text[] texts = GetComponentsInChildren<Text>();
        textLevel = texts[0];
    }

    private void LateUpdate()
    {
        textLevel.text = "Lv." + (level + 1);
    }

    public void OnClick()
    {
        switch (data.itemType)
        {
            case ItemData.ItemType.Melee:
            case ItemData.ItemType.Range:
                //아이템 레벨이 0일 경우 새로운 무기 오브젝트 생성
                if(level == 0)
                {
                    //타입에 맞는 무기로 초기화
                    GameObject newWeapon = new GameObject();
                    weapon = newWeapon.AddComponent<Weapon>();
                    weapon.Init(data);
                }
                else
                {
                    //이미 기존 무기가 있을 경우
                    float nextDamage = data.baseDamage;
                    int nextCount = 0;

                    //레벨에 비례한 가중치(ItemData)에 맞게 데미지 상승
                    nextDamage += data.baseDamage * data.damages[level];
                    nextCount += data.counts[level];

                    //Weapon.cs의 LevelUp 함수 호출
                    weapon.LevelUp(nextDamage, nextCount);
                }
                    break;

            case ItemData.ItemType.Glove:
            case ItemData.ItemType.Shoe:
                if(level == 0)
                {
                    GameObject newGear = new GameObject();
                    gear = newGear.AddComponent<Gear>();
                    gear.Init(data);
                }
                else
                {
                    float nextRate = data.damages[level];
                    gear.LevelUp(nextRate);
                }
                    break;
            case ItemData.ItemType.Heal:
                break;
        }

        level++;

        //최대 레벨에 도달한 경우
        if(level == data.damages.Length)
        {
            GetComponent<Button>().interactable = false;
        }

    }
}
