using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.SceneManagement;
public class DieUIManager : MonoBehaviour
{
    public Image FadeImg;
    float time = 0;
    float F_time = 10;
    bool isStart = false;

    DieSceneManager dieSceneManager;
    // Start is called before the first frame update
    void Start()
    {
        dieSceneManager = GameObject.Find("DieSceneManager").GetComponent<DieSceneManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isStart) StartCoroutine(FadeFlow());
    }

    IEnumerator FadeFlow()
    {
        FadeImg.gameObject.SetActive(true);
        Color alpha = FadeImg.color;
        while (alpha.a >0)
        {
            time += Time.deltaTime / F_time;
            alpha.a = Mathf.Lerp(1,0, time);
            FadeImg.color = alpha;
            yield return null;
        }
        FadeImg.gameObject.SetActive(false);
        isStart = true;
        yield return null;
    }

    public void ReStartGame()
    {
        if (dieSceneManager.data.MyStamina > 0)
        {
            dieSceneManager.data.MyStamina--;
            dieSceneManager.SaveJsonData();
            SceneManager.LoadScene("InGame_Scene");
        }
    }
    public void LoadMainGame()
    {
        SceneManager.LoadScene("Main_Scene");
    }
}
