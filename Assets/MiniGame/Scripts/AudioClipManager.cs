using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AudioClipManager : MonoBehaviour
{
    public static AudioClipManager it;
    void Start()
    {
        
        it = this;
        
    }
    //[Header("shot!")]public AudioClip shotClip;
    //[Header("Boom!")] public AudioClip boomClip;
    //[Header("영혼빠져나가는 소리")] public AudioClip goustClip;
    //[Header("코인떨구는 소리")] public AudioClip ingameDropGold;
    //[Header("코인터뜨리는 소리")] public AudioClip ingameDropExplosionGold;
    //[Header("heal이펙트 작은 소리")] public AudioClip healEffectSmallEffectClip;
    //[Header("감염이펙트 소리")] public AudioClip splateEffectClip;
    //[Header("일반적인 버튼 소리")] public AudioClip ButtonSound;
    [Header("취소버튼소리")] public AudioClip clickCancel;
    [Header("ok버튼 소리")] public AudioClip clickOk;
    [Header("보잉 소리")] public AudioClip boing;
    [Header("시계움직이는 소리")] public AudioClip clockRun;
    [Header("게임 졌을때 소리")] public AudioClip[] gameLose;
    [Header("게임 이겼을때 소리")] public AudioClip[] gameWin;
    [Header("턴 졌을때 소리")] public AudioClip[] turnLose;
    [Header("턴  이겼을때 소리")] public AudioClip[] turnWin;
    [Header("win!")] public AudioClip gameWinVoice;
    [Header("lose!")] public AudioClip gameLoseVoice;
    [Header("던지는 소리")] public AudioClip throwYut;
    [Header("OK2")] public AudioClip menu01;
    [Header("Cancel2")] public AudioClip cancel01;
    [Header("야구 휘두르는 소리")] public AudioClip swing;
    [Header("야구 맞는 소리")] public AudioClip baseballHit;
    [Header("야구 볼던지는소리 포격")] public AudioClip cannonShot;
    [Header("축구 차는 소리")] public AudioClip soccerHit;
    [Header("축구 막는 소리")] public AudioClip soccerDefance;
    [Header("성공")] public AudioClip succece;
    [Header("실패")] public AudioClip fail;
    [Header("다음턴")] public AudioClip nextTurn;
    //[Header("시계흘러가는소리")] public AudioClip clockRun;
    [Header("캐릭터 소리(TT.CharacterName)")] public AudioClip[] Chars;

    [Header("홈런")] public AudioClip homerunVoice;
    [Header("골인")] public AudioClip goalinVoice;

    [Header("시작")] public AudioClip[] gameStartVoices;
    [Header("빨리빨리")] public AudioClip[] hurryupVoices;
    [Header("끝")] public AudioClip[] gameEndVoices;
    [Header("힘내")] public AudioClip[] commonVoices;

    [Header("코인받는소리")] public AudioClip receiveCoin;
    [Header("폭탄받는소리")] public AudioClip receiveBoomb;
    [Header("결과창 열릴때")] public AudioClip resultPopup;
    [Header("Good판정")] public AudioClip good;
    //[Header("팝업 열릴때")] public AudioClip popupOpen;
    //[Header("팝업 닫힐때")] public AudioClip popupClose;


}
