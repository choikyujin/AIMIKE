using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class No120Item : MonoBehaviour
{
    No120Window no120Window;
    public void SetBoomb()
    {
        LeanAnimReset();
        AudioSourceManager.it.PlaySound(AudioClipManager.it.receiveBoomb);
        float duration = 1;
        leanId6 = LeanTween.alphaCanvas(canvasGroup, 0, duration).setEase(LeanTweenType.easeInQuad).id;
        leanId7 = LeanTween.scale(gameObject, new Vector3(2, 2, 2), duration).setEase(LeanTweenType.easeOutQuart).id;
        transform.SetParent(no120Window.boombParent);
        //transform.SetAsLastSibling();
        Lean.Pool.LeanPool.Despawn(gameObject, duration);
        no120Window.AddScore(-5);
    }
    public void SetAddScore()
    {
        AudioSourceManager.it.PlaySound(AudioClipManager.it.receiveCoin);

        LeanAnimReset();
        //AudioSourceManager.it.PlaySound(AudioClipManager.it.clickOk) ;
        leanId4 = LeanTween.moveX(gameObject, no120Window.scoreCoinImage.transform.position.x, 1).setEase(LeanTweenType.easeInQuad).id;
        leanId5 = LeanTween.moveY(gameObject, no120Window.scoreCoinImage.transform.position.y, 1).setEase(LeanTweenType.easeOutQuad).id;
        //LeanTween.move(gameObject, no120Window.scoreCoinImage.transform.position, 1).setEase(LeanTweenType.easeOutQuad);
        float duration = 1;
        leanId3 = LeanTween.scale(gameObject, new Vector3(0.35f, 0.35f, 1), duration).setOnComplete(()=> {
            no120Window.AddScore(1);
        }).id;
        Lean.Pool.LeanPool.Despawn(gameObject, duration);
    }
    //private void OnEnable()
    //{
    //    //StartCoroutine(CheckItemCo());
    //}
    //IEnumerator CheckItemCo()
    //{
    //    while (true)
    //    {
    //        if(no120Window == null)
    //        {
    //            yield return null;
    //            continue;
    //        }
    //        //MM.m("no120Window", no120Window);
    //        float distance = Vector2.Distance(no120Window.carObj.transform.position, transform.position);

    //        if (distance < 20f)
    //        {
    //            MM.m("sdfd ", distance, no120Window.carObj.transform.position, transform.position);

    //            LeanTween.cancel(gameObject);
    //            SetAddScore();
    //            break;
    //        }
    //        yield return null;
    //    }
    //}
    void LeanAnimReset()
    {
        LeanTween.cancel(leanId1);
        LeanTween.cancel(leanId2);
        LeanTween.cancel(leanId3);
        LeanTween.cancel(leanId4);
        LeanTween.cancel(leanId5);
        LeanTween.cancel(leanId6);
        LeanTween.cancel(leanId7);
    }
    int leanId1;
    int leanId2;
    int leanId3;
    int leanId4;
    int leanId5;
    int leanId6;
    int leanId7;
    CanvasGroup canvasGroup;
    public void SetMove(No120Window no120Window, float moveTime)
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if(canvasGroup != null) canvasGroup.alpha = 1;
        LeanAnimReset();
        this.no120Window = no120Window;
        transform.position = no120Window.startPosTr.position;
        transform.localScale = new Vector3(.036f, .036f, 1); ;
        float arriveX = Random.Range(no120Window.arriveLeftTr.position.x, no120Window.arriveRightTr.position.x);
        Lean.Pool.LeanPool.Despawn(gameObject, moveTime);
        leanId1 = LeanTween.move(gameObject, new Vector3(arriveX, no120Window.startEndTr.position.y, 0), moveTime).setEase(LeanTweenType.easeInQuad).id;
        leanId2 = LeanTween.scale(gameObject, Vector3.one, moveTime).setEase(LeanTweenType.easeInQuad).id;
        //no120Window.
        //float 
        //this.index = index;
        //this.no109Window = no109Window;
        //button.interactable = true;
        //canvasGroup.alpha = 1;
        //chirImage.sprite = no109Window.chaicSprites[index];

        //StartCoroutine(MoveCo(addYPos));
    }
}
