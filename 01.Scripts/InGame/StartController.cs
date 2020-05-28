using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class StartController : MonoBehaviour
{
    public SpriteRenderer StartCountImg;   // 현재 블럭의 이미지  
    public Sprite[] StartCountType; 
    GameManager gameManager;

    public float StartCount ;
    public bool isTextStart;
    // Start is called before the first frame update
    void Start()
    {
        StartCount = 4;
        StartCountImg = GetComponent<SpriteRenderer>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    { 
        if (!gameManager.isStart)
        {
            StartCount -= Time.deltaTime;
            if (StartCount > 1)
            {
                StartCountImg.sprite = StartCountType[(int)StartCount]; 
            } 
            else 
            {
                isTextStart = true;
                StartCountImg.gameObject.SetActive(false); 
            } 
        } 
    }
}
