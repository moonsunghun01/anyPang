using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalScoreController : MonoBehaviour
{
    public SpriteRenderer GoalScoreImg;   // 현재 블럭의 이미지  

    public Sprite[] CountType;
    public int numberCount = 1;
    GameManager gameManager;

    void Start()
    {
        GoalScoreImg = GetComponent<SpriteRenderer>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    void Update()
    {
        if (numberCount == 6) GoalScoreImg.sprite = CountType[gameManager.MaxScore / 100000 % 10];
        if (numberCount == 5) GoalScoreImg.sprite = CountType[gameManager.MaxScore / 10000  % 10];
        if (numberCount == 4) GoalScoreImg.sprite = CountType[gameManager.MaxScore / 1000   % 10];
        if (numberCount == 3) GoalScoreImg.sprite = CountType[gameManager.MaxScore / 100    % 10];
        if (numberCount == 2) GoalScoreImg.sprite = CountType[gameManager.MaxScore / 10     % 10];
        if (numberCount == 1) GoalScoreImg.sprite = CountType[gameManager.MaxScore % 10]; 
    }
}
