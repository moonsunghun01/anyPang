using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCountController : MonoBehaviour
{
    public SpriteRenderer MoveCountImg;   // 현재 블럭의 이미지  

    public Sprite[] CountType;
    public int CountIndex = 1;
    GameManager gameManager;
    void Start()
    {
        MoveCountImg = GetComponent<SpriteRenderer>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    void Update()
    {
        if (CountIndex == 2) MoveCountImg.sprite = CountType[gameManager.MoveCount / 10];
        if (CountIndex == 1) MoveCountImg.sprite = CountType[gameManager.MoveCount % 10]; 
    }

}
