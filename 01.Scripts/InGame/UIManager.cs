using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    public Image FadeImg;
    float time = 0;
    float F_time = 10; 
    GameManager gameManager;
    public Text TimeOverText;

    // Start is called before the first frame update
    bool isTimeOver;
    public GameObject ReStartObj;

    AudioSource EndAudioSource;
    public AudioClip EndAudioClip;

    public GameObject ActiveItem;
    public GameObject PauseUI;
    void Start()
    {
        DOTween.Init();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        EndAudioSource = GetComponent<AudioSource>(); 
    }

    // Update is called once per frame
    void Update()
    {
        if(gameManager.isDeath && !isTimeOver)
        { 
            if(!gameManager.isMute)EndAudioSource.PlayOneShot(EndAudioClip);
            isTimeOver = true;
            TimeOverText.gameObject.SetActive(true);
            TimeOverText.transform.DOPunchScale(new Vector3(0.01f, 0.01f, 0.01f), 1.5f);
        }
        if(gameManager.isDeath)
        { 
            StartCoroutine(FadeFlow()); 
        }

        if(gameManager.isRestart)
        { 
            ReStartObj.SetActive(true);
        }
        else if (!gameManager.isRestart)
        {
            ReStartObj.SetActive(false);
        }
        if (gameManager.isFirstItemActive || gameManager.isSecondItemActive) ActiveItem.SetActive(true);
        else ActiveItem.SetActive(false);
    } 

    public void ClickPauseButton()
    {
        if (gameManager.isStart)
        {
            PauseUI.SetActive(true);
            gameManager.isPause = true;
        }
    }

    public void ClickContinueButton()
    {
        PauseUI.SetActive(false);
        gameManager.isPause = false;
    }

    public void ClickGoMainButton()
    {
        PauseUI.SetActive(false);
        StartCoroutine(MainFadeOut());
    }

    IEnumerator FadeFlow()
    {
        yield return new WaitForSeconds(2);
        FadeImg.gameObject.SetActive(true);
        Color alpha = FadeImg.color;
        while (alpha.a < 1)
        {
            time += Time.deltaTime / F_time;
            alpha.a = Mathf.Lerp(0, 1, time);
            FadeImg.color = alpha;
            yield return null;
        }

        SceneManager.LoadScene("Die_Scene");
        yield return null;
    }

    IEnumerator MainFadeOut()
    {
        FadeImg.gameObject.SetActive(true);
        Color alpha = FadeImg.color;
        while (alpha.a < 1)
        {
            time += Time.deltaTime;
            alpha.a = Mathf.Lerp(0, 1, time);
            FadeImg.color = alpha;
            yield return null;
        }
         
        SceneManager.LoadScene("Main_Scene");
        yield return null;
    }
}
