using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyScoreController : MonoBehaviour
{
    public SpriteRenderer MyScoreImg;   // 현재 블럭의 이미지  

    public Sprite[] CountType;
    public int numberCount =1;
    GameManager gameManager;
    void Start()
    {
        MyScoreImg = GetComponent<SpriteRenderer>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
     
    void Update()
    {
        if(numberCount == 6) MyScoreImg.sprite = CountType[gameManager.MyScore / 100000 % 10];
        if(numberCount == 5) MyScoreImg.sprite = CountType[gameManager.MyScore / 10000  % 10];
        if(numberCount == 4) MyScoreImg.sprite = CountType[gameManager.MyScore / 1000   % 10];
        if(numberCount == 3) MyScoreImg.sprite = CountType[gameManager.MyScore / 100    % 10];
        if(numberCount == 2) MyScoreImg.sprite = CountType[gameManager.MyScore / 10     % 10];
        if(numberCount == 1) MyScoreImg.sprite = CountType[gameManager.MyScore % 10]; 
    }
    
}
