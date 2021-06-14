//using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;


public class No120Window : MonoBehaviour
{
    //UIMiniGameState_No120 uIMiniGameState_No120;
    public GameObject itemObj; //풍선 프리팹
    public GameObject itemBoombObj; //풍선 프리팹
    public GameObject itemBoombObj2; //풍선 프리팹
    public GameObject itemBoombObj3; //풍선 프리팹
    public Transform itemParent; //풍선생성후 아빠설정
    public Transform boombParent; //풍선생성후 아빠설정
    public Transform arriveLeftTr;
    public Transform arriveRightTr;
    public Transform startPosTr;
    public Transform startEndTr;
    public GameObject lineObj;
    public Transform lineParent;
    public GameObject scoreCoinImage;
    public GameObject carObj;
    public int score;
    public Text scoreText;
    int maxScore = 10;
    public void AddScore(int addScore)
    {
        //MM.m("AddScore", score);
        score += addScore;
        if (score < 0) score = 0;
       //++score;
        scoreText.text = score.ToString() ;
        if(score >= maxScore)
        {
            //랭크 팝업 띄우기
            //uIMiniGameState_No120.StageClear();
        }
    }
    //public void Init(UIMiniGameState_No120 uIMiniGameState_No120)
    //{
    //    this.uIMiniGameState_No120 = uIMiniGameState_No120;

    //}

    private void OnEnable()
    {
        itemObj.SetActive(false);
        lineObj.SetActive(false);

        StartCoroutine(SetInstantiateMakeLineCo());
        StartCoroutine(SetInstantiateCoin());
    }
    private void Start()
    {
        scoreText.text = 0.ToString();
        MiniGameTimeSlider.it.EndTimeAction += GameEnd;
        MiniGameTimeSlider.it.StartTime();
    }
    void GameEnd()
    {
        AudioSourceManager.it.PlaySound(AudioClipManager.it.gameEndVoices[0]);
        LeanTween.cancelAll();
        MiniGameRankPopup.instance.Active("Slider", score);
        gameObject.SetActive(false);
    }
    float movetime = 3.2f;
    void MakeLine()
    {
        GameObject item = Lean.Pool.LeanPool.Spawn(lineObj, lineParent);

        item.transform.position = startPosTr.position;
        
        LeanTween.move(item.gameObject, startEndTr.transform.position, movetime).setEase(LeanTweenType.easeInQuad);
        Lean.Pool.LeanPool.Despawn(item.gameObject, movetime);
        //LeanTween.delayedCall(movetime, () => {
        //    Destroy(item.gameObject);
        //});
        item.transform.localScale = new Vector3(.036f,.036f,1);
        LeanTween.scale(item, Vector3.one, movetime).setEase(LeanTweenType.easeInQuad);

        item.SetActive(true);
        //int index = Random.Range(0, chaicSprites.Count);
    }
    public IEnumerator SetInstantiateMakeLineCo()
    {
        yield return new WaitForSeconds(0.1f);
        while (true)
        {
            yield return new WaitForSeconds(1);
            MakeLine();
        }


    }
    public IEnumerator SetInstantiateCoin()
    {
        yield return new WaitForSeconds(0.1f);
        while (true)
        {
            float delayR = UnityEngine.Random.Range(0.1f, 0.5f);
            yield return new WaitForSeconds(delayR);
            MakeItem(0);
        }


    }
    void MakeItem(float addPosY)
    {
        int r =  UnityEngine.Random.Range(0, 5);
        GameObject _item = itemObj;
        MM.m("item", itemBoombObj, itemParent, itemObj);

        if (r == 0)
        {
            _item = itemBoombObj == null ? itemBoombObj2 : itemBoombObj;
            _item = _item == null? itemBoombObj3 : _item;
            _item = _item == null ? itemObj : _item;
        }
        GameObject item = Lean.Pool.LeanPool.Spawn(_item, itemParent);
        item.SetActive(true);
        //int index = Random.Range(0, chaicSprites.Count);
        item.GetComponent<No120Item>().SetMove(this, movetime);
    }
    private void Update()
    {
        
    }
}
