using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieMaxScoreController : MonoBehaviour
{
    public SpriteRenderer DieMaxScoreImg;   // 현재 블럭의 이미지  

    public Sprite[] CountType;
    public int numberCount = 1;
    DieSceneManager dieSceneManager;
    void Start()
    {
        DieMaxScoreImg = GetComponent<SpriteRenderer>();
        dieSceneManager = GameObject.Find("DieSceneManager").GetComponent<DieSceneManager>();
    }

    void Update()
    {
        if (numberCount == 6) DieMaxScoreImg.sprite = CountType[dieSceneManager.data.MaxScore / 100000 % 10];
        if (numberCount == 5) DieMaxScoreImg.sprite = CountType[dieSceneManager.data.MaxScore / 10000 % 10];
        if (numberCount == 4) DieMaxScoreImg.sprite = CountType[dieSceneManager.data.MaxScore / 1000 % 10];
        if (numberCount == 3) DieMaxScoreImg.sprite = CountType[dieSceneManager.data.MaxScore / 100 % 10];
        if (numberCount == 2) DieMaxScoreImg.sprite = CountType[dieSceneManager.data.MaxScore / 10 % 10];
        if (numberCount == 1) DieMaxScoreImg.sprite = CountType[dieSceneManager.data.MaxScore % 10];
    }
}
