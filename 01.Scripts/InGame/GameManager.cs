using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Newtonsoft.Json;
using System; 
using System.IO;
using UnityEngine.Networking;

enum BLOCK
{
    BLANK = -1,
}
 
public class GameManager : MonoBehaviour
{

    public GameObject OriginBlock;          // 블럭의 원본
    public Sprite[] BlockType;              // 블럭 이미지 배열 

    public int iBlockX, iBlockY;            // 블럭보드의 가로 세로 크기 
    public int[][] BlockBoard;              // 블럭 보드

    private bool bBlockMoveDown = false;    // 블럭을 움직이게 해주는 플래그
    private bool bBlockChange = false;      // 블럭과 블럭을 바꾼다
    private bool bBlockReChange = false;    // 블럭과 블럭이 바뀐 상태에서 다시 바꿔준다
    private bool bReCheck = false;

    private Vector3 MouseStartPos;          // 자신이 첫번쨰로 선택한 블럭의 위치
    private Vector3 MouseEndPos;            // 현재 드래그 중인 마우스의 위치
    private Vector3 MouseOffset;            // 첫번쨰로 선택한 블럭으 위치로부터 현재 마우스 위치까지

    private Vector3 StartPos1, StartPos2;
    private Vector3 EndPos1, EndPos2;

    private GameObject SelectBlock;         // 자신이 첫 번째로 선택한 블럭
    private GameObject TargetBlcok;

    private float fMouseMoveDis = 30f;      // 첫 번째 블럭의 위치로부터 현재 마우스 까지의 허용 거리
    private float fMoveSpeed = 5f;          // 블럭의 이동 속력
    private float fBlockMoveStep = 0;       
     
    private bool bDrag = true;              // 드래그 가능, 불가능 플래그
     
    bool bBlockMoveEnd = false;
    bool bReset = true;

    // 이동 횟수
    public int MoveCount = 20;

    // JSON/////////////////////////////////
    // 게임 스코어  
    public int MaxScore;
    public int MyScore;

    // 게임 시간
    public float MyTime ;
    public bool isDeath ;

    public int MyStaminaTime;
    DateTime SaveDateTime; 
    /// ///////////////////////////////////////////////  

    AudioSource audioSource;
    public AudioClip audioClip; 
    
    public float hintTime = 0;
    public float restartTime = 0;

    // 아이템 사용 확인
    bool isItem;
    bool isWidthBlock;
    bool isHeightBlock;
    int blockCount;

    public bool widthCheck = false;
    public bool heightCheck = false;
    public bool isRestart = false;

    public const int BlockNum = 8;
    bool isFirstBlockCreate; 

    // 아이템 개수
    public int MyFirstItemNum;
    public int MySecondItemNum;

    public int MyGold;
    public int MyStamina;

    public bool isFirstItemActive;
    public bool isSecondItemActive;

    // 시작
    public bool isStart;
    public bool isPause;
    playerSetting settingData = null;
    public bool isMute;

    private void Awake()
    {
        BlockBoard = new int[iBlockX][];
        for (int i = 0; i < iBlockX; i++)
        {
            BlockBoard[i] = new int[iBlockY];
        }
         
        DOTween.Init(); 
        CreateBlcok();

       // DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        LoadJsonData();
        LoadPlayerSettingJsonData();

        audioSource = GetComponent<AudioSource>();
        if (!settingData.audioOnOff) audioSource.mute = true;
        else audioSource.mute = false; 
        audioSource.PlayOneShot(audioClip);

        MyScore = 0;
        MyTime = 60;
        isDeath = false;
        isStart = false;
        isPause = false;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            RestartGame();
        }
        if(!bBlockMoveEnd && !bBlockChange && !bBlockReChange && !isRestart && isStart) MouseClick();                   // 마우스 클릭 처리 

        if (!isDeath && isStart)
        {
            BlockDeleteToMoveCheck();       // 블럭을 삭제 후 움직이는지 체크
            DragToMoveBlock();              // 마우스로 블럭 이동
            ReChangeBlock();                // 블럭을 다시 바꿔준다
            BlockHint();
            if (!bBlockMoveEnd) BlockMovePossibleCheck();

            // 시간
            if(!isRestart && !isFirstItemActive && !isSecondItemActive && !isPause)MyTime -= Time.deltaTime;

            if (MyTime < 0)
            {
                audioSource.Stop();
                isDeath = true;
                SaveJson();
            }
        }
    } 
    void LoadJsonData()
    {
        playerInfo data = null;
        string jdata = File.ReadAllText(Application.persistentDataPath + "/Json/save.json");
        if(jdata != null) data = JsonConvert.DeserializeObject<playerInfo>(jdata);
        if (data != null)
        {
            MaxScore = data.MaxScore;
            MyFirstItemNum = data.MyFirstItemNum;
            MySecondItemNum = data.MySecondItemNum;
            MyGold = data.MyGold;
            MyStamina = data.MyStamina;
            MyStaminaTime = data.MyStaminaTime;
            SaveDateTime = data.SaveDateTime;
        } 
        else
        {
            MaxScore = 0;
            MyFirstItemNum = 0;
            MySecondItemNum = 0;
            MyGold = 0;
            MyStamina = 5;
        }
    }

    void LoadPlayerSettingJsonData()
    { 
        string jdata = File.ReadAllText(Application.persistentDataPath + "/Json/playerSetting.json");
        if (jdata != null) settingData = JsonConvert.DeserializeObject<playerSetting>(jdata);
        isMute = !settingData.audioOnOff;
    }
    void SaveJson()
    { 
        playerInfo data;
        data = new playerInfo(MyScore,MaxScore,MyFirstItemNum,MySecondItemNum,MyGold,MyStamina,MyStaminaTime,SaveDateTime);
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
    void BlockMovePossibleCheck()
    {
        GameObject[] blocks = GameObject.FindGameObjectsWithTag("Block");
        // 가로 체크
        for (int y = 0; y < iBlockY; y++)
        {
            for (int x = 0; x < iBlockX - 2; x++)
            {
                if (x < iBlockX - 3)
                { 
                    if (BlockBoard[x][y] == BlockBoard[x + 1][y])
                    {
                        if (BlockBoard[x][y] == BlockBoard[x + 3][y])
                        { 
                            widthCheck = true;
                            return;
                        }
                    } 
                    if (BlockBoard[x][y] == BlockBoard[x + 2][y])
                    {
                        if (BlockBoard[x][y] == BlockBoard[x + 3][y])
                        { 
                            widthCheck = true;
                            return;
                        }
                    }
                }
                if (y < iBlockY - 1)
                { 
                    if (BlockBoard[x][y + 1] == BlockBoard[x + 1][y])
                    {
                        if (BlockBoard[x][y + 1] == BlockBoard[x + 2][y])
                        { 
                            widthCheck = true;
                            return;
                        }
                    } 
                    if (BlockBoard[x][y] == BlockBoard[x + 1][y + 1])
                    {
                        if (BlockBoard[x][y] == BlockBoard[x + 2][y])
                        {
                            
                            widthCheck = true;
                            return;
                        }
                    } 
                    if (BlockBoard[x][y] == BlockBoard[x + 1][y])
                    {
                        if (BlockBoard[x][y] == BlockBoard[x + 2][y + 1])
                        { 
                            widthCheck = true;
                            return;
                        }
                    } 
                    if (BlockBoard[x][y] == BlockBoard[x + 1][y + 1])
                    {
                        if (BlockBoard[x][y] == BlockBoard[x + 2][y + 1])
                        { 
                            widthCheck = true;
                            return;
                        }
                    }
                }
                if (y > 0)
                { 
                    if (BlockBoard[x][y] == BlockBoard[x + 1][y - 1])
                    {
                        if (BlockBoard[x][y] == BlockBoard[x + 2][y])
                        { 
                            widthCheck = true;
                            return;
                        }
                    } 
                    if (BlockBoard[x][y] == BlockBoard[x + 1][y])
                    {
                        if (BlockBoard[x][y] == BlockBoard[x + 2][y - 1])
                        { 
                            widthCheck = true;
                            return;
                        }
                    }
                }
            }
        }
        if (!widthCheck) Debug.Log("가로없음");
        // 세로체크

        for (int x = 0; x < iBlockX; x++)
        {
            for (int y = 0; y < iBlockY - 2; y++)
            {
                if (y < iBlockY - 3)
                { 
                    if (BlockBoard[x][y] == BlockBoard[x][y + 2])
                    {
                        if (BlockBoard[x][y] == BlockBoard[x][y + 3])
                        { 
                            heightCheck = true;
                            return;
                        }
                    } 
                    if (BlockBoard[x][y] == BlockBoard[x][y + 1])
                    {
                        if (BlockBoard[x][y] == BlockBoard[x][y + 3])
                        { 
                            heightCheck = true;
                            return;
                        }
                    }
                }
                if (x < iBlockX - 1)
                { 
                    if (BlockBoard[x][y] == BlockBoard[x + 1][y + 1])
                    {
                        if (BlockBoard[x][y] == BlockBoard[x + 1][y + 2])
                        { 
                            heightCheck = true;
                            return;
                        }
                    } 
                    if (BlockBoard[x][y] == BlockBoard[x + 1][y + 1])
                    {
                        if (BlockBoard[x][y] == BlockBoard[x][y + 2])
                        { 
                            heightCheck = true;
                            return;
                        }
                    } 
                    if (BlockBoard[x][y] == BlockBoard[x][y + 1])
                    {
                        if (BlockBoard[x][y] == BlockBoard[x + 1][y + 2])
                        { 
                            heightCheck = true;
                            return;
                        }
                    }
                }
                if (x > 0)
                { 
                    if (BlockBoard[x][y] == BlockBoard[x - 1][y + 1])
                    {
                        if (BlockBoard[x][y] == BlockBoard[x][y + 2])
                        { 
                            heightCheck = true;
                            return;
                        }
                    } 
                    if (BlockBoard[x][y] == BlockBoard[x][y + 1])
                    {
                        if (BlockBoard[x][y] == BlockBoard[x - 1][y + 2])
                        {
                            
                            heightCheck = true;
                            return;
                        }
                    } 
                    if (BlockBoard[x][y] == BlockBoard[x - 1][y + 1])
                    {
                        if (BlockBoard[x][y] == BlockBoard[x - 1][y + 2])
                        { 
                            heightCheck = true;
                            return;
                        }
                    }
                }
            }
        }

        if (!heightCheck) Debug.Log("세로없음");
        if (!bBlockMoveEnd )
        {
            restartTime += Time.deltaTime;
            if (restartTime > 1)
            {
                restartTime = 0;
                isRestart = true;
            } 
        }
    }
    void BlockHint()
    { 
        if(!bBlockMoveDown) hintTime += Time.deltaTime; 
        if (hintTime > 5)
        {
            if (!isFirstItemActive && !isSecondItemActive) BlockHintCheck();
            else AllBlockShake();
        }
    }
    void AllBlockShake()
    {
        GameObject[] blocks = GameObject.FindGameObjectsWithTag("Block");
        // 가로 체크
        for (int y = 0; y < iBlockY; y++)
        {
            for (int x = 0; x < iBlockX; x++)
            {
                foreach (GameObject block in blocks)
                {
                    Block sBlock = block.GetComponent<Block>(); 

                    if (sBlock.iX == x && sBlock.iY == y)
                    {
                        sBlock.transform.DOPunchScale(new Vector3(0.05f, 0.05f, 0.05f), 1);
                        continue;
                    } 
                }
            }
        }
        hintTime = 0; 
    }
    void BlockHintCheck()
    {
        GameObject[] blocks = GameObject.FindGameObjectsWithTag("Block");
        // 가로 체크
        for (int y = 0; y < iBlockY; y++)
        {
            for (int x = 0; x < iBlockX - 2; x++)
            {
                if (x < iBlockX - 3)
                {
                    //ooxo
                    if (BlockBoard[x][y] == BlockBoard[x + 1][y])
                    {
                        if (BlockBoard[x][y] == BlockBoard[x + 3][y])
                        {
                            foreach (GameObject block in blocks)
                            {
                                Block sBlock = block.GetComponent<Block>();
                                if (sBlock.iY != y) continue;

                                if (sBlock.iX == x && sBlock.iY == y)
                                {
                                    sBlock.transform.DOPunchScale(new Vector3(0.05f, 0.05f, 0.05f), 1);
                                    continue;
                                }

                                if (sBlock.iX == x + 1 && sBlock.iY == y)
                                {
                                    sBlock.transform.DOPunchScale(new Vector3(0.05f, 0.05f, 0.05f), 1);
                                    continue;
                                }

                                if (sBlock.iX == x + 3 && sBlock.iY == y)
                                    sBlock.transform.DOPunchScale(new Vector3(0.05f, 0.05f, 0.05f), 1);
                            }
                            hintTime = 0;
                            return;
                        }
                    }
                    //oxoo
                    if (BlockBoard[x][y] == BlockBoard[x + 2][y])
                    {
                        if (BlockBoard[x][y] == BlockBoard[x + 3][y])
                        {
                            foreach (GameObject block in blocks)
                            {
                                Block sBlock = block.GetComponent<Block>();
                                if (sBlock.iY != y) continue;

                                if (sBlock.iX == x && sBlock.iY == y)
                                {
                                    sBlock.transform.DOPunchScale(new Vector3(0.05f, 0.05f, 0.05f), 1);
                                    continue;
                                }

                                if (sBlock.iX == x + 2 && sBlock.iY == y)
                                {
                                    sBlock.transform.DOPunchScale(new Vector3(0.05f, 0.05f, 0.05f), 1);
                                    continue;
                                }

                                if (sBlock.iX == x + 3 && sBlock.iY == y)
                                    sBlock.transform.DOPunchScale(new Vector3(0.05f, 0.05f, 0.05f), 1);
                            }
                            hintTime = 0;
                            return;
                        }
                    }
                }
                if (y < iBlockY - 1)
                {
                    //oxx
                    //xoo
                    if (BlockBoard[x][y + 1] == BlockBoard[x + 1][y])
                    {
                        if (BlockBoard[x][y + 1] == BlockBoard[x + 2][y])
                        {
                            foreach (GameObject block in blocks)
                            {
                                Block sBlock = block.GetComponent<Block>();
                                //if (sBlock.iY != y) continue;

                                if (sBlock.iX == x && sBlock.iY == y + 1)
                                {
                                    sBlock.transform.DOPunchScale(new Vector3(0.05f, 0.05f, 0.05f), 1);
                                    continue;
                                }

                                if (sBlock.iX == x + 1 && sBlock.iY == y)
                                {
                                    sBlock.transform.DOPunchScale(new Vector3(0.05f, 0.05f, 0.05f), 1);
                                    continue;
                                }

                                if (sBlock.iX == x + 2 && sBlock.iY == y)
                                    sBlock.transform.DOPunchScale(new Vector3(0.05f, 0.05f, 0.05f), 1);
                            }
                            hintTime = 0;
                            return;
                        }
                    }

                    //xox
                    //oxo
                    if (BlockBoard[x][y] == BlockBoard[x + 1][y + 1])
                    {
                        if (BlockBoard[x][y] == BlockBoard[x + 2][y])
                        {
                            foreach (GameObject block in blocks)
                            {
                                Block sBlock = block.GetComponent<Block>();
                                //if (sBlock.iY != y) continue;

                                if (sBlock.iX == x && sBlock.iY == y)
                                {
                                    sBlock.transform.DOPunchScale(new Vector3(0.05f, 0.05f, 0.05f), 1);
                                    continue;
                                }

                                if (sBlock.iX == x + 1 && sBlock.iY == y + 1)
                                {
                                    sBlock.transform.DOPunchScale(new Vector3(0.05f, 0.05f, 0.05f), 1);
                                    continue;
                                }

                                if (sBlock.iX == x + 2 && sBlock.iY == y)
                                    sBlock.transform.DOPunchScale(new Vector3(0.05f, 0.05f, 0.05f), 1);
                            }
                            hintTime = 0;
                            return;
                        }
                    }

                    //xxo
                    //oox
                    if (BlockBoard[x][y] == BlockBoard[x + 1][y])
                    {
                        if (BlockBoard[x][y] == BlockBoard[x + 2][y + 1])
                        {
                            foreach (GameObject block in blocks)
                            {
                                Block sBlock = block.GetComponent<Block>();
                                //if (sBlock.iY != y) continue;

                                if (sBlock.iX == x && sBlock.iY == y)
                                {
                                    sBlock.transform.DOPunchScale(new Vector3(0.05f, 0.05f, 0.05f), 1);
                                    continue;
                                }

                                if (sBlock.iX == x + 1 && sBlock.iY == y)
                                {
                                    sBlock.transform.DOPunchScale(new Vector3(0.05f, 0.05f, 0.05f), 1);
                                    continue;
                                }

                                if (sBlock.iX == x + 2 && sBlock.iY == y + 1)
                                    sBlock.transform.DOPunchScale(new Vector3(0.05f, 0.05f, 0.05f), 1);
                            }
                            hintTime = 0;
                            return;
                        }
                    }
                    //xoo
                    //oxx
                    if (BlockBoard[x][y] == BlockBoard[x + 1][y + 1])
                    {
                        if (BlockBoard[x][y] == BlockBoard[x + 2][y + 1])
                        {
                            foreach (GameObject block in blocks)
                            {
                                Block sBlock = block.GetComponent<Block>();
                                //if (sBlock.iY != y) continue;

                                if (sBlock.iX == x && sBlock.iY == y)
                                {
                                    sBlock.transform.DOPunchScale(new Vector3(0.05f, 0.05f, 0.05f), 1);
                                    continue;
                                }

                                if (sBlock.iX == x + 1 && sBlock.iY == y + 1)
                                {
                                    sBlock.transform.DOPunchScale(new Vector3(0.05f, 0.05f, 0.05f), 1);
                                    continue;
                                }

                                if (sBlock.iX == x + 2 && sBlock.iY == y + 1)
                                    sBlock.transform.DOPunchScale(new Vector3(0.05f, 0.05f, 0.05f), 1);
                            }
                            hintTime = 0;
                            return;
                        }
                    }
                }
                if (y > 0)
                {
                    //oxo
                    //xox 
                    if (BlockBoard[x][y] == BlockBoard[x + 1][y - 1])
                    {
                        if (BlockBoard[x][y] == BlockBoard[x + 2][y])
                        {
                            foreach (GameObject block in blocks)
                            {
                                Block sBlock = block.GetComponent<Block>();
                                //if (sBlock.iY != y) continue;

                                if (sBlock.iX == x && sBlock.iY == y)
                                {
                                    sBlock.transform.DOPunchScale(new Vector3(0.05f, 0.05f, 0.05f), 1);
                                    continue;
                                }

                                if (sBlock.iX == x + 1 && sBlock.iY == y - 1)
                                {
                                    sBlock.transform.DOPunchScale(new Vector3(0.05f, 0.05f, 0.05f), 1);
                                    continue;
                                }

                                if (sBlock.iX == x + 2 && sBlock.iY == y)
                                    sBlock.transform.DOPunchScale(new Vector3(0.05f, 0.05f, 0.05f), 1);
                            }
                            hintTime = 0;
                            return;
                        }
                    }
                    //oox
                    //xxo 
                    if (BlockBoard[x][y] == BlockBoard[x + 1][y])
                    {
                        if (BlockBoard[x][y] == BlockBoard[x + 2][y - 1])
                        {
                            foreach (GameObject block in blocks)
                            {
                                Block sBlock = block.GetComponent<Block>();
                                //if (sBlock.iY != y) continue;

                                if (sBlock.iX == x && sBlock.iY == y)
                                {
                                    sBlock.transform.DOPunchScale(new Vector3(0.05f, 0.05f, 0.05f), 1);
                                    continue;
                                }

                                if (sBlock.iX == x + 1 && sBlock.iY == y)
                                {
                                    sBlock.transform.DOPunchScale(new Vector3(0.05f, 0.05f, 0.05f), 1);
                                    continue;
                                }

                                if (sBlock.iX == x + 2 && sBlock.iY == y - 1)
                                    sBlock.transform.DOPunchScale(new Vector3(0.05f, 0.05f, 0.05f), 1);
                            }
                            hintTime = 0;
                            return;
                        }
                    }
                }
            }
        } 
        // 세로체크

        for (int x = 0; x < iBlockX; x++)
        {
            for (int y = 0; y < iBlockY - 2; y++)
            {
                if (y < iBlockY - 3)
                {
                    //o
                    //o
                    //x
                    //o
                    if (BlockBoard[x][y] == BlockBoard[x][y + 2])
                    {
                        if (BlockBoard[x][y] == BlockBoard[x][y + 3])
                        {
                            foreach (GameObject block in blocks)
                            {
                                Block sBlock = block.GetComponent<Block>();
                                //if (sBlock.iY != y) continue;

                                if (sBlock.iX == x && sBlock.iY == y)
                                {
                                    sBlock.transform.DOPunchScale(new Vector3(0.05f, 0.05f, 0.05f), 1);
                                    continue;
                                }

                                if (sBlock.iX == x && sBlock.iY == y + 2)
                                {
                                    sBlock.transform.DOPunchScale(new Vector3(0.05f, 0.05f, 0.05f), 1);
                                    continue;
                                }

                                if (sBlock.iX == x && sBlock.iY == y + 3)
                                    sBlock.transform.DOPunchScale(new Vector3(0.05f, 0.05f, 0.05f), 1);
                            }
                            hintTime = 0;
                            return;
                        }
                    }

                    //o
                    //x
                    //o
                    //o
                    if (BlockBoard[x][y] == BlockBoard[x][y + 1])
                    {
                        if (BlockBoard[x][y] == BlockBoard[x][y + 3])
                        {
                            foreach (GameObject block in blocks)
                            {
                                Block sBlock = block.GetComponent<Block>();
                                //if (sBlock.iY != y) continue;

                                if (sBlock.iX == x && sBlock.iY == y)
                                {
                                    sBlock.transform.DOPunchScale(new Vector3(0.05f, 0.05f, 0.05f), 1);
                                    continue;
                                }

                                if (sBlock.iX == x && sBlock.iY == y + 1)
                                {
                                    sBlock.transform.DOPunchScale(new Vector3(0.05f, 0.05f, 0.05f), 1);
                                    continue;
                                }

                                if (sBlock.iX == x && sBlock.iY == y + 3)
                                    sBlock.transform.DOPunchScale(new Vector3(0.05f, 0.05f, 0.05f), 1);
                            }
                            hintTime = 0;
                            return;
                        }
                    }
                }
                if (x < iBlockX - 1)
                {
                    //xo
                    //xo
                    //ox
                    if (BlockBoard[x][y] == BlockBoard[x + 1][y + 1])
                    {
                        if (BlockBoard[x][y] == BlockBoard[x + 1][y + 2])
                        {
                            foreach (GameObject block in blocks)
                            {
                                Block sBlock = block.GetComponent<Block>();
                                //if (sBlock.iY != y) continue;

                                if (sBlock.iX == x && sBlock.iY == y)
                                {
                                    sBlock.transform.DOPunchScale(new Vector3(0.05f, 0.05f, 0.05f), 1);
                                    continue;
                                }

                                if (sBlock.iX == x + 1 && sBlock.iY == y + 1)
                                {
                                    sBlock.transform.DOPunchScale(new Vector3(0.05f, 0.05f, 0.05f), 1);
                                    continue;
                                }

                                if (sBlock.iX == x + 1 && sBlock.iY == y + 2)
                                    sBlock.transform.DOPunchScale(new Vector3(0.05f, 0.05f, 0.05f), 1);
                            }
                            hintTime = 0;
                            return;
                        }
                    }

                    //ox
                    //xo
                    //ox
                    if (BlockBoard[x][y] == BlockBoard[x + 1][y + 1])
                    {
                        if (BlockBoard[x][y] == BlockBoard[x][y + 2])
                        {
                            foreach (GameObject block in blocks)
                            {
                                Block sBlock = block.GetComponent<Block>();
                                //if (sBlock.iY != y) continue;

                                if (sBlock.iX == x && sBlock.iY == y)
                                {
                                    sBlock.transform.DOPunchScale(new Vector3(0.05f, 0.05f, 0.05f), 1);
                                    continue;
                                }

                                if (sBlock.iX == x + 1 && sBlock.iY == y + 1)
                                {
                                    sBlock.transform.DOPunchScale(new Vector3(0.05f, 0.05f, 0.05f), 1);
                                    continue;
                                }

                                if (sBlock.iX == x && sBlock.iY == y + 2)
                                    sBlock.transform.DOPunchScale(new Vector3(0.05f, 0.05f, 0.05f), 1);
                            }
                            hintTime = 0;
                            return;
                        }
                    }

                    //xo
                    //ox
                    //ox
                    if (BlockBoard[x][y] == BlockBoard[x][y + 1])
                    {
                        if (BlockBoard[x][y] == BlockBoard[x + 1][y + 2])
                        {
                            foreach (GameObject block in blocks)
                            {
                                Block sBlock = block.GetComponent<Block>();
                                //if (sBlock.iY != y) continue;

                                if (sBlock.iX == x && sBlock.iY == y)
                                {
                                    sBlock.transform.DOPunchScale(new Vector3(0.05f, 0.05f, 0.05f), 1);
                                    continue;
                                }

                                if (sBlock.iX == x && sBlock.iY == y + 1)
                                {
                                    sBlock.transform.DOPunchScale(new Vector3(0.05f, 0.05f, 0.05f), 1);
                                    continue;
                                }

                                if (sBlock.iX == x +1&& sBlock.iY == y + 2)
                                    sBlock.transform.DOPunchScale(new Vector3(0.05f, 0.05f, 0.05f), 1);
                            }
                            hintTime = 0;
                            return;
                        }
                    }
                }
                if (x > 0)
                {
                    //xo
                    //ox
                    //xo
                    if (BlockBoard[x][y] == BlockBoard[x - 1][y + 1])
                    {
                        if (BlockBoard[x][y] == BlockBoard[x][y + 2])
                        {
                            foreach (GameObject block in blocks)
                            {
                                Block sBlock = block.GetComponent<Block>();
                                //if (sBlock.iY != y) continue;

                                if (sBlock.iX == x && sBlock.iY == y)
                                {
                                    sBlock.transform.DOPunchScale(new Vector3(0.05f, 0.05f, 0.05f), 1);
                                    continue;
                                }

                                if (sBlock.iX == x - 1 && sBlock.iY == y + 1)
                                {
                                    sBlock.transform.DOPunchScale(new Vector3(0.05f, 0.05f, 0.05f), 1);
                                    continue;
                                }

                                if (sBlock.iX == x && sBlock.iY == y + 2)
                                    sBlock.transform.DOPunchScale(new Vector3(0.05f, 0.05f, 0.05f), 1);
                            }
                            hintTime = 0;
                            return;
                        }
                    }

                    //ox
                    //xo
                    //xo
                    if (BlockBoard[x][y] == BlockBoard[x][y + 1])
                    {
                        if (BlockBoard[x][y] == BlockBoard[x - 1][y + 2])
                        {
                            foreach (GameObject block in blocks)
                            {
                                Block sBlock = block.GetComponent<Block>();
                                //if (sBlock.iY != y) continue;

                                if (sBlock.iX == x && sBlock.iY == y)
                                {
                                    sBlock.transform.DOPunchScale(new Vector3(0.05f, 0.05f, 0.05f), 1);
                                    continue;
                                }

                                if (sBlock.iX == x && sBlock.iY == y + 1)
                                {
                                    sBlock.transform.DOPunchScale(new Vector3(0.05f, 0.05f, 0.05f), 1);
                                    continue;
                                }

                                if (sBlock.iX == x - 1 && sBlock.iY == y + 2)
                                    sBlock.transform.DOPunchScale(new Vector3(0.05f, 0.05f, 0.05f), 1);
                            }
                            hintTime = 0;
                            return;
                        }
                    }

                    //ox
                    //ox
                    //xo
                    if (BlockBoard[x][y] == BlockBoard[x - 1][y + 1])
                    {
                        if (BlockBoard[x][y] == BlockBoard[x - 1][y + 2])
                        {
                            foreach (GameObject block in blocks)
                            {
                                Block sBlock = block.GetComponent<Block>();
                                //if (sBlock.iY != y) continue;

                                if (sBlock.iX == x && sBlock.iY == y)
                                {
                                    sBlock.transform.DOPunchScale(new Vector3(0.05f, 0.05f, 0.05f), 1);
                                    continue;
                                }

                                if (sBlock.iX == x - 1 && sBlock.iY == y + 1)
                                {
                                    sBlock.transform.DOPunchScale(new Vector3(0.05f, 0.05f, 0.05f), 1);
                                    continue;
                                }

                                if (sBlock.iX == x - 1 && sBlock.iY == y + 2)
                                    sBlock.transform.DOPunchScale(new Vector3(0.05f, 0.05f, 0.05f), 1);
                            }
                            hintTime = 0;
                            return;
                        }
                    }
                }
            }
        }

        //if (!heightCheck) Debug.Log("세로없음");
        //if (!widthCheck && !heightCheck)
        //{
        //    Debug.Log("재시작");
        //    restartTime += Time.deltaTime;
        //    if(restartTime > 3) RestartGame();
        //}
    }


    public void RestartGame()
    { 
        isRestart = false;
        restartTime = 0;
        hintTime = 0;
        widthCheck = false;
        heightCheck = false;
        GameObject[] blocks = GameObject.FindGameObjectsWithTag("Block");
        foreach (GameObject block in blocks) Destroy(block);
        CreateBlcok();
    }

    // 블럭이 이동중인지 체크
    void BlockDeleteToMoveCheck()
    {
        // 블럭 삭제 후 블럭이 이동중인지 체크
        if(bBlockMoveDown)
        {
            // 블럭이 움직인다 : true
            //bool bBlockMoveEnd = false;
            bBlockMoveEnd = false;
            GameObject[] blocks = GameObject.FindGameObjectsWithTag("Block");

            foreach(GameObject block in blocks)
            {
                // 움직이는 블럭이 있는지 bDown 변수를 체크 하여 확인
                if(block.GetComponent<Block>().bDown)
                {
                    // 움직이는 블럭이 있으면 bBlockMoveEnd = true
                    bBlockMoveEnd = true;
                    break;
                 }
            }
            // 블럭이 움직이지 않는다면 
            if(!bBlockMoveEnd)
            {
                bBlockMoveDown = false;
                BlockDown();
            }
        }
    }  
    // 마우스 클릭 이동
    void MouseClick()
    {
        Block tempBlock = null; 
        // 마우스 왼쪽 버튼 클릭
        if (Input.GetMouseButtonDown(0))
        { 
            hintTime = 0;
            RaycastHit hit;
            MouseStartPos = Input.mousePosition;                            // 마우스의 현재 위치
            Ray ray = Camera.main.ScreenPointToRay(MouseStartPos);          // 마우스 위치 레이로 변환
            hintTime = 0;
            // 레이 발사
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                // 레이에 맞은 오브젝트가 Block인지 판별 
                if (hit.collider.CompareTag("Block"))
                {
                    // 맞은 블럭 오브젝트를 SelectBlock 에 초기화
                    SelectBlock = hit.collider.gameObject;
                    StartPos1 = SelectBlock.transform.position;

                    tempBlock = SelectBlock.GetComponent<Block>();

                    // 스킬 사용
                    if (isFirstItemActive && tempBlock.iType < 9 && MyFirstItemNum >0)
                    {
                       MyFirstItemNum--;
                       StartCoroutine( FirstItemActive(tempBlock.iType));
                    }
                    if (isSecondItemActive && tempBlock.iType < 9 && MySecondItemNum > 0)
                    {
                        MySecondItemNum--;
                        StartCoroutine(SecondItemActive(tempBlock.iX, tempBlock.iY));
                    }
                    ///////////////////// 아이템
                    else
                    {
                        if (tempBlock.iType == 9 && !isFirstItemActive && !isSecondItemActive)
                        {
                            SelectBlock = null;
                            isItem = true;
                            StartCoroutine(widthDelete(tempBlock.iY));
                        }
                        if (tempBlock.iType == 10 && !isFirstItemActive && !isSecondItemActive)
                        {
                            SelectBlock = null;
                            isItem = true;
                            StartCoroutine(heightDelete(tempBlock.iX));
                        }
                        if (tempBlock.iType == 11 && !isFirstItemActive && !isSecondItemActive)
                        {
                            SelectBlock = null;
                            isItem = true;
                            StartCoroutine(crossDelete(tempBlock.iX,tempBlock.iY));
                        }
                    }
                }
            } 
            else
            {
                isFirstItemActive = false;
                isSecondItemActive = false;
            }
        } 
        // 마우스 드래그 
        if (Input.GetMouseButton(0) && !isFirstItemActive && !isSecondItemActive)
        {
            MouseEndPos = Input.mousePosition;
            MouseOffset = MouseStartPos - MouseEndPos;

            if (bDrag && SelectBlock != null)
            {
                // 왼쪽
                if (MouseOffset.x > fMouseMoveDis)
                {
                    if (SelectBlock.transform.position.x > 0) MouseDirection(-1, 0);
                }
                // 오른쪽
                if (MouseOffset.x < -fMouseMoveDis)
                {
                    if (SelectBlock.transform.position.x < iBlockX - 1) MouseDirection(1, 0);
                }
                // 위
                if (MouseOffset.y < -fMouseMoveDis)
                {
                    if (SelectBlock.transform.position.y < iBlockY - 1) MouseDirection(0, 1);
                }
                // 아래
                if (MouseOffset.y > fMouseMoveDis)
                {
                    if (SelectBlock.transform.position.y > 0) MouseDirection(0, -1);
                } 
            }
        }
    }
    IEnumerator crossDelete(int iX,int iY)
    {
        // 이펙트  
        for (int x = 0; x < iBlockX; x++)
        {
            GameObject[] blocks = GameObject.FindGameObjectsWithTag("Block");
            foreach (GameObject block in blocks)
            {
                Block sBlock = block.GetComponent<Block>();
                if (sBlock.iX == iX + x && sBlock.iY == iY)
                { 
                    Instantiate(sBlock.deleteEffect[1], sBlock.transform.position, sBlock.transform.rotation);
                    if (iY + x > 6 && iY - x < 0 && iX - x < 0) yield return new WaitForSeconds(0.05f);
                }
                if (sBlock.iX == iX - x && sBlock.iY == iY)
                {
                    Instantiate(sBlock.deleteEffect[1], sBlock.transform.position, sBlock.transform.rotation);
                    if (iY + x > 6 && iY -x <0) yield return new WaitForSeconds(0.05f);
                }
                if (sBlock.iX == iX  && sBlock.iY == iY - x)
                {
                    Instantiate(sBlock.deleteEffect[1], sBlock.transform.position, sBlock.transform.rotation);
                    if(iY+x > 6) yield return new WaitForSeconds(0.05f); 
                }
                if (sBlock.iX == iX && sBlock.iY  == iY + x)
                {
                    Instantiate(sBlock.deleteEffect[1], sBlock.transform.position, sBlock.transform.rotation);
                    yield return new WaitForSeconds(0.05f); 
                } 
            }
        }

        // 폭탄 터트림
        for (int x = 0; x < iBlockX; x++)
        {
            GameObject[] blocks = GameObject.FindGameObjectsWithTag("Block");
            foreach (GameObject block in blocks)
            {
                Block sBlock = block.GetComponent<Block>();
                if (sBlock.iX == x && sBlock.iY == iY)
                {
                    sBlock.transform.DOPunchRotation(new Vector3(0, 120, 0), 0.5f);
                    sBlock.bDead = true;
                    continue;
                }
            }
        } 
        for (int y = 0; y < iBlockY; y++)
        {
            GameObject[] blocks = GameObject.FindGameObjectsWithTag("Block");
            foreach (GameObject block in blocks)
            {
                Block sBlock = block.GetComponent<Block>();
                if (sBlock.iX == iX && sBlock.iY == y)
                {
                    sBlock.transform.DOPunchRotation(new Vector3(0, 120, 0), 0.5f);
                    sBlock.bDead = true;
                    continue;
                }
            }
        }

        //yield return new WaitForSeconds(0.1f);
        //Instantiate(sBlock.deleteEffect[1], sBlock.transform.position, sBlock.transform.rotation);
         

        yield return new WaitForSeconds(0.2f);
        widthCheck = false;
        heightCheck = false;

        SelectBlock = null;
        TargetBlcok = null;
        BlockDelete();
    }
    IEnumerator widthDelete(int iY)
    {
        for (int x = 0; x < iBlockX; x++)
        {
            GameObject[] blocks = GameObject.FindGameObjectsWithTag("Block");
            foreach (GameObject block in blocks)
            {
                Block sBlock = block.GetComponent<Block>();
                if (sBlock.iX == x && sBlock.iY == iY)
                {
                    sBlock.transform.DOPunchRotation(new Vector3(0, 120, 0), 0.5f);
                    sBlock.bDead = true;
                    continue;
                }
            }
        }
        yield return new WaitForSeconds(0.2f);
        widthCheck = false;
        heightCheck = false;

        SelectBlock = null;
        TargetBlcok = null;
        BlockDelete(); 
    }
    IEnumerator heightDelete(int iX)
    {
        for (int y = 0; y < iBlockY; y++)
        {
            GameObject[] blocks = GameObject.FindGameObjectsWithTag("Block");
            foreach (GameObject block in blocks)
            {
                Block sBlock = block.GetComponent<Block>();
                if (sBlock.iX == iX && sBlock.iY == y)
                {
                    sBlock.transform.DOPunchRotation(new Vector3(0, 120, 0), 0.5f);
                    sBlock.bDead = true;
                    continue;
                }
            }
        }
        yield return new WaitForSeconds(0.2f);
        widthCheck = false;
        heightCheck = false;

        SelectBlock = null;
        TargetBlcok = null;
        BlockDelete(); 
    }
    void MouseDirection(float _x, float _y)
    {
        //MoveCount--;
        EndPos1 = new Vector3(StartPos1.x + _x, StartPos1.y + _y, 0);
        bDrag = false;
        GameObject[] blocks = GameObject.FindGameObjectsWithTag("Block");

        foreach (GameObject block in blocks)
        {
            Block sBlock = block.GetComponent<Block>();
            if (sBlock.iX == EndPos1.x && sBlock.iY == EndPos1.y) TargetBlcok = block;
        }
        StartPos2 = EndPos1;
        EndPos2 = StartPos1;
        bBlockChange = true;
    }

    void DragToMoveBlock()
    {
        // 교환 플래그가 활성화 중이 아니라면
        if (!bBlockChange || TargetBlcok == null) return;

        if(TargetBlcok.transform.position != StartPos1)
        {
            fBlockMoveStep += fMoveSpeed * Time.deltaTime;
            SelectBlock.transform.position = Vector3.MoveTowards(StartPos1, EndPos1, fBlockMoveStep);
            TargetBlcok.transform.position = Vector3.MoveTowards(StartPos2, EndPos2, fBlockMoveStep);
        }
        else
        {
            bBlockChange = false;
            fBlockMoveStep = 0;
            SwitchBoard();
            StartCoroutine(BlockCheck());
        }
    }

    void SwitchBoard()
    {
        Block sBlock = SelectBlock.GetComponent<Block>();
        sBlock.iX = (int)EndPos1.x;
        sBlock.iY = (int)EndPos1.y;

        sBlock = TargetBlcok.GetComponent<Block>();

        sBlock.iX = (int)EndPos2.x;
        sBlock.iY = (int)EndPos2.y;

        int Tmptype = BlockBoard[(int)StartPos1.x][(int)StartPos1.y];
        BlockBoard[(int)StartPos1.x][(int)StartPos1.y] = BlockBoard[(int)EndPos1.x][(int)EndPos1.y];
        BlockBoard[(int)EndPos1.x][(int)EndPos1.y] = Tmptype;
    }

    void ReChangeBlock()
    {
        if(bBlockReChange && TargetBlcok)
        {
            if(TargetBlcok.transform.position != EndPos2)
            {
                fBlockMoveStep += fMoveSpeed * Time.deltaTime;
                SelectBlock.transform.position = Vector3.MoveTowards(StartPos1, EndPos1, fBlockMoveStep);
                TargetBlcok.transform.position = Vector3.MoveTowards(StartPos2, EndPos2, fBlockMoveStep);
            }
            else
            {
                fBlockMoveStep = 0;
                bBlockReChange = false;

                SwitchBoard();
                SelectBlock = null;
                TargetBlcok = null;
                bDrag = true;
            }
        }
    }
   
    // 블럭생성
    void CreateBlcok()
    {
        for (int y = 0; y < iBlockY; y++)
        {
            for (int x = 0; x < iBlockX; x++)
            { 
                GameObject block = Instantiate(OriginBlock, new Vector3(x, y, 0), Quaternion.identity);
                block.transform.SetParent(transform);

                //int iType = Random.Range(0, BlockType.Length );
                int iType = UnityEngine.Random.Range(0, BlockNum);
                
                Block sBlock = block.GetComponent<Block>();
                sBlock.iX = x;
                sBlock.iY = y;
                sBlock.iType = iType;
                sBlock.SetBlockImg(BlockType[iType]);
                BlockBoard[x][y] = iType;
            }
        }
        StartBlockCheck();
        //StartCoroutine(BlockCheck());
    }
    // 시작시 블럭 안터지도록 
    void StartBlockCheck()
    {
        //while (bReset)
        {
            bReset = false;
            // 가로 체크
            for (int y = 0; y < iBlockY; y++)
            {
                for (int x = 0; x < iBlockX - 2; x++)
                {
                    if (BlockBoard[x][y] == BlockBoard[x + 1][y])
                    {
                        if (BlockBoard[x][y] == BlockBoard[x + 2][y])
                        { 
                            bReset = true;
                        }
                    }
                }
            } 
            // 세로체크 
            for (int x = 0; x < iBlockX; x++)
            {
                for (int y = 0; y < iBlockY - 2; y++)
                {
                    if (BlockBoard[x][y] == BlockBoard[x][y + 1])
                    {
                        if (BlockBoard[x][y] == BlockBoard[x][y + 2])
                        { 
                            bReset = true;
                        }
                    }
                }
            }
        }
        if(bReset) RestartGame();
    }

    IEnumerator BlockCheck()
    {
        // 가로 체크
        for(int y = 0; y<iBlockY; y++)
        {
            for(int x = 0; x<iBlockX - 2; x++)
            {
                if(BlockBoard[x][y] == BlockBoard[x +1][y])
                {
                    if(BlockBoard[x][y] == BlockBoard[x+2][y])
                    {
                        GameObject[] blocks = GameObject.FindGameObjectsWithTag("Block");
                        foreach (GameObject block in blocks)
                        { 
                            Block sBlock = block.GetComponent<Block>();
                            if (sBlock.iY != y) continue;

                            if(sBlock.iX == x && sBlock.iY == y)
                            {
                                sBlock.transform.DOPunchRotation(new Vector3(0,120 , 0), 0.5f);                     
                                sBlock.bDead = true;
                                continue;
                            }

                            if (sBlock.iX == x +1 && sBlock.iY == y)
                            { 
                                sBlock.transform.DOPunchRotation(new Vector3(0, 120, 0), 0.5f);
                                sBlock.bDead = true;
                                continue;
                            }

                            if (sBlock.iX == x + 2 && sBlock.iY == y)
                            {
                                sBlock.transform.DOPunchRotation(new Vector3(0, 120, 0), 0.5f);
                                sBlock.bDead = true; 
                            }
                        }
                    }
                }
            }
        }

        // 세로체크

        for (int x = 0; x < iBlockX; x++)
        {
            for (int y = 0; y < iBlockY - 2; y++)
            {
                if (BlockBoard[x][y] == BlockBoard[x][y +1])
                {
                    if (BlockBoard[x][y] == BlockBoard[x][y+2])
                    {
                        GameObject[] blocks = GameObject.FindGameObjectsWithTag("Block");
                        foreach (GameObject block in blocks)
                        {
                            Block sBlock = block.GetComponent<Block>();
                            if (sBlock.iX != x) continue;

                            if (sBlock.iX == x && sBlock.iY == y)
                            { 
                                sBlock.transform.DOPunchRotation(new Vector3(0, 120, 0), 0.5f);
                                sBlock.bDead = true;
                                continue;
                            }

                            if (sBlock.iX == x && sBlock.iY == y + 1)
                            {
                                sBlock.transform.DOPunchRotation(new Vector3(0, 120, 0), 0.5f);
                                sBlock.bDead = true;
                                continue;
                            }

                            if (sBlock.iX == x && sBlock.iY == y + 2)
                            {
                                sBlock.transform.DOPunchRotation(new Vector3(0, 120, 0), 0.5f);
                                sBlock.bDead = true; 
                            }
                        }
                    }
                }
            }
        }
        if(!NoMatch())
        {  
            if(bReCheck)
            {
                bReCheck = false;
                bDrag = true;
            }
            else
            {
                Vector3 TmpStartPos = StartPos1;
                StartPos1 = StartPos2;
                StartPos2 = TmpStartPos;

                Vector3 TmpEndPos = EndPos1;
                EndPos1 = EndPos2;
                EndPos2 = TmpEndPos;

                bBlockReChange = true;
            }
        }
        else
        {
            yield return new WaitForSeconds(0.2f); 
            widthCheck = false;
            heightCheck = false;

            SelectBlock = null;
            TargetBlcok = null;
            BlockDelete();
        }
    }

    // 블럭중 매치된 블럭이 있는지
    bool NoMatch()
    {
        GameObject[] blocks = GameObject.FindGameObjectsWithTag("Block");

        foreach(GameObject block in blocks)
        {
            if (block.GetComponent<Block>().bDead) return true;
        }
        return false;
    }

    // 매치된 블럭 삭제
    void BlockDelete()
    {
        bool bMatch = false;                        // 블럭 삭제 체크 
        GameObject[] blocks = GameObject.FindGameObjectsWithTag("Block");
        foreach(GameObject block in blocks)
        {
            Block sBlock = block.GetComponent<Block>();
            if(sBlock.bDead)
            {
                MyScore += 100; 
                if (MyScore > MaxScore) MaxScore = MyScore;
                audioSource.PlayOneShot(sBlock.DeleteAudioClip); 
                if(isItem)  Instantiate(sBlock.deleteEffect[1], sBlock.transform.position, sBlock.transform.rotation);
                else Instantiate(sBlock.deleteEffect[0], sBlock.transform.position, sBlock.transform.rotation);
                // 삭제된 블럭 칸을 -1로 
                BlockBoard[sBlock.iX][sBlock.iY] = (int)BLOCK.BLANK;
                Destroy(block);
                bMatch = true;
            }
        } 
        if (bMatch) BlockDown();
    }
    void BlockDown()
    {
        restartTime = 0;
        isFirstBlockCreate = true;
        bool bBlockDown = false;
        for(int x = 0; x<iBlockX; x++)
        {
            bool BlankCheck = false;
            for(int y = iBlockY -1; y >-1; y--)
            {
                if(!BlankCheck && BlockBoard[x][y] == (int)BLOCK.BLANK)
                {
                    BlankCheck = true;
                    bBlockDown = true;

                    GameObject block = Instantiate(OriginBlock, new Vector3(x, iBlockY, 0), Quaternion.identity);

                    //int iType = Random.Range(0, BlockType.Length );
                    int iType = UnityEngine.Random.Range(0, BlockNum);  
                    Block sBlock = block.GetComponent<Block>();
                    sBlock.iX = x;
                    sBlock.iY = iBlockY;
                    //sBlock.iType = iType;
                    //sBlock.SetBlockImg(BlockType[iType]);

                    GameObject[] tempBlocks = GameObject.FindGameObjectsWithTag("Block");

                    // 블럭 4개 이상  체크
                    foreach (GameObject block2 in tempBlocks)
                    {
                        sBlock = block2.GetComponent<Block>(); 
                        if (sBlock.bDead) blockCount++;
                    }

                    if ((blockCount == 4 || blockCount == 7 ) && isFirstBlockCreate && !isItem)
                    {
                        isFirstBlockCreate = false;
                        if(x < iBlockX - 3 && BlockBoard[x][y] == BlockBoard[x+1][y] && BlockBoard[x][y] == BlockBoard[x + 2][y] && BlockBoard[x][y] == BlockBoard[x + 3][y])
                            iType = 9;
                        else iType = 10;
                    }
                    if ((blockCount == 5) && isFirstBlockCreate && !isItem)
                    {
                        isFirstBlockCreate = false;
                        iType = 11;
                    }
                    blockCount = 0;

                    ///////////////////////////////////////////// 
                    sBlock.iType = iType;
                    sBlock.SetBlockImg(BlockType[iType]);
                    for (int z = y+1; z<iBlockY +1; z++)
                    {
                        GameObject[] blocks = GameObject.FindGameObjectsWithTag("Block");

                        foreach(GameObject block2 in blocks)
                        {
                            sBlock = block2.GetComponent<Block>();

                            if(sBlock.iX == x && sBlock.iY == z)
                            {
                                sBlock.StartPos = new Vector3(x, z, 0);
                                sBlock.TartgetPos = new Vector3(x, z -1, 0);  
                                sBlock.bDown = true;
                                break;
                            }
                        }
                    } 
                }
            }
        }
        isItem = false;
        if (bBlockDown) bBlockMoveDown = true;
        else
        {
            bReCheck = true;
            StartCoroutine(BlockCheck());
        }
    }
    public void FirstItemClick()
    {
        if(MyFirstItemNum > 0 && isStart)    isFirstItemActive = true;
    }
    IEnumerator FirstItemActive(int blockType)
    { 
        GameObject[] blocks = GameObject.FindGameObjectsWithTag("Block");
        foreach (GameObject block in blocks)
        {
            Block sBlock = block.GetComponent<Block>(); 
            if(sBlock.iType == blockType)
            {
                sBlock.transform.DOPunchRotation(new Vector3(0, 120, 0), 0.5f);
                sBlock.bDead = true;
                continue;
            }
        } 
        yield return new WaitForSeconds(0.2f);
        widthCheck = false;
        heightCheck = false;

        SelectBlock = null;
        TargetBlcok = null;
        isFirstItemActive = false;
        BlockDelete();
    }

    public void SecondItemClick()
    {
        if (MySecondItemNum > 0 && isStart) isSecondItemActive = true;
    }
    IEnumerator SecondItemActive(int x,int y)
    {
        for (int i = x-1; i < x+2; i++)
        {
            for (int j = y-1; j < y+2; j++)
            {
                GameObject[] blocks = GameObject.FindGameObjectsWithTag("Block");
                foreach (GameObject block in blocks)
                {
                    Block sBlock = block.GetComponent<Block>();  
                    if(sBlock.iX == i && sBlock.iY == j)
                    {
                        sBlock.transform.DOPunchRotation(new Vector3(0, 120, 0), 0.5f);
                        sBlock.bDead = true;
                        continue;
                    }
                }
            }
        }
        yield return new WaitForSeconds(0.2f);
        widthCheck = false;
        heightCheck = false;

        SelectBlock = null;
        TargetBlcok = null;
        isSecondItemActive = false;
        BlockDelete();
    }
}
