using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainScoreController : MonoBehaviour
{
    public SpriteRenderer ScoreImg;   // 현재 블럭의 이미지 

    public Sprite[] ScoreType;
    public int ScoreNum = 1;
    MainSceneManager mainSceneManager;

    // Start is called before the first frame update
    void Start()
    {
        ScoreImg = GetComponent<SpriteRenderer>();
        mainSceneManager = GameObject.Find("MainSceneManager").GetComponent<MainSceneManager>();
    }

    // Update is called once per frame
    void Update()
    {  
        if (ScoreNum == 6) ScoreImg.sprite =ScoreType[mainSceneManager.MaxScore / 100000 % 10];
        if (ScoreNum == 5) ScoreImg.sprite =ScoreType[mainSceneManager.MaxScore / 10000 % 10];
        if (ScoreNum == 4) ScoreImg.sprite =ScoreType[mainSceneManager.MaxScore / 1000 % 10];
        if (ScoreNum == 3) ScoreImg.sprite =ScoreType[mainSceneManager.MaxScore / 100 % 10];
        if (ScoreNum == 2) ScoreImg.sprite =ScoreType[mainSceneManager.MaxScore / 10 % 10];
        if (ScoreNum == 1) ScoreImg.sprite = ScoreType[mainSceneManager.MaxScore % 10];
    }
}
