using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GooglePlayGames; 
using UnityEngine.SceneManagement;

public class GoogleManager : MonoBehaviour
{
    public Text LogText;
    public GameObject LoginButton;
    public GameObject LogOutButton;
     
    // Start is called before the first frame update
    void Start()
    {
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();
        Login();

    }
    public void Login()
    {
        Social.localUser.Authenticate((bool success) =>
        {
            //if (success) LogText.text = Social.localUser.id + " \n" + Social.localUser.userName;
            if (success)  LogText.text = Social.localUser.userName; 
            else LogText.text = "로그인 실패";
            LogOutButton.SetActive(true);
            LoginButton.SetActive(false);
        });
    }

    public void LogOut()
    {
        ((PlayGamesPlatform)Social.Active).SignOut();
        LogText.text = "게스트";
        LogOutButton.SetActive(false);
        LoginButton.SetActive(true); 
        SceneManager.LoadScene("Login_Scene");
    } 
}
