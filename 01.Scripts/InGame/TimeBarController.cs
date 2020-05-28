using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeBarController : MonoBehaviour
{
    public Slider TimeBar; 
    GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        TimeBar.value = Mathf.Lerp(TimeBar.value, gameManager.MyTime / 60, Time.deltaTime * 10);
    }
}
