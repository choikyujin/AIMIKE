using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayOff : MonoBehaviour
{
    public bool m_bDestroy = false;
    public float m_fDelayTime = 1f;
    float m_fDefault = 0f;

    // Start is called before the first frame update
    void Start()
    {
        m_fDefault = m_fDelayTime;
    }

    // Update is called once per frame
    void Update()
    {
        m_fDelayTime -= Time.unscaledDeltaTime;
        if(m_fDelayTime <= 0f)
        {
            if (m_bDestroy)
            {
                Destroy(gameObject);
            }
            else
            {
                m_fDelayTime = m_fDefault;
                gameObject.SetActive(false);
            }
        }
    }
}
