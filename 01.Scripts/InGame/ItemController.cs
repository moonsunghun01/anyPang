using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemController : MonoBehaviour
{
    public SpriteRenderer ItemNumImg;   // 현재 블럭의 이미지 

    public Sprite[] ItemNumType; 
    public int ItemNum = 1;
    GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        ItemNumImg = GetComponent<SpriteRenderer>(); 
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
       if(ItemNum ==1) ItemNumImg.sprite = ItemNumType[gameManager.MyFirstItemNum];  
       if(ItemNum ==2) ItemNumImg.sprite = ItemNumType[gameManager.MySecondItemNum];  
    }
     
}
