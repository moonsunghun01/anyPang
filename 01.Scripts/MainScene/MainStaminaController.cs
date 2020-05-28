using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainStaminaController : MonoBehaviour
{
    public SpriteRenderer StaminaImg;   // 현재 블럭의 이미지 

    public Sprite[] StaminaType; 
    MainSceneManager mainSceneManager;

    public Text StaminCountText;

    public float StaminaUpCount;
    // Start is called before the first frame update
    void Start()
    {
        StaminaImg = GetComponent<SpriteRenderer>();
        mainSceneManager = GameObject.Find("MainSceneManager").GetComponent<MainSceneManager>();
        //StaminaUpCount = 60;
        StaminaUpCount = mainSceneManager.staminaCountNum;
    }

    // Update is called once per frame
    void Update()
    {
        StaminaImg.sprite = StaminaType[mainSceneManager.MyStamina];
        
        if (mainSceneManager.MyStamina == 5) StaminCountText.text = "  Max";
        else
        {
            if(StaminaUpCount <10) StaminCountText.text = "00:0" + ((int)StaminaUpCount).ToString();
            else StaminCountText.text = "00:" + ((int)StaminaUpCount).ToString();
        }
        if(mainSceneManager.MyStamina != 5)
        {
            StaminaUpCount -= Time.deltaTime;
            if (StaminaUpCount < 0)
            {
                mainSceneManager.MyStamina++;
                StaminaUpCount = 59;
                mainSceneManager.SaveJsonData(); 
            }
            mainSceneManager.MyStaminaTime = (int)StaminaUpCount;
        } 
    }
}
