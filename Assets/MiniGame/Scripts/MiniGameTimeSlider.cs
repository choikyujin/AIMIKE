using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniGameTimeSlider : MonoBehaviour
{
    public Slider remindTimeSlider;
    public GameObject remindTimeWarnningGameObject; //남은시간 경고
    public Text turnTimeText;
    Coroutine timeTextInActiveCo;
    public static MiniGameTimeSlider it;
    private void Awake()
    {
        MM.m("MiniGameTimeSlider2");
        it = this;




    }
    public void StartTime()
    {
        StartCoroutine(TimeCo());
    }
    public event Action EndTimeAction;
    int time;
    IEnumerator TimeCo()
    {
       
        yield return null;
        //yield return new WaitForSeconds(5);

        //MiniGameRankPopup.instance.Active("Slider" + 1, 3);

        int maxTime = 30;
        while (true)
        {
            MM.m("MiniGameTimeSlider1", MiniGameTimeSlider.it);
            TurnTime(time, maxTime);
            yield return new WaitForSeconds(1);

            ++time;

            if (time >= maxTime)
            {
                EndTimeAction();
                break;
                //MM.OpenScale(MiniGameRankPopup.instance.gameObject);
            }
        }
    }
    public void TurnTime(int turnTime, int maxTime)
    {
        //MM.m("df", (float)turnTime / (float)maxTime, 1 - ((float)turnTime / (float)maxTime), turnTime, maxTime);
        remindTimeSlider.value = 1 - ((float)turnTime / (float)maxTime);

        int remindTurnTime = maxTime - turnTime;
        if (remindTurnTime == 5)
        {
            AudioSourceManager.it.PlaySound(AudioClipManager.it.hurryupVoices[(int)0]);

        }
        if (remindTurnTime < 6)
        {
            remindTimeWarnningGameObject.SetActive(true);
            AudioSourceManager.it.PlaySound(AudioClipManager.it.clockRun);
        }
       
        turnTimeText.gameObject.SetActive(true);
        turnTimeText.transform.localScale = Vector3.one;
        LeanTween.scale(turnTimeText.gameObject, new Vector3(1.1f, 1.1f, 1), 0.12f).setEase(LeanTweenType.easeOutQuad).setLoopPingPong(1);
        //turnTimeText.text = $"<color=green>{turnTime}</color>";
        turnTimeText.text = $"{remindTurnTime}";
        if (timeTextInActiveCo != null)
        {
            StopCoroutine(timeTextInActiveCo);
        }
        timeTextInActiveCo = StartCoroutine(TimeTextInActiveCo());
    }
    IEnumerator TimeTextInActiveCo()
    {
        yield return new WaitForSeconds(1);
        turnTimeText.gameObject.SetActive(false);
    }
}
