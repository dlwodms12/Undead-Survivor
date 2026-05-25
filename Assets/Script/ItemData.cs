using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptble Object/ItemData")] 
public class ItemData : ScriptableObject
{
    public enum ItemType
    {
        Melee, //근접
        Range, //원거리
        Glove, //장갑
        Shoe,  //신발
        Heal   //치료
    }

    [Header("# Main Info")]
    public ItemType itemType;
    public int itemId;
    public string itemName;
    public string itemDesc;
    public Sprite itemIcon;

    [Header("# Level Data")]
    public float baseDamage; //기본 데미지
    public int baseCount;  //근접은 주변을 도는 오브젝트 수, 원거리는 관통력
    public float[] damages; //레벨별 데미지를 저장할 배열
    public int[] counts;  //레벨별 카운트를 저장할 배열

    [Header("# Weapon")]
    public GameObject projectile;

}
