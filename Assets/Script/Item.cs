using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public ItemData data;
    public int level;
    public Weapon weapon;

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

                break;

            case ItemData.ItemType.Glove:
                break;
            case ItemData.ItemType.Shoe:
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
