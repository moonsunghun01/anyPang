using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundController : MonoBehaviour
{
    public Transform[] m_tfBackground = null;
    public float m_speed = 0;

    public float m_leftPosX = 0;
    public float m_rightPosX = 0;

    float viewWidth;
    public int tempNum = 1;
    void Start()
    {
        float t_length = m_tfBackground[0].GetComponent<SpriteRenderer>().sprite.bounds.size.x;
        m_leftPosX = -t_length;
        m_rightPosX = t_length * m_tfBackground.Length;
        if (tempNum == 2) m_rightPosX = 15;
    }

    // Update is called once per frame
    void Update()
    {
       for (int i =0;i<m_tfBackground.Length; i++)
        {
            m_tfBackground[i].position += new Vector3(m_speed, 0, 0) * Time.deltaTime;

            if(m_tfBackground[i].position.x <m_leftPosX)
            {
                Vector3 t_selfPos = m_tfBackground[i].position;
                t_selfPos.Set(t_selfPos.x + m_rightPosX, t_selfPos.y , t_selfPos.z);
                m_tfBackground[i].position = t_selfPos;
            }
        }
    }
}
