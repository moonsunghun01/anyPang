using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainSettingController : MonoBehaviour
{
    public GameObject audioOnButton;
    public GameObject audioOffButton;
    public GameObject pushOnButton;
    public GameObject pushOffButton; 
    public GameObject infoOnButton;
    public GameObject infoOffButton;

    MainSceneManager mainSceneManager;
    void Start()
    { 
        mainSceneManager = GameObject.Find("MainSceneManager").GetComponent<MainSceneManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (mainSceneManager.audioOnOff)
        {
            audioOnButton.SetActive(true);
            audioOffButton.SetActive(false);
        }
        else if (!mainSceneManager.audioOnOff)
        {
            audioOnButton.SetActive(false);
            audioOffButton.SetActive(true);
        }

        if (mainSceneManager.pushOnOff)
        {
            pushOnButton.SetActive(true);
            pushOffButton.SetActive(false);
        }
        else if (!mainSceneManager.pushOnOff)
        {
            pushOnButton.SetActive(false);
            pushOffButton.SetActive(true);
        }

        if (mainSceneManager.infoOnOff)
        {
            infoOnButton.SetActive(true);
            infoOffButton.SetActive(false);
        }
        else if (!mainSceneManager.infoOnOff)
        {
            infoOnButton.SetActive(false);
            infoOffButton.SetActive(true);
        }
    }
}
