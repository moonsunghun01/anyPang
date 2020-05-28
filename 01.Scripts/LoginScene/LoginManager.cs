using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GooglePlayGames;
using UnityEngine.SceneManagement;  
using UnityEngine.Networking;
using Newtonsoft.Json;
using System;
using System.IO;

public class LoginManager : MonoBehaviour
{
    public playerInfo pInfo;
    public playerSetting pSetting;

    public GameObject BlackObj;
    public Image FadeImg;
    float time = 0;
    string m_id;
     
    public GameObject LoginButton;
     
    AudioSource audioSource;
    public AudioClip audioClip;

    void Start()
    {
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();
        audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(audioClip);
    }
    private void Update()
    { 
    }
    public void Login()
    {
        Social.localUser.Authenticate((bool success) =>
        {
            if (success)
            {
                m_id = Social.localUser.id; 
                selectDataBase();
                BlackObj.SetActive(true);
                StartCoroutine(FadeFlow());
            }
        });
    }
    IEnumerator FadeFlow()
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

    //DB 조회
    public void selectDataBase()
    {
        StartCoroutine(selectStart());
    }
    IEnumerator selectStart()
    {
        WWWForm form = new WWWForm();

        form.AddField("ID", m_id);
        form.AddField("PASS", "asd123");
        string url = "msh4585.cafe24.com/Login2.php";
        UnityWebRequest webRequest = UnityWebRequest.Post(url, form);

        yield return webRequest.SendWebRequest();
        string str = webRequest.downloadHandler.text.ToString();
        string[] requsetData = str.Split('/');
        if (requsetData.Length < 2)
        {
            StartCoroutine(insertPlayer());
            saveJson(0, 0, 0, 0, 0, 0, 0, DateTime.Now);
        }
        else
        {
            saveJson(Convert.ToInt32(requsetData[0].Trim()), Convert.ToInt32(requsetData[1].Trim())
                , Convert.ToInt32(requsetData[2].Trim()), Convert.ToInt32(requsetData[3].Trim())
                , Convert.ToInt32(requsetData[4].Trim()), Convert.ToInt32(requsetData[5].Trim())
                , Convert.ToInt32(requsetData[6].Trim()), Convert.ToDateTime(requsetData[7].Trim()));
        }
    } 
    IEnumerator insertPlayer()
    {
        WWWForm form = new WWWForm();

        form.AddField("ID", m_id);
        form.AddField("DATETIME", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
        string url = "msh4585.cafe24.com/Insert2.php";
        UnityWebRequest webRequest = UnityWebRequest.Post(url, form);

        yield return webRequest.SendWebRequest(); 
    }

    public void saveJson(int score, int MaxScore, int MyFirstItemNum, int MySecondItemNum,int MyGold,int MyStamina,int MyStaminaTime,DateTime SaveDateTime)
    {
        playerInfo data;
        data = new playerInfo(score, MaxScore, MyFirstItemNum, MySecondItemNum, MyGold, MyStamina, MyStaminaTime, DateTime.Parse(SaveDateTime.ToString("yyyy-MM-dd hh:mm:ss")) );
        if (!Directory.Exists(Application.persistentDataPath + "/Json"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/Json");
        }
        string jdata = JsonConvert.SerializeObject(data);
        File.WriteAllText(Application.persistentDataPath + "/Json/save.json", jdata); 
    }
}
