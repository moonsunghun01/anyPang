using System.Collections;
using System.Collections.Generic;
using UnityEngine;  
using Newtonsoft.Json; 
using System.IO;
using System;
using UnityEngine.SceneManagement;
using DG.Tweening; 
using UnityEngine.Networking;

public class playerSetting
{
    public bool audioOnOff;
    public bool pushOnOff;
    public bool infoOnOff;

    public playerSetting(bool audioOnOff, bool pushOnOff, bool infoOnOff)
    {
        this.audioOnOff = audioOnOff;
        this.pushOnOff = pushOnOff;
        this.infoOnOff = infoOnOff;
    }
}

public class playerInfo
{ 
    public int score;
    public int MaxScore;
    public int MyFirstItemNum;
    public int MySecondItemNum;
    public int MyGold;
    public int MyStamina;
    public int MyStaminaTime;
    public DateTime SaveDateTime; 

    public playerInfo(int score, int MaxScore, int MyFirstItemNum, int MySecondItemNum, int MyGold, int MyStamina, int MyStaminaTime, DateTime SaveDateTime)
    { 
        this.score = score;
        this.MaxScore = MaxScore;
        this.MyFirstItemNum = MyFirstItemNum;
        this.MySecondItemNum = MySecondItemNum;
        this.MyGold = MyGold;
        this.MyStamina = MyStamina;
        this.MyStaminaTime = MyStaminaTime;
        this.SaveDateTime = SaveDateTime; 
    }
}
public class MainSceneManager : MonoBehaviour
{ 

    //JSON 데이터 저장
    public int MaxScore;
    public int MyFirstItemNum;
    public int MySecondItemNum;
    public int MyGold;
    public int MyStamina;
    public int MyStaminaTime;
    public DateTime SaveDateTime;

    public int staminaCountNum;

    //Setting Json Data
    public bool audioOnOff;
    public bool pushOnOff;
    public bool infoOnOff;

    public GameObject minusSG;
    public GameObject minusGoldPosition;
    public GameObject minusFIG;
    public GameObject minusSIG;
    // 오디오
    AudioSource audioSource;
    public AudioClip audioClip;

    // 세팅
    public GameObject SettingUI;
    void Start()
    {
        LoadJsonData();
        StaminaCount();
        LoadPlayerSettingJsonData();
        audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(audioClip);
    }
    private void Update()
    {
        if (audioOnOff) audioSource.mute = false;
        else if (!audioOnOff) audioSource.mute = true; 
    }
    void LoadPlayerSettingJsonData()
    {
        playerSetting settingData = null;
        string jdata = File.ReadAllText(Application.persistentDataPath + "/Json/playerSetting.json");
        if (jdata != null) settingData = JsonConvert.DeserializeObject<playerSetting>(jdata);
        if (settingData != null)
        {
            audioOnOff = settingData.audioOnOff;
            pushOnOff = settingData.pushOnOff;
            infoOnOff = settingData.infoOnOff;
        }
        else
        {
            audioOnOff = true;
            pushOnOff = true;
            infoOnOff = true;
        }
    } 
    void LoadJsonData()
    {
        playerInfo data = null;
        string jdata = File.ReadAllText(Application.persistentDataPath + "/Json/save.json");
        if (jdata != null) data = JsonConvert.DeserializeObject<playerInfo>(jdata);
        if (data != null)
        {
            MaxScore = data.MaxScore;
            MyFirstItemNum = data.MyFirstItemNum;
            MySecondItemNum = data.MySecondItemNum;
            MyGold = data.MyGold;
            MyStamina = data.MyStamina;
            MyStaminaTime = data.MyStaminaTime;
            SaveDateTime =  data.SaveDateTime;
        }
        else
        {
            MaxScore = 0;
            MyFirstItemNum = 0;
            MySecondItemNum = 0;
            MyGold = 0;
            MyStamina = 5;
            MyStaminaTime = 60;
            SaveDateTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
        }
    }

    void StaminaCount()
    {
        if(MyStamina <5)
        {
            DateTime nowTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
            TimeSpan countTime = nowTime - SaveDateTime;

            staminaCountNum = MyStaminaTime - countTime.Seconds;
            if (staminaCountNum <= 0)
            { 
                staminaCountNum = 60;
                MyStamina++;
            }

            if (countTime.Days > 0 || countTime.Hours > 0) MyStamina = 5;
            else
            {
               MyStamina += countTime.Minutes;
                if (MyStamina >= 5)
                {
                    MyStaminaTime = 60;
                    MyStamina = 5;
                }
            }
        } 
        else if (MyStamina == 5) staminaCountNum = 59;
    }
    public void StartGame()
    {
        if (MyStamina > 0)
        {
            MyStamina--;
            MyStaminaTime = 59;
            SaveJsonData();
            SavePlayerSettingJsonData();
            SceneManager.LoadScene("InGame_Scene");
        }
    }
    void SavePlayerSettingJsonData()
    {
        playerSetting data;
        data = new playerSetting(audioOnOff, pushOnOff, infoOnOff);
        if (!Directory.Exists(Application.persistentDataPath + "/Json"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/Json");
        }
        string jdata = JsonConvert.SerializeObject(data);
        File.WriteAllText(Application.persistentDataPath + "/Json/playerSetting.json", jdata);
    }

    public void SaveJsonData()
    { 
        playerInfo data;
        data = new playerInfo(0, MaxScore, MyFirstItemNum, MySecondItemNum, MyGold, MyStamina,MyStaminaTime, DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")));
        if (!Directory.Exists(Application.persistentDataPath + "/Json"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/Json");
        }
        string jdata = JsonConvert.SerializeObject(data);
        File.WriteAllText(Application.persistentDataPath + "/Json/save.json", jdata);
        StartCoroutine( updateDataBase(data));
    }

    IEnumerator updateDataBase(playerInfo data)
    {
        WWWForm form = new WWWForm();
        form.AddField("ID", Social.localUser.id);
        form.AddField("MAXSCORE", data.MaxScore);
        form.AddField("SCORE", data.score);
        form.AddField("MYFIRSTITEMNUM", data.MyFirstItemNum);
        form.AddField("MYSECONDITEMNUM", data.MySecondItemNum);
        form.AddField("MYGOLD", data.MyGold);
        form.AddField("MYSTAMINA", data.MyStamina);
        form.AddField("MYSTAMINATIME", data.MyStaminaTime);
        form.AddField("SAVEDATETIME", data.SaveDateTime.ToString("yyyy-MM-dd hh:mm:ss"));

        string url = "msh4585.cafe24.com/Update2.php";
        UnityWebRequest webRequest = UnityWebRequest.Post(url, form);
        yield return webRequest.SendWebRequest();
        Debug.Log(webRequest.downloadHandler.text);
    }

    public void BuyFirstItem()
    {
        if (MyGold >= 500 && MyFirstItemNum < 9)
        {
            GameObject temp = Instantiate(minusFIG, minusGoldPosition.transform);
            temp.transform.DOMoveY(-0.3f, 0.5f);
            Destroy(temp, 0.5f);
            MyGold -= 500;
            MyFirstItemNum++;
            SaveJsonData();
        }
    }
    public void BuySecondItem()
    {
        if (MyGold >= 300 && MySecondItemNum < 9)
        {
            GameObject temp = Instantiate(minusSIG, minusGoldPosition.transform);
            temp.transform.DOMoveY(-0.3f, 0.5f);
            Destroy(temp, 0.5f);
            MyGold -= 300;
            MySecondItemNum++;
            SaveJsonData();
        }
    }
    public void BuyStamina()
    {
        if (MyGold >= 1000 && MyStamina < 5)
        {
            GameObject temp = Instantiate(minusSG, minusGoldPosition.transform);
            temp.transform.DOMoveY(-0.3f, 0.5f);
            Destroy(temp, 0.5f);
            MyGold -= 1000;
            MyStamina++;
            SaveJsonData();
        }
    }
    public void OpenSettingUI()
    {
        SettingUI.SetActive(true);
    }

    public void ClickAudioOnOffButton() { audioOnOff = !audioOnOff; }   
    public void ClickPushOnOffButton() { pushOnOff = !pushOnOff; }   
    public void ClickInfoOnOffButton() { infoOnOff = !infoOnOff; }   

    public void ClickSettingUIOKButton()
    {
        SavePlayerSettingJsonData();
        SettingUI.SetActive(false);
    } 

    public void ClickSettingUIExitButton()
    {
        SettingUI.SetActive(false);
    }

    // 종료 시점 
    private void OnApplicationQuit()
    {
        SaveJsonData();
    }
}
