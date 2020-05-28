using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public GameManager gMar;            // 게임 매니저
    public SpriteRenderer MyBlockImg;   // 현재 블럭의 이미지

    public int iX, iY;      // 현재 블럭의 위치.
    public int iType;       // 블럭의 종류

    public bool bDown;      // 현재 블럭이 떨어지고 있는지
    public bool bDead;      // 현재 블럭이 사라져야 하는지

    private float fStep = 0;
    private float fMoveSpeed = 10f;

    public Vector3 StartPos;
    public Vector3 TartgetPos;
    
    public GameObject[] deleteEffect;
     
    public AudioClip DeleteAudioClip;

    private void Awake()
    {
        MyBlockImg = GetComponent<SpriteRenderer>();
        gMar = GameObject.Find("GameManager").GetComponent<GameManager>(); 
    }
    private void Update()
    {
        if(bDown && transform.position != TartgetPos)
        {
            fStep += Time.deltaTime * fMoveSpeed;
            transform.position = Vector3.MoveTowards(StartPos, TartgetPos, fStep);

            if(transform.position == TartgetPos)
            {
                int X = (int)TartgetPos.x;
                int Y = (int)TartgetPos.y;

                gMar.BlockBoard[X][Y] = iType;
                iX = X;
                iY = Y;
                fStep = 0;
                bDown = false;
            }
        }
    }
    public void SetBlockImg(Sprite _sprite) { MyBlockImg.sprite = _sprite; }
}
