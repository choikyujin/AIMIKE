using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class No120Car : MonoBehaviour
{
    public GameObject parentObj;
    
    public void MessageMoveLeft()
    {
        //KeyCode.F11
        if (parentObj.transform.eulerAngles.z < 30 + 18 || parentObj.transform.eulerAngles.z > 328)
        {
            parentObj.transform.Rotate(0, 0, -0.5f);
        }
       
       
    }
    public void MessageMoveRight()
    {
        if (parentObj.transform.eulerAngles.z < 30 || parentObj.transform.eulerAngles.z > 320 - 18)
        {
            parentObj.transform.Rotate(0, 0, 0.5f);
        }
    }
    IEnumerator PushKey(bool isLeft)
    {
        while (true)
        {
            if (isLeft)
            {
                MessageMoveLeft();
            }
            else
            {
                MessageMoveRight();
            }
            yield return null;
        }
    }
    Coroutine pushKeyco;
    void PushKeyCo(bool isLeft)
    {
        if(pushKeyco != null)
        {
            StopCoroutine(pushKeyco);
        }
        //SFXManager.Instance.PlaySFX(1021);
        pushKeyco = StartCoroutine(PushKey(isLeft));
    }
    private void Update()
    {

        //#if UNITY_EDITOR
        if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.LeftArrow))
            PushKeyCo(true);
        if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.RightArrow))
            PushKeyCo(false);
        //#endif
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            MM.m("LDKSJFLKSDJFDLKS");
            collision.GetComponent<No120Item>().SetAddScore();
        }else if(collision.tag == "SlidingBoomb")
        {
            collision.GetComponent<No120Item>().SetBoomb();
         

        }
    }
   

}
