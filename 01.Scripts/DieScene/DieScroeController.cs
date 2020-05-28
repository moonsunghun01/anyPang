using System.Collections;
using System.Collections.Generic;
using UnityEngine; 

public class DieScroeController : MonoBehaviour
{
    public SpriteRenderer DieScoreImg;   // 현재 블럭의 이미지  

    public Sprite[] CountType;
    public int numberCount = 1;
    DieSceneManager dieSceneManager;
    void Start()
    {
        DieScoreImg = GetComponent<SpriteRenderer>();
        dieSceneManager = GameObject.Find("DieSceneManager").GetComponent<DieSceneManager>();
    }

    void Update()
    {
        if (numberCount == 6) DieScoreImg.sprite = CountType[dieSceneManager.data.score / 100000 % 10];
        if (numberCount == 5) DieScoreImg.sprite = CountType[dieSceneManager.data.score / 10000 % 10];
        if (numberCount == 4) DieScoreImg.sprite = CountType[dieSceneManager.data.score / 1000 % 10];
        if (numberCount == 3) DieScoreImg.sprite = CountType[dieSceneManager.data.score / 100 % 10];
        if (numberCount == 2) DieScoreImg.sprite = CountType[dieSceneManager.data.score / 10 % 10];
        if (numberCount == 1) DieScoreImg.sprite = CountType[dieSceneManager.data.score % 10];
    }

}
