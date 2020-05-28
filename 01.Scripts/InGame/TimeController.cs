using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeController : MonoBehaviour
{
    public SpriteRenderer TimeImg;   // 현재 블럭의 이미지  

    public Sprite[] CountType;
    public int numberCount = 1;
    GameManager gameManager;
     
    void Start()
    {
        TimeImg = GetComponent<SpriteRenderer>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    void Update()
    { 
        if (numberCount == 2) TimeImg.sprite = CountType[(int)gameManager.MyTime / 10 % 10]; 
        //Debug.Log(TempNum);
        if (numberCount == 3) TimeImg.sprite = CountType[(int)gameManager.MyTime % 10];
         
    }
}
