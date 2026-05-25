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
    Text textName;
    Text textDesc;

    private void Awake()
    {
        //배열의 두번째 컴포넌트를 가져오기(첫번째는 자기 자신)
        //Item 스크립트는 자식으로 TextLevel과 Icon을 가지고 있음
        icon = GetComponentsInChildren<Image>()[1];
        //호출된 아이템에 맞는 아이콘 가져오기(ItemData)
        icon.sprite = data.itemIcon;

        Text[] texts = GetComponentsInChildren<Text>();
        //계층구조 순서에 따라 텍스트 컴포넌트를 가져와 할당
        textLevel = texts[0];
        textName = texts[1];
        textDesc = texts[2];

        //아이템 이름 출력
        textName.text = data.itemName;
    }

    private void OnEnable()
    {
        textLevel.text = "Lv." + (level + 1);

        switch (data.itemType)
        {
            //무기의 경우 데미지 증가 수치와 회전체/관통력 증가 수치를 알려줘야 함
            case ItemData.ItemType.Melee:
            case ItemData.ItemType.Range:
                textDesc.text = string.Format(data.itemDesc, data.damages[level] * 100, data.counts[level]);
                break;
            //장갑과 신발의 경우 연사/이동 속도 증가 수치를 알려줘야 함
            case ItemData.ItemType.Glove:
            case ItemData.ItemType.Shoe:
                textDesc.text = string.Format(data.itemDesc, data.damages[level] * 100);
                break;
            //음료의 경우 아이템 설명만 있으면 됨
            default:
                textDesc.text = string.Format(data.itemDesc);
                break;
        }

        
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

                level++;
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
                level++;
                break;
            case ItemData.ItemType.Heal:
                GameManager.instance.health = GameManager.instance.maxHealth;
                break;
        }

        //최대 레벨에 도달한 경우
        if(level == data.damages.Length)
        {
            GetComponent<Button>().interactable = false;
        }

    }
}
