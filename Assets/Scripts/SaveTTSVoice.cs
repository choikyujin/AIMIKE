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

public class SaveTTSVoice : MonoBehaviour
{
    public string m_sAPIKey = "AIzaSyCHn96RFsN2uZhZxxUWZNuaeS-V-3HrX30";
    public GCTextToSpeech m_GCTTS = null;
    public ArrayList m_SoundList = new ArrayList();
    public List<string> m_TextList = new List<string>();

    //============================================================================================
    // Start
    // -
    //============================================================================================
    void Start()
    {
        
    }

    //============================================================================================
    // Update
    // -
    //============================================================================================
    void Update()
    {
            
    }
}
