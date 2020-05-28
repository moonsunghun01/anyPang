using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class DieGoldController : MonoBehaviour 
{
    public SpriteRenderer GoldImg;   // 현재 블럭의 이미지  

    public Sprite[] CountType;
    public int numberCount = 1;
    DieSceneManager dieSceneManager;
    void Start()
    {
        GoldImg = GetComponent<SpriteRenderer>();
        dieSceneManager = GameObject.Find("DieSceneManager").GetComponent<DieSceneManager>();
    }

    void Update()
    { 
        if (numberCount == 5) GoldImg.sprite = CountType[dieSceneManager.GainGold / 10000 % 10];
        if (numberCount == 4) GoldImg.sprite = CountType[dieSceneManager.GainGold / 1000 % 10];
        if (numberCount == 3) GoldImg.sprite = CountType[dieSceneManager.GainGold / 100 % 10];
        if (numberCount == 2) GoldImg.sprite = CountType[dieSceneManager.GainGold / 10 % 10];
        if (numberCount == 1) GoldImg.sprite = CountType[dieSceneManager.GainGold % 10];
    }
}
