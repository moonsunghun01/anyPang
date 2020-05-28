using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGoldController : MonoBehaviour
{
    public SpriteRenderer GoldImg;   // 현재 블럭의 이미지  

    public Sprite[] GoldType;
    public int GoldCount = 1;
    MainSceneManager mainSceneManager;

    void Start()
    {
        GoldImg = GetComponent<SpriteRenderer>();
        mainSceneManager = GameObject.Find("MainSceneManager").GetComponent<MainSceneManager>();
    }

    void Update()
    {
        if (GoldCount == 6) GoldImg.sprite = GoldType[mainSceneManager.MyGold / 100000 % 10];
        if (GoldCount == 5) GoldImg.sprite = GoldType[mainSceneManager.MyGold / 10000 % 10];
        if (GoldCount == 4) GoldImg.sprite = GoldType[mainSceneManager.MyGold / 1000 % 10];
        if (GoldCount == 3) GoldImg.sprite = GoldType[mainSceneManager.MyGold / 100 % 10];
        if (GoldCount == 2) GoldImg.sprite = GoldType[mainSceneManager.MyGold / 10 % 10];
        if (GoldCount == 1) GoldImg.sprite = GoldType[mainSceneManager.MyGold % 10];
    }
}
