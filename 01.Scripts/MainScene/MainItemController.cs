using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainItemController : MonoBehaviour
{
    public SpriteRenderer ItemNumImg;   // 현재 블럭의 이미지 

    public Sprite[] ItemNumType;
    public int ItemNum = 1;
    MainSceneManager mainSceneManager;

    // Start is called before the first frame update
    void Start()
    {
        ItemNumImg = GetComponent<SpriteRenderer>();
        mainSceneManager = GameObject.Find("MainSceneManager").GetComponent<MainSceneManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (ItemNum == 1) ItemNumImg.sprite = ItemNumType[mainSceneManager.MyFirstItemNum];
        if (ItemNum == 2) ItemNumImg.sprite = ItemNumType[mainSceneManager.MySecondItemNum];
    }
}
