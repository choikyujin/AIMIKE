using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorMatchingChar : MonoBehaviour
{
    public ColorMatchingManager.ColorKind colorKind;
    ColorMatchingManager cm;
    public CanvasGroup canvasGroup;
    public Image charImage;
    public void Init(ColorMatchingManager colorMatchingManager, ColorMatchingManager.ColorKind colorKind)
    {
        gameObject.SetActive(true);
        AnimReset();
        cm = colorMatchingManager;
        this.colorKind = colorKind;
        MM.m("(int)colorKind", (int)colorKind);
        charImage.sprite = cm.charSprites[(int)colorKind];
        transform.position = cm.charStartPosTr.position;
        transform.localScale = new Vector3(0.4f, 0.4f, 1);
        canvasGroup.alpha = 1;
        Move();
    }
    int leatid1;
    int leatid2;
    int leatid3;
    int leatid4;
    void Move()
    {
        float animTime = 5;
        leatid1 = LeanTween.moveY(gameObject, cm.charEndPosTr.position.y, animTime).id;
        leatid2 = LeanTween.scale(gameObject, Vector3.one, animTime).id;
        Lean.Pool.LeanPool.Despawn(this, animTime);
    }
    void AnimReset()
    {
        LeanTween.cancel(leatid1);
        LeanTween.cancel(leatid2);
        LeanTween.cancel(leatid3);
        LeanTween.cancel(leatid4);
    }
    //빨간선위에서 클릭 성공하면
    public void SuccessClick()
    {
        AnimReset();
        float animTime = 2;
        leatid3 = LeanTween.alphaCanvas(canvasGroup, 0, animTime).id;
        leatid4 = LeanTween.moveY(gameObject, gameObject.transform.position.y + 100, animTime).id;
        Lean.Pool.LeanPool.Despawn(this, animTime);
        charImage.sprite = cm.charSucceSprites[(int)colorKind];
    }
    //public void FailClick()
    //{
    //    AnimReset();
    //}
}
