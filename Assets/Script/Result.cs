using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Result : MonoBehaviour
{
    public GameObject[] titles;

    //Title Over 창 활성화
    public void Lose()
    {
        titles[0].SetActive(true);
    }

    //Title Victory 활성화
    public void Win()
    {
        titles[1].SetActive(true);
    }
}
