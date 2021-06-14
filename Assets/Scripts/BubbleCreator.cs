using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleCreator : MonoBehaviour
{
    public GameObject[] m_pBubbleList = new GameObject[4];

    float m_PosY = 1200f;
    float[] m_PosX = new float[] { -380f, -130f, 130f, 380f };
    float m_fDuration = 0f;
    float m_fTermATime = 0.1f;
    float m_fSpeed = 300f;
    ArrayList m_EffectList = new ArrayList();

    //============================================================================================
    // Update  
    // -
    //============================================================================================
    void Update()
    {
        if (m_fDuration > 0f)
        {
            m_fDuration -= Time.unscaledDeltaTime;
            if (m_fDuration <= 0f)
                m_fDuration = 0f;

            CreateRandom();
        }        
    }

    //============================================================================================
    // SetStart  
    // -
    //============================================================================================
    public void SetStart(float fDuration)
    {
        m_fDuration = fDuration;

        for (int i = 0; i < m_EffectList.Count; i++)
        {
            if (m_EffectList[i] != null)
                Destroy((GameObject)m_EffectList[i]);
        }
        m_EffectList.Clear();
    }

    //============================================================================================
    // SetStop
    // -
    //============================================================================================
    public void SetStop()
    {
        m_fDuration = 0;

        for(int i=0; i<m_EffectList.Count; i++)
        {
            if(m_EffectList[i] != null)
                Destroy((GameObject)m_EffectList[i]);
        }
        m_EffectList.Clear();
    }


    //============================================================================================
    // CreateRandom  
    // -
    //============================================================================================
    void CreateRandom()
    {
        if (m_fTermATime > 0f)
        {
            m_fTermATime -= Time.unscaledDeltaTime;
            if (m_fTermATime <= 0f)
            {
                int Type = Random.Range(0, 4);
                m_fTermATime = 1.5f + Random.Range(-0.2f, 0.2f);
                GameObject obj = GameObject.Instantiate(m_pBubbleList[Type]);
                obj.transform.parent = transform;
                obj.transform.localPosition = new Vector3(m_PosX[Random.Range(0, 4)], m_PosY, 0f);
                obj.transform.localScale = Vector3.one;                
                obj.GetComponent<DropBubble>().SetSpeed(m_fSpeed);
                obj.name = "Bubble_" + Type;
                obj.SetActive(true);
                m_EffectList.Add(obj);
            }
        }

    }
}
