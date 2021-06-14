using System;
using SimpleJSON;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.Runtime.InteropServices;
using UnityEngine.UI;
using TextSpeech;
using FrostweepGames.Plugins.Core;
using FrostweepGames.Plugins.GoogleCloud.TextToSpeech;
using UnityEngine.SceneManagement;
public class InGameManager : MonoBehaviour
{
    private string MicDevice = "";
    bool m_IsRecording = false;

    // 포인트
    public int m_iPoint = 0;
    public UILabel m_pUILabel_Point = null;
    public GameObject m_pEffect_Trans = null;
    public GameObject m_pEffect_Talk = null;
    public DateTime NextPointTime;

    // 알림창
    public GameObject m_objUI_Notice = null;

    // TTS
    public GCTextToSpeech m_GCTTS = null;
    public string m_sAPIKey = "AIzaSyCHn96RFsN2uZhZxxUWZNuaeS-V-3HrX30";

    // 보이스
    Dictionary<string, Voice> m_VoiceList;
    Voice m_CurrentVoice = null;
    UnityEngine.Audio.AudioMixerGroup m_MixerGroups_Master;
    UnityEngine.Audio.AudioMixerGroup m_MixerGroups_Custom;

    string m_sTransSpeakType = "Babby";
    Voice m_Voice_Babby = null;
    Voice m_Voice_Daddy = null;
    Voice m_Voice_Mom = null;
    bool m_bTransSpeak = false;

    public AudioSource m_AudioVoice = null;
    public GameObject m_objTalkButton = null;

    // 애니메이션
    public Animator m_pAnimator = null;
    public string m_sPlayAnim = "";
    public int m_iCurType = 0;
    public float m_fActionTime = 0f;

    // 터치관련
    int m_iOldTouchCnt = 0;
    public LayerMask m_Charlayer;
    public LayerMask m_UIlayer;
    public Camera m_pCamera = null;

    // UI관련
    public GameObject m_objUI = null;
    public GameObject m_objUI_Game = null;
    public GameObject m_objUI_Contents = null;

    public GameObject m_objUI_Shop = null;
    public GameObject m_objUI_Cloth = null;
    public GameObject m_objUI_Quit = null;
    public GameObject m_objUI_Lang = null;
    public GameObject m_objButton_Lang = null;
    public GameObject m_objUI_GamePop = null;
    public GameObject m_objUI_Voice = null;
    public GameObject m_objButton_Voice = null;

    // 아이템 장착관련
    public Material m_BaseMaterial = null;
    public Renderer m_BodyRenderer = null;

    public Transform m_trmEarL;
    public Transform m_trmEarR;
    public Transform[] m_trmEquipRoot = new Transform[3];
    
    public int m_HideEar = 0;
    int[] m_EquipMent = new int[4];
    GameObject[] m_objEquipItem = new GameObject[4];

    // 언어관련    
    string m_sLanguage = "KOR";
    string m_sLanguage_Trans = "KOR";
    string m_sInputMessage = "";
    string m_sTranslateInput = "";

    //Idle 처리
    float SleepTime = 0f;

    // 번역기
    int m_iTranslateVoice = 0;
    public GameObject m_objTransButton = null;
    public GameObject m_objTransLang = null;

    // 게임관련
    bool m_bPlayGame = false;
    float m_fMusicTime = 60f;
    public float m_fPlayTime = 0f;
    public float m_fFailedTime = 0f;
    public float m_fReadyTime = 0f;

    public Animator m_pAnimator_tear_L = null;
    public Animator m_pAnimator_tear_R = null;
    public AudioClip m_pAudioClip_Dance = null;

    public List<AudioClip> m_AudioClipList = new List<AudioClip>();
    public List<GameObject> m_EffetList = new List<GameObject>();
    public BubbleCreator m_pBubbleCreator = null;

    public GameObject m_pReady_3 = null;
    public GameObject m_pReady_2 = null;
    public GameObject m_pReady_1 = null;
    public GameObject m_pReady_Go = null;
    public GameObject m_pGuideLine = null;

    public GameObject m_pBad = null;
    public GameObject m_pGood = null;
    public GameObject m_pExcellent = null;

    public UILabel m_pUILabel_Score = null;
    public int m_iTotalScore = 0;
    public int m_iGuideLine = 0;

    public GameObject m_pFinish = null;
    public GameObject m_pGameRetry = null;
    public GameObject m_objUI_QuitGame = null;

    //============================================================================================
    // Awake
    // -
    //============================================================================================
    private void Awake()
    {
        m_sTransSpeakType = "Babby";
        CommandTable.GetInstance.LoadTable();
        ActionTable.GetInstance.LoadTable();
        ItemTable.GetInstance.LoadTable();
        LanguageTable.GetInstance.LoadTable();

        m_EquipMent[0] = PlayerPrefs.GetInt("Equip_1");
        m_EquipMent[1] = PlayerPrefs.GetInt("Equip_2");
        m_EquipMent[2] = PlayerPrefs.GetInt("Equip_3");
        m_EquipMent[3] = PlayerPrefs.GetInt("Equip_4");
        EquipItems(m_EquipMent[0]);
        EquipItems(m_EquipMent[1]);
        EquipItems(m_EquipMent[2]);
        EquipItems(m_EquipMent[3]);

        SleepTime = UnityEngine.Random.RandomRange(30f, 60f);
    }

    //============================================================================================
    // Start
    // - 
    //============================================================================================
    void Start()
    {
        if (Application.HasUserAuthorization(UserAuthorization.Microphone) == false)
            Application.RequestUserAuthorization(UserAuthorization.Microphone);

        m_sLanguage = "KOR";
        m_sLanguage_Trans = "KOR";

        SpeechToText.instance.Setting("ko-KR");
        SpeechToText.instance.onResultCallback = OnResultSpeech;

        SpeechToText.instance.onReadyForSpeechCallback = onReadyForSpeech;
        SpeechToText.instance.onEndOfSpeechCallback = OnEndOfSpeech;
        SpeechToText.instance.onBeginningOfSpeechCallback = OnBeginningOfSpeech;
        SpeechToText.instance.onErrorCallback = onError;

        UnityEngine.Audio.AudioMixer audioMixer = (UnityEngine.Audio.AudioMixer)Resources.Load("Sound/MicrophoneMixer");
        m_MixerGroups_Master = audioMixer.FindMatchingGroups("Master")[0];
        m_MixerGroups_Custom = audioMixer.FindMatchingGroups("Custom")[0];

        // 5분마다 포인트 제공
        string sDatae = PlayerPrefs.GetString("PointTime");
        if (sDatae == "")
        {
            sDatae = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            PlayerPrefs.SetString("PointTime", sDatae);

            m_iPoint = 1000;
            PlayerPrefs.SetInt("Point", m_iPoint);

            NextPointTime = DateTime.Now;
            NextPointTime = NextPointTime.AddMinutes(5);
        }
        else
        {
            m_iPoint = PlayerPrefs.GetInt("Point");

            DateTime myDate = DateTime.Parse(sDatae);
            TimeSpan timeDiff = DateTime.Now - myDate;
            double val = timeDiff.TotalMinutes / 5;

            m_iPoint += (int)val;
            if (m_iPoint > 9999)
                m_iPoint = 9999;

            sDatae = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            PlayerPrefs.SetString("PointTime", sDatae);
            PlayerPrefs.SetInt("Point", m_iPoint);

            NextPointTime = DateTime.Now;
            NextPointTime = NextPointTime.AddMinutes(5);
        }
        
        m_pUILabel_Point.text = m_iPoint.ToString();
    }

    //============================================================================================
    // OnDestroy
    // - 
    //============================================================================================
    private void OnDestroy()
    {
        m_GCTTS.GetVoicesSuccessEvent -= TextToSpeech_GetVoicesSuccessEvent;
        m_GCTTS.SynthesizeSuccessEvent -= TextToSpeech_SynthesizeSuccessEvent;
        m_GCTTS.GetVoicesFailedEvent -= TextToSpeech_GetVoicesFailedEvent;
        m_GCTTS.SynthesizeFailedEvent -= TextToSpeech_SynthesizeFailedEvent;
    }

    //============================================================================================
    // InitMic
    // - 
    //============================================================================================
    public void InitMic()
    {
        if (MicDevice == "")
        {
            MicDevice = Microphone.devices[0];

            // TTS
            m_GCTTS = FrostweepGames.Plugins.GoogleCloud.TextToSpeech.GCTextToSpeech.Instance;
            m_GCTTS.GetVoicesSuccessEvent += TextToSpeech_GetVoicesSuccessEvent;
            m_GCTTS.SynthesizeSuccessEvent += TextToSpeech_SynthesizeSuccessEvent;
            m_GCTTS.GetVoicesFailedEvent += TextToSpeech_GetVoicesFailedEvent;
            m_GCTTS.SynthesizeFailedEvent += TextToSpeech_SynthesizeFailedEvent;

            // 아빠
            m_Voice_Daddy = new Voice();
            m_Voice_Daddy.languageCodes = new string[1];
            m_Voice_Daddy.languageCodes[0] = "en-US";
            m_Voice_Daddy.name = "en-US-Wavenet-D";
            m_Voice_Daddy.ssmlGender = Enumerators.SsmlVoiceGender.MALE;
            m_Voice_Daddy.naturalSampleRateHertz = 24000;

            // 엄마
            m_Voice_Mom = new Voice();
            m_Voice_Mom.languageCodes = new string[1];
            m_Voice_Mom.languageCodes[0] = "en-US";
            m_Voice_Mom.name = "en-US-Wavenet-F";
            m_Voice_Mom.ssmlGender = Enumerators.SsmlVoiceGender.FEMALE;
            m_Voice_Mom.naturalSampleRateHertz = 24000;

            // 아기
            m_Voice_Babby = new Voice();
            m_Voice_Babby.languageCodes = new string[1];
            m_Voice_Babby.languageCodes[0] = "en-US";
            m_Voice_Babby.name = "en-US-Wavenet-H";
            m_Voice_Babby.ssmlGender = Enumerators.SsmlVoiceGender.FEMALE;
            m_Voice_Babby.naturalSampleRateHertz = 24000;

            // TTS 목소리 설정
            Voice pVoice = null;
            m_VoiceList = new Dictionary<string, Voice>();

            for (int i = 0; i < 4; i++)
            {
                pVoice = new Voice();
                pVoice.languageCodes = new string[1];
                pVoice.naturalSampleRateHertz = 24000;
                pVoice.ssmlGender = Enumerators.SsmlVoiceGender.FEMALE;

                if (i == 0)
                {
                    pVoice.languageCodes[0] = "ko-KR";
                    pVoice.name = "ko-KR-Wavenet-A";
                    m_VoiceList.Add("KOR", pVoice);
                }
                else if (i == 1)
                {
                    pVoice.languageCodes[0] = "en-US";
                    pVoice.name = "en-US-Wavenet-G";
                    m_VoiceList.Add("ENG", pVoice);
                }
                else if (i == 2)
                {
                    pVoice.languageCodes[0] = "cmn-CN";
                    pVoice.name = "cmn-CN-Wavenet-A";
                    m_VoiceList.Add("CHN", pVoice);
                }
                else if (i == 3)
                {
                    pVoice.languageCodes[0] = "ja-JP";
                    pVoice.name = "ja-JP-Wavenet-B";
                    m_VoiceList.Add("JPN", pVoice);
                }
            }

            // 보이스 설정
            m_CurrentVoice = m_VoiceList["KOR"];

            ActionTableData pData = (ActionTableData)ActionTable.GetInstance.m_DataList[0];
            m_sPlayAnim = pData.ActionAnim[0];
            string s = "";

            if (PlayerPrefs.HasKey("FirstConnect"))
            {
                PlayerPrefs.SetString("FirstConnect", "FirstConnect");
                s = LanguageTable.GetInstance.FindData(pData.ActionTalk[1], m_sLanguage);
                StartSpeak(s);
            }
            else
            {
                s = LanguageTable.GetInstance.FindData(pData.ActionTalk[0], m_sLanguage);
                StartSpeak(s);
            }
        }
    }

    //============================================================================================
    // LateUpdate
    // - 
    //============================================================================================
    private void LateUpdate()
    {
        if (m_HideEar == 1)
        {
            m_trmEarL.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            m_trmEarR.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        }
        else
        {
            m_trmEarL.localScale = new Vector3(1f, 1f, 1f);
            m_trmEarR.localScale = new Vector3(1f, 1f, 1f);
        }
    }

    //============================================================================================
    // Update
    // - 
    //============================================================================================
    void Update()
    {
        if (m_bPlayGame == false)
        {
            CheckIdle();
            CheckTime();
            CheckInput();
        }
        else
        {
            CheckPlayGame();
        }

        CheckAddPoint();
    }

    //============================================================================================
    // CheckAddPoint
    // - 
    //============================================================================================
    void CheckAddPoint()
    {
        if(NextPointTime < DateTime.Now )
        {
            TimeSpan timeDiff = DateTime.Now - NextPointTime;
            double val = timeDiff.TotalMinutes / 5 + 1;

            m_iPoint += (int)val;
            if (m_iPoint > 9999)
                m_iPoint = 9999;

            m_pUILabel_Point.text = m_iPoint.ToString();

            NextPointTime = DateTime.Now;
            NextPointTime = NextPointTime.AddMinutes(5);

            string sDatae = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            PlayerPrefs.SetString("PointTime", sDatae);
            PlayerPrefs.SetInt("Point", m_iPoint);
        }
    }

    //============================================================================================
    // CheckPlayGame
    // - 
    //============================================================================================
    void CheckPlayGame()
    {
        if (m_fReadyTime > 0f)
        {
            TweenScale pScale = null;
            TweenAlpha pAlpha = null;
            m_fReadyTime -= Time.unscaledDeltaTime;

            if (m_fReadyTime < 2f && m_fReadyTime > 1f)
            {
                if (m_pReady_2.activeSelf == false)
                {
                    m_pReady_2.SetActive(true);
                    pAlpha = m_pReady_2.GetComponent<TweenAlpha>();
                    pScale = m_pReady_2.GetComponent<TweenScale>();
                    m_AudioVoice.PlayOneShot(m_AudioClipList[0]);
                }
            }
            else if (m_fReadyTime < 1f && m_fReadyTime > 0f)
            {
                if (m_pReady_1.activeSelf == false)
                {
                    m_pReady_1.SetActive(true);
                    pAlpha = m_pReady_1.GetComponent<TweenAlpha>();
                    pScale = m_pReady_1.GetComponent<TweenScale>();
                    m_AudioVoice.PlayOneShot(m_AudioClipList[0]);
                }
            }
            else if (m_fReadyTime <= 0f)
            {
                if (m_pReady_Go.activeSelf == false)
                {
                    m_pReady_Go.SetActive(true);
                    pAlpha = m_pReady_Go.GetComponent<TweenAlpha>();
                    pScale = m_pReady_Go.GetComponent<TweenScale>();
                    m_AudioVoice.PlayOneShot(m_AudioClipList[0]);
                }

                m_fReadyTime = 0f;
                m_fPlayTime = Time.time;
                m_AudioVoice.pitch = 1f;
                m_AudioVoice.clip = m_pAudioClip_Dance;
                m_AudioVoice.Play();
                m_AudioVoice.outputAudioMixerGroup = m_MixerGroups_Master;
                m_pAnimator.CrossFadeInFixedTime("cute_A", 0.1f, -1, 0f);

                // 버블생성
                m_pBubbleCreator.SetStart(m_fMusicTime - 10f);
            }

            if (pAlpha != null)
            {
                pAlpha.enabled = true;
                pAlpha.ResetToBeginning();
            }

            if (pScale != null)
            {
                pScale.enabled = true;
                pScale.ResetToBeginning();
            }
        }


        //============================================================================================
        // 플레이
        if (m_fPlayTime > 0f)
        {
            float CurTime = Time.time - m_fPlayTime;

            if (m_fFailedTime > 0f)
            {
                m_fFailedTime -= Time.unscaledDeltaTime;

                if (m_fFailedTime <= 0f)
                {
                    m_pAnimator.CrossFadeInFixedTime("cute_A", 0.2f, -1, CurTime);
                    m_fFailedTime = 0f;
                }
            }

            // Finish
            if (m_fPlayTime + m_fMusicTime < Time.time)
            {
                if (m_pGameRetry.activeSelf == false)
                {
                    m_AudioVoice.PlayOneShot(m_AudioClipList[3]);
                    m_pGameRetry.SetActive(true);
                    m_pGuideLine.SetActive(false);

                    TweenScale pScale = m_pGameRetry.GetComponent<TweenScale>();
                    if (pScale != null)
                    {
                        pScale.enabled = true;
                        pScale.ResetToBeginning();
                    }

                    // Finish
                    m_pFinish.SetActive(true);
                    pScale = m_pFinish.GetComponent<TweenScale>();

                    if (pScale != null)
                    {
                        pScale.enabled = true;
                        pScale.ResetToBeginning();
                    }

                    TweenAlpha pTweenAlpha = m_pFinish.GetComponent<TweenAlpha>();
                    if (pTweenAlpha != null)
                    {
                        pTweenAlpha.enabled = true;
                        pTweenAlpha.ResetToBeginning();
                    }

                    int iVal = (int)(m_iTotalScore / 700);
                    m_iPoint += iVal;
                    if (m_iPoint > 9999)
                        m_iPoint = 9999;

                    m_pUILabel_Point.text = m_iPoint.ToString();
                    m_pFinish.transform.Find("Point/Label").GetComponent<UILabel>().text = "+ " + iVal.ToString();
                }
            }
        }
    }

    //============================================================================================
    // CheckIdle
    // - 
    //============================================================================================
    void CheckIdle()
    {
        AnimatorStateInfo pAnimatorStateInfo = m_pAnimator.GetCurrentAnimatorStateInfo(0);

        if (pAnimatorStateInfo.IsName("Idle"))
        {
            SleepTime -= Time.unscaledDeltaTime;
            if (SleepTime <= 0f)
            {
                SleepTime = UnityEngine.Random.RandomRange(30f, 60f);
                m_pAnimator.CrossFadeInFixedTime("10_mike_sleepy", 0.1f);
            }
        }
        else
        {
            if (pAnimatorStateInfo.IsName("11_mike_sleep"))
            {
                // 무반응
            }
            else
            {
                if (SleepTime < 30f)
                    SleepTime = UnityEngine.Random.RandomRange(30f, 60f);
            }
        }
    }

    //============================================================================================
    // ChaeckTime
    // - 시간체크
    //============================================================================================
    public void CheckTime()
    {
        // 액션시간 처리용
        if (m_fActionTime > 0f)
        {
            m_fActionTime -= Time.deltaTime;
            if (m_fActionTime <= 0f)
            {
                m_fActionTime = 0f;
                m_iCurType = 0;
            }
        }
    }

    //============================================================================================
    // CheckInput
    // -
    //============================================================================================
    void CheckInput()
    {
        if (m_fActionTime > 0f)
            return;

        AnimatorStateInfo pAnimatorStateInfo = m_pAnimator.GetCurrentAnimatorStateInfo(0);

#if UNITY_EDITOR    

        // 마우스 클릭
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            GameObject objSelect = null;

            Ray ray = UICamera.currentCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100f, m_UIlayer) == false)
            {
                ray = m_pCamera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, 100f, m_Charlayer))
                {
                    objSelect = hit.collider.gameObject;
                    ClickCharacter(objSelect);
                }
            }
        }

        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            UnityEngine.Audio.AudioMixer audioMixer = (UnityEngine.Audio.AudioMixer)Resources.Load("Sound/MicrophoneMixer");
            UnityEngine.Audio.AudioMixerGroup[] audioMixerGroups = audioMixer.FindMatchingGroups("Master");
            m_AudioVoice.outputAudioMixerGroup = audioMixerGroups[0];

            m_sTransSpeakType = "Daddy";
            string s = "Google TTS, a smart AI translator, dreams of a world of conversation without language barriers.";
            m_bTransSpeak = true;
            m_GCTTS.Synthesize(s,
                    new VoiceConfig()
                    {
                        gender = m_Voice_Daddy.ssmlGender,
                        languageCode = m_Voice_Daddy.languageCodes[0],
                        name = m_Voice_Daddy.name
                    },
                    false, 1f, 1f, m_Voice_Daddy.naturalSampleRateHertz);
        }

        if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            UnityEngine.Audio.AudioMixer audioMixer = (UnityEngine.Audio.AudioMixer)Resources.Load("Sound/MicrophoneMixer");
            UnityEngine.Audio.AudioMixerGroup[] audioMixerGroups = audioMixer.FindMatchingGroups("Master");
            m_AudioVoice.outputAudioMixerGroup = audioMixerGroups[0];

            m_sTransSpeakType = "Mom";
            string s = "Google TTS, a smart AI translator, dreams of a world of conversation without language barriers.";
            m_bTransSpeak = true;
            m_GCTTS.Synthesize(s,
                    new VoiceConfig()
                    {
                        gender = m_Voice_Mom.ssmlGender,
                        languageCode = m_Voice_Mom.languageCodes[0],
                        name = m_Voice_Mom.name
                    },
                    false, 1f, 1f, m_Voice_Mom.naturalSampleRateHertz);
        }

        if (Input.GetKeyUp(KeyCode.Alpha3))
        {
            UnityEngine.Audio.AudioMixer audioMixer = (UnityEngine.Audio.AudioMixer)Resources.Load("Sound/MicrophoneMixer");
            UnityEngine.Audio.AudioMixerGroup[] audioMixerGroups = audioMixer.FindMatchingGroups("Master");
            m_AudioVoice.outputAudioMixerGroup = audioMixerGroups[0];

            m_sTransSpeakType = "Babby";
            string s = "Google TTS, a smart AI translator, dreams of a world of conversation without language barriers.";
            m_AudioVoice.pitch = 1f;
            m_bTransSpeak = true;
            m_GCTTS.Synthesize(s,
                    new VoiceConfig()
                    {
                        gender = m_Voice_Babby.ssmlGender,
                        languageCode = m_Voice_Babby.languageCodes[0],
                        name = m_Voice_Babby.name
                    },
                    false, 1f, 1f, m_Voice_Babby.naturalSampleRateHertz);
        }

        if (Input.GetKeyUp(KeyCode.Alpha4))
        {
            m_sTransSpeakType = "Robot";
            UnityEngine.Audio.AudioMixer audioMixer = (UnityEngine.Audio.AudioMixer)Resources.Load("Sound/MicrophoneMixer");
            UnityEngine.Audio.AudioMixerGroup[] audioMixerGroups = audioMixer.FindMatchingGroups("Custom");
            m_AudioVoice.outputAudioMixerGroup = audioMixerGroups[0];

            string s = "Google TTS, a smart AI translator, dreams of a world of conversation without language barriers.";
            m_bTransSpeak = true;
            m_GCTTS.Synthesize(s,
                    new VoiceConfig()
                    {
                        gender = m_Voice_Daddy.ssmlGender,
                        languageCode = m_Voice_Daddy.languageCodes[0],
                        name = m_Voice_Daddy.name
                    },
                    false, 1f, 1f, m_Voice_Daddy.naturalSampleRateHertz);
        }

        if (Input.GetKeyUp(KeyCode.Alpha5))
        {
            UnityEngine.Audio.AudioMixer audioMixer = (UnityEngine.Audio.AudioMixer)Resources.Load("Sound/MicrophoneMixer");
            UnityEngine.Audio.AudioMixerGroup[] audioMixerGroups = audioMixer.FindMatchingGroups("Master");
            m_AudioVoice.outputAudioMixerGroup = audioMixerGroups[0];
        }

        if (Input.GetKeyUp(KeyCode.Alpha6))
        {
            ChaeckVoiceCommand("노래 불러줘");
        }

#else

        // 모바일 터치
        if (Input.touchCount > 0 && m_iOldTouchCnt == 0)
        {
            RaycastHit hit;
            GameObject objSelect = null;

            Ray ray = UICamera.currentCamera.ScreenPointToRay(Input.touches[0].position);
            if (Physics.Raycast(ray, out hit, 100f, m_UIlayer) == false)
            {
                ray = m_pCamera.ScreenPointToRay(Input.touches[0].position);
                if (Physics.Raycast(ray, out hit, 100f, m_Charlayer))
                {
                    objSelect = hit.collider.gameObject;                    
                    ClickCharacter(objSelect);
                }
            }
        }
#endif

        m_iOldTouchCnt = Input.touchCount;
    }

    //============================================================================================
    // ClickCharacter
    // - 캐릭터 클릭
    //============================================================================================
    public void ClickCharacter(GameObject obj)
    {
        if (m_iTranslateVoice > 0 || m_IsRecording)
            return;

        AnimatorStateInfo pAnimatorStateInfo = m_pAnimator.GetCurrentAnimatorStateInfo(0);
        if (pAnimatorStateInfo.IsName("11_mike_sleep"))
        {
            m_fActionTime = 1f;
            m_sPlayAnim = "12_mike_getup";
            string s = LanguageTable.GetInstance.FindData(17, m_sLanguage);
            StartSpeak(s);
        }
        else if (pAnimatorStateInfo.IsName("Idle"))
        {
            if (obj.name == "Head")
            {
                m_fActionTime = 1f;
                m_sPlayAnim = "fail_ani_left";
                string s = LanguageTable.GetInstance.FindData(15, m_sLanguage);
                StartSpeak(s);
            }
            else if (obj.name == "Body")
            {
                m_fActionTime = 1f;
                m_sPlayAnim = "09_mike_cheer";
                string s = LanguageTable.GetInstance.FindData(16, m_sLanguage);
                StartSpeak(s);
            }
            else if (obj.name == "Nose")
            {
                m_fActionTime = 1f;
                m_pAnimator.CrossFadeInFixedTime("16_mike_cute", 0.1f);
            }
        }
    }

    //============================================================================================
    // CloseNoticePopup
    // - 
    //============================================================================================
    public void CloseNoticePopup()
    {
        m_objUI_Notice.SetActive(false);
    }

    //============================================================================================
    // OpenClothPopup
    // - 꾸미기 열기
    //============================================================================================
    public void OpenClothPopup(GameObject obj)
    {
        // Debug.Log("OpenClothPopup");
        if (m_objUI_Cloth.activeSelf)
        {
            m_objUI_Cloth.SetActive(false);
            obj.GetComponent<UIButton>().normalSprite = "btn-magic-on";
            return;
        }
        else
        {
            obj.GetComponent<UIButton>().normalSprite = "btn-magic-off";
            m_objUI_Cloth.SetActive(true);
            m_objUI_Cloth.transform.Find("Button_Cap").GetComponent<TweenPosition>().enabled = true;
            m_objUI_Cloth.transform.Find("Button_Cap").GetComponent<TweenPosition>().ResetToBeginning();
            m_objUI_Cloth.transform.Find("Button_Eye").GetComponent<TweenPosition>().enabled = true;
            m_objUI_Cloth.transform.Find("Button_Eye").GetComponent<TweenPosition>().ResetToBeginning();
            m_objUI_Cloth.transform.Find("Button_Neck").GetComponent<TweenPosition>().enabled = true;
            m_objUI_Cloth.transform.Find("Button_Neck").GetComponent<TweenPosition>().ResetToBeginning();
            m_objUI_Cloth.transform.Find("Button_Suit").GetComponent<TweenPosition>().enabled = true;
            m_objUI_Cloth.transform.Find("Button_Suit").GetComponent<TweenPosition>().ResetToBeginning();
        }
    }

    //============================================================================================
    // CloseClothPopup
    // - 꾸미기 닫기
    //============================================================================================
    public void CloseClothPopup()
    {
        Debug.Log("CloseClothPopup");
        m_objUI_Cloth.SetActive(false);
    }

    //============================================================================================
    // ClickCloth_Suit
    // -
    //============================================================================================
    public void ClickCloth_Suit()
    {
        Debug.Log("ClickCloth_Suit");
        CheckEquipItem(4);
    }

    //============================================================================================
    // ClickCloth_Hat
    // -
    //============================================================================================
    public void ClickCloth_Hat()
    {
        Debug.Log("ClickCloth_Hat");
        CheckEquipItem(1);
    }

    //============================================================================================
    // ClickCloth_Glass
    // -
    //============================================================================================
    public void ClickCloth_Glass()
    {
        Debug.Log("ClickCloth_Glass");
        CheckEquipItem(2);
    }

    //============================================================================================
    // ClickCloth_Neck
    // -
    //============================================================================================
    public void ClickCloth_Neck()
    {
        Debug.Log("ClickCloth_Neck");
        CheckEquipItem(3);
    }

    //============================================================================================
    // CheckEquipItem
    // -
    //============================================================================================
    public void CheckEquipItem(int iType)
    {
        ItemTableData pItemTableData = null;
        foreach (ItemTableData data in ItemTable.GetInstance.m_DataList.Values)
        {
            if (data.Type == iType)
            {
                if (data.ID > m_EquipMent[iType - 1])
                {
                    pItemTableData = data;
                    break;
                }
            }
        }

        if (pItemTableData == null)
        {
            m_EquipMent[iType - 1] = 0;
            GameObject.Destroy(m_objEquipItem[iType - 1]);
            m_objEquipItem[iType - 1] = null;

            if (iType == 1)
            {
                m_HideEar = 0;
            }
            else if (iType == 4)
            {
                m_BodyRenderer.material = m_BaseMaterial;
                ArrayList pArrayList = new ArrayList();
                GameObject obj = null;

                for (int i = 0; i < m_pAnimator.transform.childCount; i++)
                {
                    obj = m_pAnimator.transform.transform.GetChild(i).gameObject;
                    if (obj.name == "Suit")
                        pArrayList.Add(obj);
                }

                for (int i = 0; i < pArrayList.Count; i++)
                {
                    obj = (GameObject)pArrayList[i];
                    GameObject.Destroy(obj);
                    obj = null;
                }
            }

            PlayerPrefs.SetInt("Equip_" + iType.ToString(), 0);
        }
        else
        {
            GameObject.Destroy(m_objEquipItem[iType - 1]);
            m_objEquipItem[iType - 1] = null;
            m_EquipMent[iType - 1] = pItemTableData.ID;

            if (iType == 1)
                m_HideEar = pItemTableData.HideEar;
            GameObject obj = GameObject.Instantiate((GameObject)Resources.Load("Item/" + pItemTableData.Model));
            m_objEquipItem[iType - 1] = obj;

            if (iType != 4)
            {
                obj.transform.parent = m_trmEquipRoot[iType - 1];
                obj.transform.localPosition = pItemTableData.Pos;
                obj.transform.localRotation = pItemTableData.Rot;
                obj.transform.localScale = pItemTableData.Scale;

                if (iType == 1)
                    m_pAnimator.CrossFadeInFixedTime("13_mike_hat", 0.1f);
                else if (iType == 2)
                    m_pAnimator.CrossFadeInFixedTime("14_mike_glass", 0.1f);
                else if (iType == 3)
                    m_pAnimator.CrossFadeInFixedTime("15_mike_necktie", 0.1f);
            }
            else
            {
                // TODO : 슈트장착
                obj.transform.parent = m_pAnimator.transform;
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localRotation = Quaternion.identity;
                obj.transform.localScale = Vector3.one;
                obj.name = "Suit";
                Utillity.CharngeParts(obj, m_pAnimator.transform, m_pAnimator.transform, "Suit", false);
                m_BodyRenderer.material = (Material)Resources.Load("Item/" + pItemTableData.MatAlpha);
            }

            PlayerPrefs.SetInt("Equip_" + iType.ToString(), pItemTableData.ID);
        }
    }

    //============================================================================================
    // EquipItems
    // -
    //============================================================================================
    public void EquipItems(int ID)
    {
        if (ID == 0)
            return;

        ItemTableData pItemTableData = ItemTable.GetInstance.m_DataList[ID];

        if (pItemTableData.Type == 1)
            m_HideEar = pItemTableData.HideEar;

        GameObject obj = GameObject.Instantiate((GameObject)Resources.Load("Item/" + pItemTableData.Model));
        m_objEquipItem[pItemTableData.Type - 1] = obj;

        if (pItemTableData.Type != 4)
        {            
            obj.transform.parent = m_trmEquipRoot[pItemTableData.Type - 1];
            obj.transform.localPosition = pItemTableData.Pos;
            obj.transform.localRotation = pItemTableData.Rot;
            obj.transform.localScale = pItemTableData.Scale;
        }
        else
        {
            // TODO : 슈트장착
            obj.transform.parent = m_pAnimator.transform;
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
            obj.transform.localScale = Vector3.one;
            obj.name = "Suit";
            Utillity.CharngeParts(obj, m_pAnimator.transform, m_pAnimator.transform, "Suit", false);
            m_BodyRenderer.material = (Material)Resources.Load("Item/" + pItemTableData.MatAlpha);
        }
    }

    //============================================================================================
    // OpenPopup_Quit
    // - 종료창 열기
    //============================================================================================
    public void OpenPopup_Quit()
    {
        Debug.Log("OpenPopup_Quit");
        if (m_objUI_Quit.activeSelf)
            return;

        m_objUI_Quit.SetActive(true);
        m_objUI_Quit.GetComponent<TweenScale>().enabled = true;
        m_objUI_Quit.GetComponent<TweenScale>().ResetToBeginning();
    }

    //============================================================================================
    // ClosePopup_Quit
    // - 종료창 닫기
    //============================================================================================
    public void ClosePopup_Quit()
    {
        m_objUI_Quit.SetActive(false);
    }

    //============================================================================================
    // QuitApp
    // - 
    //============================================================================================
    public void QuitApp()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    //============================================================================================
    // StartRecording
    // - 
    //============================================================================================
    public void StartRecording_Talk()
    {
        if (m_IsRecording == false)
        {
            m_sLanguage = "KOR";
            SpeechToText.instance.Setting("ko-KR");
        }

        StartRecording();
    }

    //============================================================================================
    // StartRecording
    // - 
    //============================================================================================
    public void StartRecording()
    {
        if (m_IsRecording == false)
        {
            Debug.Log("StartRecording");            

            if (m_iTranslateVoice == 0)
            {
                if (m_iPoint < 1)
                {
                    // 알림창
                    m_objUI_Notice.SetActive(true);
                    m_objUI_Notice.GetComponent<TweenScale>().enabled = true;
                    m_objUI_Notice.GetComponent<TweenScale>().ResetToBeginning();
                    return;
                }

                m_iPoint -= 1;
                m_pUILabel_Point.text = m_iPoint.ToString();
                m_objTalkButton.GetComponent<UIButton>().normalSprite = "voice-command";

                m_pEffect_Talk.SetActive(true);
                m_pEffect_Talk.GetComponent<TweenAlpha>().enabled = true;
                m_pEffect_Talk.GetComponent<TweenAlpha>().ResetToBeginning();
                m_pEffect_Talk.GetComponent<TweenPosition>().enabled = true;
                m_pEffect_Talk.GetComponent<TweenPosition>().ResetToBeginning();
            }
            else
            {
                if(m_sLanguage_Trans == "KOR")
                    m_objTransButton.GetComponent<UIButton>().normalSprite = "voice-eng";
                else if (m_sLanguage_Trans == "ENG")
                    m_objTransButton.GetComponent<UIButton>().normalSprite = "voice-kor";
            }

            SpeechToText.instance.StartRecording("Speak any");
            m_IsRecording = true;
            SleepTime = UnityEngine.Random.RandomRange(30f, 60f);
        }
        else
        {
            m_IsRecording = false;
            SpeechToText.instance.StopRecording();
            SleepTime = UnityEngine.Random.RandomRange(30f, 60f);
            m_objTalkButton.GetComponent<UIButton>().normalSprite = "voice-command2";
            m_objTransButton.GetComponent<UIButton>().normalSprite = "voice-eng2";
        }
    }

    //============================================================================================
    // ClickLanguage
    // -
    //============================================================================================
    public void ClickLanguage()
    {
        Debug.Log("ClickLanguage");
        if (m_IsRecording == true || m_iTranslateVoice > 0)
            return;

        if (m_objUI_Lang.activeSelf)
        {
            m_objUI_Lang.SetActive(false);
        }
        else
        {
            m_objUI_Lang.SetActive(true);
            m_objUI_Lang.transform.Find("KOR/Select").gameObject.SetActive(false);
            m_objUI_Lang.transform.Find("ENG/Select").gameObject.SetActive(false);
            m_objUI_Lang.transform.Find("CHN/Select").gameObject.SetActive(false);
            m_objUI_Lang.transform.Find("JPN/Select").gameObject.SetActive(false);
            m_objUI_Lang.transform.Find(m_sLanguage + "/Select").gameObject.SetActive(true);
        }
    }

    //============================================================================================
    // ClickSelectLanguage
    // -
    //============================================================================================
    public void ClickSelectLanguage(GameObject obj)
    {
        if (m_IsRecording == true || m_iTranslateVoice > 0)
            return;

        m_sLanguage = obj.name;
        m_objUI_Lang.SetActive(false);
        m_objButton_Lang.transform.Find("Label").GetComponent<UILabel>().text = m_sLanguage;

        if (m_sLanguage == "KOR")
        {
            m_objButton_Lang.transform.Find("Icon").GetComponent<UISprite>().spriteName = "flag-kor";
            SpeechToText.instance.Setting("ko-KR");
        }
        else if (m_sLanguage == "ENG")
        {
            m_objButton_Lang.transform.Find("Icon").GetComponent<UISprite>().spriteName = "flag-usa";
            SpeechToText.instance.Setting("en-US");
        }
        else if (m_sLanguage == "CHN")
        {
            m_objButton_Lang.transform.Find("Icon").GetComponent<UISprite>().spriteName = "flag-chn";
            SpeechToText.instance.Setting("zh");
        }
        else if (m_sLanguage == "JPN")
        {
            m_objButton_Lang.transform.Find("Icon").GetComponent<UISprite>().spriteName = "flag-jpn";
            SpeechToText.instance.Setting("ja-JP");
        }

        m_CurrentVoice = m_VoiceList[m_sLanguage];
    }

    //============================================================================================
    // OnResultSpeech
    // - 
    //============================================================================================
    void OnResultSpeech(string _data)
    {
        Debug.Log("OnResultSpeech : " + _data);
        m_IsRecording = false;
        if (_data != "")
        {
            if (m_iTranslateVoice == 0)
            {
                ChaeckVoiceCommand(_data);
                m_objTalkButton.GetComponent<UIButton>().normalSprite = "voice-command2";
            }
            else if (m_iTranslateVoice == 1)
            {
                m_sTranslateInput = _data;
                ClickTranslate();
                m_objTransButton.GetComponent<UIButton>().normalSprite = "voice-eng2";
            }
        }
    }

    //============================================================================================
    // onError
    // - 
    //============================================================================================
    void onError(string _data)
    {
        Debug.Log("onError : " + _data);
        m_IsRecording = false;
        m_iTranslateVoice = 0;
        m_sInputMessage = "";
        m_sTranslateInput = "";

        m_objTalkButton.GetComponent<UIButton>().normalSprite = "voice-command2";
        m_objTransButton.GetComponent<UIButton>().normalSprite = "voice-eng2";
        m_sLanguage = "KOR";
    }

    //============================================================================================
    // ChaeckVoiceCommand
    // - 음성입력 체크 
    //============================================================================================
    void ChaeckVoiceCommand(string sInput)
    {
        if (sInput == "" || m_fActionTime > 0f)
            return;

        // 문자 검사
        CommandTableData pData = CommandTable.GetInstance.FindData(sInput);
        if (pData == null)
            return;

        ActionTableData pAction = ActionTable.GetInstance.FindData(pData.Action, m_iCurType);

        // 액션처리
        if (pAction != null)
        {
            m_iCurType = pAction.Type;
            m_fActionTime = pAction.ActionTime;
            int rnd = UnityEngine.Random.Range(0, pAction.RndCnt);

            AnimatorStateInfo pAnimatorStateInfo = m_pAnimator.GetCurrentAnimatorStateInfo(0);
            if (pAnimatorStateInfo.IsName("11_mike_sleep"))
            {
                m_fActionTime = 1f;
                m_sPlayAnim = "12_mike_getup";
                string s = LanguageTable.GetInstance.FindData(17, m_sLanguage);
                StartSpeak(s);
            }
            else
            {
                switch (pAction.Type)
                {
                    case 1:
                        {
                            // 말하기                    
                            m_sPlayAnim = pAction.ActionAnim[rnd];
                            string s = LanguageTable.GetInstance.FindData(pAction.ActionTalk[rnd], m_sLanguage);
                            StartSpeak(s);
                            break;
                        }

                    case 2:
                        {
                            // 춤추기
                            m_pAnimator.CrossFadeInFixedTime(pAction.ActionAnim[rnd], 0.1f);
                            break;
                        }

                    case 3:
                        {
                            // 노래하기
                            m_AudioVoice.clip = (AudioClip)Resources.Load("Sound/Music/" + pAction.Sound[rnd]);
                            m_AudioVoice.pitch = 1f;
                            m_AudioVoice.Play();
                            m_AudioVoice.outputAudioMixerGroup = m_MixerGroups_Master;
                            m_pAnimator.CrossFadeInFixedTime(pAction.ActionAnim[rnd], 0.1f);
                            break;
                        }
                    case 4:
                        {
                            // 말하기                    
                            m_sPlayAnim = pAction.ActionAnim[rnd];
                            string s = LanguageTable.GetInstance.FindData(pAction.ActionTalk[rnd], m_sLanguage);
                            StartSpeak(s);
                            Invoke("QuitApp", 2f);
                            break;
                        }
                    case 5:
                        {
                            // 볼륨 내려
                            float fVal = m_AudioVoice.volume;
                            fVal -= 0.2f;
                            if (fVal <= 0f)
                                fVal = 0f;
                            m_AudioVoice.volume = fVal;
                            PlayerPrefs.SetFloat("Volume", fVal);
                            break;
                        }
                    case 6:
                        {
                            // 볼륨 올려
                            float fVal = m_AudioVoice.volume;
                            fVal += 0.2f;
                            if (fVal > 1f)
                                fVal = 1f;
                            m_AudioVoice.volume = fVal;
                            PlayerPrefs.SetFloat("Volume", fVal);
                            break;
                        }
                }
            }
        }
    }

    //============================================================================================
    // StartSpeak
    // - 
    //============================================================================================
    public void StartSpeak(string s)
    {
        if (s == "")
            return;

        m_sInputMessage = s;
        m_AudioVoice.Stop();
        Invoke("SpeakLipsync", 0.5f);
    }

    //============================================================================================
    // SpeakLipsync
    // - 
    //============================================================================================
    public void SpeakLipsync()
    {
        m_GCTTS.Synthesize(
            m_sInputMessage,
            new VoiceConfig()
            {
                gender = m_CurrentVoice.ssmlGender,
                languageCode = m_CurrentVoice.languageCodes[0],
                name = m_CurrentVoice.name
            },
            false, 1f, 1f, m_CurrentVoice.naturalSampleRateHertz);
    }

    //============================================================================================
    // ClickTranslate
    // -
    //============================================================================================
    public void ClickTranslate()
    {
        Debug.Log("ClickTranslate");
        if (m_IsRecording == true)
        {
            if (m_iTranslateVoice != 0)
            {
                SpeechToText.instance.StopRecording();
                m_objTalkButton.GetComponent<UIButton>().normalSprite = "voice-command2";
                m_objTransButton.GetComponent<UIButton>().normalSprite = "voice-eng2";
            }
            return;
        }

        SleepTime = UnityEngine.Random.RandomRange(30f, 60f);
        if (m_iTranslateVoice == 0)
        {
            if(m_iPoint < 3)
            {
                // 알림창
                m_objUI_Notice.SetActive(true);
                m_objUI_Notice.GetComponent<TweenScale>().enabled = true;
                m_objUI_Notice.GetComponent<TweenScale>().ResetToBeginning();
                return;
            }

            m_iPoint -= 3;
            m_pUILabel_Point.text = m_iPoint.ToString();

            m_pEffect_Trans.SetActive(true);
            m_pEffect_Trans.GetComponent<TweenAlpha>().enabled = true;
            m_pEffect_Trans.GetComponent<TweenAlpha>().ResetToBeginning();
            m_pEffect_Trans.GetComponent<TweenPosition>().enabled = true;
            m_pEffect_Trans.GetComponent<TweenPosition>().ResetToBeginning();


            m_AudioVoice.Stop();
            AnimatorStateInfo pAnimatorStateInfo = m_pAnimator.GetCurrentAnimatorStateInfo(0);
            if (pAnimatorStateInfo.IsName("11_mike_sleep"))
                m_pAnimator.CrossFadeInFixedTime("12_mike_getup", 0.1f);
            else
                m_pAnimator.CrossFadeInFixedTime("Idle", 0.1f);

            m_iTranslateVoice = 1;
            if (m_sLanguage_Trans == "KOR")
                SpeechToText.instance.Setting("ko-KR");
            else if (m_sLanguage_Trans == "ENG")
                SpeechToText.instance.Setting("en-US");
            else if (m_sLanguage_Trans == "CHN")
                SpeechToText.instance.Setting("zh");
            else if (m_sLanguage_Trans == "JPN")
                SpeechToText.instance.Setting("ja-JP");

            StartRecording();
        }
        else if (m_iTranslateVoice == 1)
        {
            m_iTranslateVoice = 2;
            string s = m_sTranslateInput;

            if (m_sLanguage_Trans == "KOR")
                StartCoroutine(TranslateTextRoutine("ko", "en", m_sTranslateInput));
            else if (m_sLanguage_Trans == "ENG")
                StartCoroutine(TranslateTextRoutine("en", "ko", m_sTranslateInput));
            else if (m_sLanguage_Trans == "CHN")
                StartCoroutine(TranslateTextRoutine("zh", "ko", m_sTranslateInput));
            else if (m_sLanguage_Trans == "JPN")
                StartCoroutine(TranslateTextRoutine("ja", "ko", m_sTranslateInput));
        }
    }

    //============================================================================================
    // ClickTranslate
    // -
    //============================================================================================
    public void ClickTranslate_EnKr()
    {
        Debug.Log("ClickTranslate");
        if (m_IsRecording == true || m_iTranslateVoice != 0)
            return;

        if (m_sLanguage_Trans == "KOR")
        {
            m_sLanguage_Trans = "ENG";
            m_objTransLang.GetComponent<UIButton>().normalSprite = "btn-eng";
        }
        else if (m_sLanguage_Trans == "ENG")
        {
            m_sLanguage_Trans = "KOR";
            m_objTransLang.GetComponent<UIButton>().normalSprite = "btn-kor";
        }
    }

    //============================================================================================
    // TranslateTextRoutine
    // -
    //============================================================================================
    private IEnumerator TranslateTextRoutine(string sourceLanguage, string targetLanguage, string sourceText)
    {
        string url = "https://translate.googleapis.com/translate_a/single?client=gtx&sl="
            + sourceLanguage + "&tl=" + targetLanguage + "&dt=t&q=" + WWW.EscapeURL(sourceText);

        WWW www = new WWW(url);
        yield return www;

        string s = "";
        if (www.isDone)
        {
            if (string.IsNullOrEmpty(www.error))
            {
                var N = JSONNode.Parse(www.text);
                if (N[0] != null)
                    s = N[0][0][0];
                Debug.Log("TranslateTextRoutine : " + s);
            }
        }

        yield return new WaitForSeconds(0.5f);

        Voice pVoice = null;

        if (m_sLanguage_Trans == "ENG")
        {
            pVoice = m_VoiceList["KOR"];
        }
        else
        {
            if (m_sTransSpeakType == "Daddy")
                pVoice = m_Voice_Daddy;
            else if (m_sTransSpeakType == "Mom")
                pVoice = m_Voice_Mom;
            else if (m_sTransSpeakType == "Babby")
                pVoice = m_Voice_Babby;
            else if (m_sTransSpeakType == "Robot")
                pVoice = m_Voice_Daddy;
        }

        m_bTransSpeak = true;
        m_GCTTS.Synthesize(s,
                new VoiceConfig()
                {
                    gender = pVoice.ssmlGender,
                    languageCode = pVoice.languageCodes[0],
                    name = pVoice.name
                },
                false, 1f, 1f, pVoice.naturalSampleRateHertz);

        m_iTranslateVoice = 0;
    }

    //============================================================================================
    // TextToSpeech_SynthesizeSuccessEvent
    // - 
    //============================================================================================
    private void TextToSpeech_SynthesizeSuccessEvent(PostSynthesizeResponse response)
    {
        if (m_bTransSpeak)
        {
            m_bTransSpeak = false;

            if (m_sLanguage_Trans == "KOR")
            {
                if (m_sTransSpeakType == "Babby")
                    m_AudioVoice.pitch = 1.1f;
                else if (m_sTransSpeakType == "Robot")
                    m_AudioVoice.pitch = 1.1f;
                else
                    m_AudioVoice.pitch = 1f;

                if (m_sTransSpeakType == "Robot")
                    m_AudioVoice.outputAudioMixerGroup = m_MixerGroups_Custom;
                else
                    m_AudioVoice.outputAudioMixerGroup = m_MixerGroups_Master;
            }
            else if (m_sLanguage_Trans == "ENG")
            {
                m_AudioVoice.pitch = 1.1f;
                m_AudioVoice.outputAudioMixerGroup = m_MixerGroups_Master;
            }
        }
        else
        {
            m_AudioVoice.pitch = 1.15f;
            m_AudioVoice.outputAudioMixerGroup = m_MixerGroups_Master;
        }

        m_AudioVoice.clip = m_GCTTS.GetAudioClipFromBase64(response.audioContent, FrostweepGames.Plugins.GoogleCloud.TextToSpeech.Constants.DEFAULT_AUDIO_ENCODING);
        m_AudioVoice.Play();

        if (m_sPlayAnim != "" && m_sPlayAnim != "0")
        {
            m_pAnimator.CrossFadeInFixedTime(m_sPlayAnim, 0.1f);
            m_sPlayAnim = "";
        }

        m_sLanguage = "KOR";
    }

    //============================================================================================
    // ClickOpenVoice
    // - 
    //============================================================================================
    public void ClickOpenVoice()
    {
        if (m_IsRecording == true || m_iTranslateVoice > 0)
            return;

        if (m_objUI_Voice.activeSelf)
        {
            m_objUI_Voice.SetActive(false);
        }
        else
        {
            m_objUI_Voice.SetActive(true);
            m_objUI_Voice.transform.Find("Daddy/Select").gameObject.SetActive(false);
            m_objUI_Voice.transform.Find("Mom/Select").gameObject.SetActive(false);
            m_objUI_Voice.transform.Find("Babby/Select").gameObject.SetActive(false);
            m_objUI_Voice.transform.Find("Robot/Select").gameObject.SetActive(false);
            m_objUI_Voice.transform.Find(m_sTransSpeakType + "/Select").gameObject.SetActive(true);
        }
    }

    //============================================================================================
    // ClickSelectVoice
    // - 
    //============================================================================================
    public void ClickSelectVoice(GameObject obj)
    {
        // m_objButton_Voice
        m_sTransSpeakType = obj.name;
        m_objUI_Voice.SetActive(false);

        string s = "";
        if (m_sTransSpeakType == "Daddy")
        {
            s = "아빠 목소리";
            m_objButton_Voice.transform.Find("Icon").GetComponent<UISprite>().spriteName = "icon-dad";
        }
        else if (m_sTransSpeakType == "Mom")
        {
            s = "엄마 목소리";
            m_objButton_Voice.transform.Find("Icon").GetComponent<UISprite>().spriteName = "icon-mom";
        }
        else if (m_sTransSpeakType == "Babby")
        {
            s = "아기 목소리";
            m_objButton_Voice.transform.Find("Icon").GetComponent<UISprite>().spriteName = "icon-bebe";
        }
        else if (m_sTransSpeakType == "Robot")
        {
            s = "로봇 목소리";
            m_objButton_Voice.transform.Find("Icon").GetComponent<UISprite>().spriteName = "icon-robo";
        }

        m_objButton_Voice.transform.Find("Label").GetComponent<UILabel>().text = s;
    }
    public void TestButton1()
    {
        SceneManager.LoadScene("Sliding");
    }
    public void TestButton2()
    {
        SceneManager.LoadScene("FindColor");
    }
    //============================================================================================
    // ClickOpenPopupGame
    // - 
    //============================================================================================
    public void ClickOpenPopupGame()
    {
        if (m_IsRecording)
            return;

        m_objUI_GamePop.SetActive(true);
        m_objUI_GamePop.GetComponent<TweenScale>().enabled = true;
        m_objUI_GamePop.GetComponent<TweenScale>().ResetToBeginning();
    }

    //============================================================================================
    // ClickClosePopupGame
    // - 
    //============================================================================================
    public void ClickClosePopupGame()
    {
        m_objUI_GamePop.SetActive(false);
    }

    //============================================================================================
    // ClickPlayGame
    // - 
    //============================================================================================
    public void ClickPlayGame()
    {
        m_objUI_GamePop.SetActive(false);
        m_objUI_Contents.SetActive(false);
        m_objUI_Game.SetActive(true);

        m_iTotalScore = 0;
        m_pUILabel_Score.text = "0";

        m_pGuideLine.transform.localPosition = new Vector3(0f, (float)m_iGuideLine, 0f);
        m_pGuideLine.SetActive(true);

        m_bPlayGame = true;
        m_fReadyTime = 3f;
        m_fPlayTime = 0f;
        m_fFailedTime = 0f;

        TweenAlpha pAlpha = m_pReady_3.GetComponent<TweenAlpha>();
        if (pAlpha != null)
        {
            pAlpha.enabled = true;
            pAlpha.ResetToBeginning();
        }

        TweenScale pScale = m_pReady_3.GetComponent<TweenScale>();
        if (pScale != null)
        {
            pScale.enabled = true;
            pScale.ResetToBeginning();
        }

        m_pReady_3.SetActive(true);
        m_AudioVoice.PlayOneShot(m_AudioClipList[0]);
    }

    //============================================================================================
    // ClickBubble
    // - 
    //============================================================================================
    public void ClickBubble(GameObject obj)
    {
        // Debug.Log("ClickBubble : " + obj.transform.localPosition);
        int iType = 0;
        if (obj.transform.localPosition.y <= m_iGuideLine + 150f && obj.transform.localPosition.y >= m_iGuideLine - 150f)
            iType = 2;
        else if ((obj.transform.localPosition.y > m_iGuideLine + 150f && obj.transform.localPosition.y < m_iGuideLine + 400f) ||
                 (m_iGuideLine - 150f > obj.transform.localPosition.y && obj.transform.localPosition.y > m_iGuideLine - 300f))
            iType = 1;
        else
            iType = 0;

        TweenScale pTweenScale = null;
        TweenAlpha pTweenAlpha = null;
        GameObject objEff = null;
        Vector3 Pos = obj.transform.localPosition;

        // 계산
        if (iType == 0)
        {
            m_iTotalScore += 50;
            m_pBad.SetActive(true);
            pTweenScale = m_pBad.GetComponent<TweenScale>();
            pTweenAlpha = m_pBad.GetComponent<TweenAlpha>();
            m_AudioVoice.PlayOneShot(m_AudioClipList[2]);

            if (m_fFailedTime <= 0f)
            {
                if (m_fPlayTime + m_fMusicTime < Time.time)
                {
                    // skip
                }
                else
                {
                    m_fFailedTime = 0.9f;
                    int iRnd = UnityEngine.Random.Range(0, 2);
                    if (iRnd == 0)
                    {
                        m_pAnimator.CrossFadeInFixedTime("18_mike_lose", 0.1f);
                        m_pAnimator_tear_L.gameObject.SetActive(true);
                        m_pAnimator_tear_R.gameObject.SetActive(true);
                        m_pAnimator_tear_L.CrossFadeInFixedTime("Tear", 0.0f, 0, 0f);
                        m_pAnimator_tear_R.CrossFadeInFixedTime("Tear", 0.0f, 0, 0f);
                    }
                    else
                    {
                        m_pAnimator.CrossFadeInFixedTime("17_mike_lose_fail", 0.1f);
                    }
                }
            }

            objEff = GameObject.Instantiate(m_EffetList[1]);
        }
        else if (iType == 1)
        {
            m_iTotalScore += 100;
            m_pGood.SetActive(true);
            pTweenScale = m_pGood.GetComponent<TweenScale>();
            pTweenAlpha = m_pGood.GetComponent<TweenAlpha>();
            m_AudioVoice.PlayOneShot(m_AudioClipList[0]);
            objEff = GameObject.Instantiate(m_EffetList[0]);
        }
        else if (iType == 2)
        {
            m_iTotalScore += 150;
            m_pExcellent.SetActive(true);
            pTweenScale = m_pExcellent.GetComponent<TweenScale>();
            pTweenAlpha = m_pExcellent.GetComponent<TweenAlpha>();
            m_AudioVoice.PlayOneShot(m_AudioClipList[1]);
            objEff = GameObject.Instantiate(m_EffetList[0]);
        }

        if (pTweenScale != null)
        {
            pTweenScale.enabled = true;
            pTweenScale.ResetToBeginning();
        }

        if (pTweenAlpha != null)
        {
            pTweenAlpha.enabled = true;
            pTweenAlpha.ResetToBeginning();
        }

        if (objEff != null)
        {
            objEff.transform.parent = m_pBubbleCreator.transform;
            objEff.transform.localScale = Vector3.one;
            objEff.transform.localPosition = Pos;
            objEff.SetActive(true);
        }

        // 스코어
        m_pUILabel_Score.text = string.Format("{0:#,###,###,##0}", m_iTotalScore);
        m_pUILabel_Score.GetComponent<TweenScale>().enabled = true;
        m_pUILabel_Score.GetComponent<TweenScale>().ResetToBeginning();

        Destroy(obj);
    }

    //============================================================================================
    // ClickBubble
    // - 
    //============================================================================================
    public void RetryGame()
    {
        m_pGameRetry.SetActive(false);
        m_pFinish.SetActive(false);
        m_pGuideLine.SetActive(true);
        m_pBubbleCreator.SetStop();

        m_pAnimator.CrossFadeInFixedTime("Idle", 0.1f);
        m_AudioVoice.Stop();

        m_iTotalScore = 0;
        m_pUILabel_Score.text = "0";

        m_fReadyTime = 3f;
        m_fPlayTime = 0f;
        m_fFailedTime = 0f;

        TweenAlpha pAlpha = m_pReady_3.GetComponent<TweenAlpha>();
        if (pAlpha != null)
        {
            pAlpha.enabled = true;
            pAlpha.ResetToBeginning();
        }

        TweenScale pScale = m_pReady_3.GetComponent<TweenScale>();
        if (pScale != null)
        {
            pScale.enabled = true;
            pScale.ResetToBeginning();
        }

        m_pReady_3.SetActive(true);
        m_AudioVoice.PlayOneShot(m_AudioClipList[0]);
    }

    //============================================================================================
    // ReturnContents
    // - 
    //============================================================================================
    public void ReturnContents()
    {
        m_pBubbleCreator.SetStop();
        m_objUI_QuitGame.SetActive(false);
        m_pGameRetry.SetActive(false);
        m_objUI_Game.SetActive(false);
        m_pFinish.SetActive(false);
        m_objUI_Contents.SetActive(true);

        m_pAnimator.CrossFadeInFixedTime("Idle", 0.1f);
        m_AudioVoice.Stop();

        m_bPlayGame = false;
        m_fReadyTime = 0f;
        m_fPlayTime = 0f;
        m_fFailedTime = 0f;
        m_iTotalScore = 0;
        m_pUILabel_Score.text = "0";

        ChaeckVoiceCommand("멈춰");
    }

    //============================================================================================
    // ClickOpenQuitGamePopup
    // - 
    //============================================================================================
    public void ClickOpenQuitGamePopup()
    {
        m_objUI_QuitGame.SetActive(true);
        TweenScale pScale = m_objUI_QuitGame.GetComponent<TweenScale>();
        pScale.enabled = true;
        pScale.ResetToBeginning();
    }

    //============================================================================================
    // CloseQuitGamePopup
    // - 
    //============================================================================================
    public void CloseQuitGamePopup()
    {
        m_objUI_QuitGame.SetActive(false);
    }

    //============================================================================================
    // ClickOpenShop
    // - 
    //============================================================================================
    public void ClickOpenShop()
    {
        m_objUI_Shop.SetActive(true);
    }

    //============================================================================================
    // CloseShop
    // - 
    //============================================================================================
    public void CloseShop()
    {
        m_objUI_Shop.SetActive(false);
    }





    //============================================================================================
    // 
    //
    //==   아래는 미사용   =======================================================================
    //
    //

    //============================================================================================
    // onReadyForSpeech
    // - 
    //============================================================================================
    void onReadyForSpeech(string _params)
    {
        Debug.Log("onReadyForSpeech : " + _params);
    }

    //============================================================================================
    // OnEndOfSpeech
    // - 미사용
    //============================================================================================
    void OnEndOfSpeech()
    {
        Debug.Log("OnEndOfSpeech");
    }

    //============================================================================================
    // OnBeginningOfSpeech
    // - 미사용
    //============================================================================================
    void OnBeginningOfSpeech()
    {
        Debug.Log("OnBeginningOfSpeech");
    }

    //============================================================================================
    // TextToSpeech_GetVoicesSuccessEvent
    // - 미사용
    //============================================================================================
    private void TextToSpeech_GetVoicesSuccessEvent(GetVoicesResponse response)
    {
        Debug.Log("TextToSpeech_GetVoicesSuccessEvent");
    }

    //============================================================================================
    // TextToSpeech_GetVoicesFailedEvent
    // - 미사용
    //============================================================================================
    private void TextToSpeech_GetVoicesFailedEvent(string error)
    {
        Debug.Log("TextToSpeech_GetVoicesFailedEvent : " + error);
    }

    //============================================================================================
    // TextToSpeech_SynthesizeFailedEvent
    // - 미사용
    //============================================================================================
    private void TextToSpeech_SynthesizeFailedEvent(string error)
    {
        Debug.Log("TextToSpeech_SynthesizeFailedEvent : " + error);
    }

}