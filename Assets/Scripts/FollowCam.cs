using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    public Transform m_trmTaget = null;
    public float smoothSpeed = 0.1f;
    public bool FixHieght = true;

    private Vector3 GapPos = Vector3.zero;
    
    //============================================================================================
    // FindData
    // -
    //============================================================================================
    void Start()
    {
        GapPos = transform.position - m_trmTaget.position;
    }

    //============================================================================================
    // FindData
    // -
    //============================================================================================
    void Update()
    {
        Vector3 desiredPosition = m_trmTaget.position + GapPos;

        if (FixHieght)
            desiredPosition.y = transform.position.y;

        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
        // transform.LookAt(target);
    }
}
