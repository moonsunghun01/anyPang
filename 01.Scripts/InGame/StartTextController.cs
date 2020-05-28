using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using DG.Tweening;

public class StartTextController : MonoBehaviour
{
    public Text StartText;
    public StartController startController; 
    GameManager gameManager;

    bool isStartText;
    float StartTextCount;
    // Start is called before the first frame update
    void Start()
    { 
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        isStartText = false;
        StartTextCount = 1;
    }

    // Update is called once per frame
    void Update()
    {  
        if (startController.isTextStart)
        {
            StartText.gameObject.SetActive(true);
            StartText.transform.DOPunchScale(new Vector3(0.01f, 0.01f, 0.01f), 1);
            startController.isTextStart = false;
            isStartText = true;
        }
        if (isStartText)
        {
            StartTextCount -= Time.deltaTime;
            if (StartTextCount < 0)
            {
                gameManager.isStart = true;
                StartText.gameObject.SetActive(false);
            }
        }
        
    }
}
