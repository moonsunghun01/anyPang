using System.Collections;
using System.Collections.Generic;
using UnityEngine; 
using System.IO;
using System;
using Newtonsoft.Json;
using UnityEngine.Networking;

public class DieSceneManager : MonoBehaviour
{ 
    public playerInfo data;
    public playerSetting settingData;
    // 오디오
    AudioSource audioSource;
    public AudioClip audioClip;

    public int GainGold;
    void Start()
    { 
        string jdata = File.ReadAllText(Application.persistentDataPath + "/Json/save.json");
        data = JsonConvert.DeserializeObject<playerInfo>(jdata);
        LoadPlayerSettingData();
        GainGold = (data.score / 10);

        audioSource = GetComponent<AudioSource>();
        if (!settingData.audioOnOff) audioSource.mute = true;
        else audioSource.mute = false;
        audioSource.PlayOneShot(audioClip);
        StaminaCount();
        SaveJsonData();
    }

    void LoadPlayerSettingData()
    {
        string jdata = File.ReadAllText(Application.persistentDataPath + "/Json/playerSetting.json");
        if (jdata != null) settingData = JsonConvert.DeserializeObject<playerSetting>(jdata);
    }
    public void SaveJsonData()
    {
        data = new playerInfo(data.score, data.MaxScore, data.MyFirstItemNum, data.MySecondItemNum, data.MyGold + GainGold,data.MyStamina,data.MyStaminaTime, DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")));
        if (!Directory.Exists(Application.persistentDataPath + "/Json"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/Json");
        }
        string jdata = JsonConvert.SerializeObject(data);
        File.WriteAllText(Application.persistentDataPath + "/Json/save.json", jdata);
        StartCoroutine(updateDataBase(data));
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
    void StaminaCount()
    {
        if (data.MyStamina < 5)
        {
            DateTime nowTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
            TimeSpan countTime = nowTime - data.SaveDateTime; 

            Debug.Log("Day " + countTime.Days);
            Debug.Log("Hours " + countTime.Hours);
            Debug.Log("Minutes " + countTime.Minutes);
            Debug.Log("Seconds " + countTime.Seconds);
            data.MyStaminaTime = data.MyStaminaTime - countTime.Seconds;
            if (data.MyStaminaTime < 0)
            {
                data.MyStaminaTime += 60;
                data.MyStamina++;
            }

            if (countTime.Days > 0 || countTime.Hours > 0) data.MyStamina = 5;
            else
            {
                data.MyStamina += countTime.Minutes;
                if (data.MyStamina > 5)
                {
                    data.MyStaminaTime = 60;
                    data.MyStamina = 5;
                }
            }
        }
    }
}
