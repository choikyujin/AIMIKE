using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorMatchingManager : MonoBehaviour
{
    public enum ColorKind
    {
        blue, yellow, green
    }
    [Header("blue, yellow, green")]
    public Sprite[] charSprites;
    public Sprite[] charSucceSprites;
    public Sprite[] charFailSprites;
    public ColorMatchingChar itemObj; //풍선 프리팹
    public Transform itemObjParent; //풍선  아빠
    public Transform charStartPosTr;
    public Transform charEndPosTr;
    public Transform goalLine;
    public Text scoreText;
    public GameObject goodObj;
    private void Awake()
    {
        StartCoroutine(SpawnCo());
        goodObj.SetActive(false);
        scoreText.text = 0.ToString();
    }
    private void Start()
    {
        MiniGameTimeSlider.it.EndTimeAction += GameEnd;
        MiniGameTimeSlider.it.StartTime();
    }
    List<ColorMatchingChar> itemObjs = new List<ColorMatchingChar>();
    IEnumerator SpawnCo()
    {
        while (true)
        {
            float delayR = Random.Range(0.5f, 2);
            yield return new WaitForSeconds(delayR);
            ColorMatchingChar a = Lean.Pool.LeanPool.Spawn(itemObj, itemObjParent);
            int colorKindR = Random.Range(0, 3);
            a.Init(this, (ColorKind)colorKindR);
            a.transform.SetAsFirstSibling();
            itemObjs.Add(a);
        }
    }
    public int score;
    float goodDis = 80f; //클릭했을때 성공으로 간주되는 캐릭터 위치
    public void BtnClick(int index)
    {
        AudioSourceManager.it.PlaySound(AudioClipManager.it.clickOk);
        for (int j = itemObjs.Count-1; j >= 0; --j)
        {
            float dis = goalLine.position.y - itemObjs[j].transform.position.y;
           
            if (itemObjs[j].colorKind != (ColorKind)index) continue;

            if(Mathf.Abs(dis) < goodDis)
            {
                itemObjs[j].SuccessClick();
                ++score;
                MM.m("itemObjs[j].colorKind", (int)itemObjs[j].colorKind, index, goalLine.position.y, itemObjs[j].transform.position.y, dis, score);
                scoreText.text = score.ToString();
                itemObjs.Remove(itemObjs[j]);
                ShowGood();
            }
        }
       
    }
    void ResetAnim()
    {
        LeanTween.cancel(leanId1);
    }
   int leanId1;
    void ShowGood()
    {
        AudioSourceManager.it.PlaySound(AudioClipManager.it.good);

        ResetAnim();
        goodObj.SetActive(true);
        leanId1 = LeanTween.scale(goodObj, new Vector3(1.2f, 1.2f, 1), 0.14f).setEase(LeanTweenType.easeOutQuad).setLoopPingPong(1).setOnComplete(()=> {
            goodObj.SetActive(false);
        }).id;

    }
    void GameEnd()
    {
        AudioSourceManager.it.PlaySound(AudioClipManager.it.gameEndVoices[0]);
        LeanTween.cancelAll();
        MiniGameRankPopup.instance.Active("ColorMatch", score);
        gameObject.SetActive(false);
    }

}
