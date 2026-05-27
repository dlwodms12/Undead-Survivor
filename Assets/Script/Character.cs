using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    //이동속도+10%
    public static float Speed 
    {
        //id가 0인 경우 1.1f 반환, 그렇지 않은 경우 1f 반환
        get { return GameManager.instance.playerId == 0 ? 1.1f: 1f; }
    }

    //무기 회전 속도(근접) +10%
    public static float WeaponSpeed
    {
        //id가 1인 경우 1.1f 반환, 그렇지 않은 경우 1f 반환
        get { return GameManager.instance.playerId == 1 ? 1.1f : 1f; }
    }

    //무기 연사 속도(원거리) +10%
    public static float WeaponRate
    {
        //id가 1인 경우 0.9f 반환(연사는 속도가 낮아야 더 빨리 쏜다), 그렇지 않은 경우 1f 반환
        get { return GameManager.instance.playerId == 1 ? 0.9f : 1f; }
    }

    //무기 데미지 +20%
    public static float Damage
    {
        //id가 2인 경우 1.2f 반환, 그렇지 않은 경우 1f 반환
        get { return GameManager.instance.playerId == 2 ? 1.2f : 1f; }
    }

    //무기 개수(근접)/관통력(원거리) +1
    public static int Count
    {
        //id가 3인 경우 1 반환, 그렇지 않은 경우 1f 반환
        get { return GameManager.instance.playerId == 3 ? 1 : 0; }
    }
}
