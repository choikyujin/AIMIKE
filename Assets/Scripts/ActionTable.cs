using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class ActionTableData
{
    public int Action = 0;
    public int IgnoreAction = 0;
    public float ActionTime = 0f;
    public int Type = 0;
    public int RndCnt = 0;
    public string[] ActionTalk = new string[3];
    public string[] ActionAnim = new string[3];
    public string[] Sound = new string[3];
}

public class ActionTable
{
    static ActionTable Instance = null;
    static readonly object padlock = new object();

    //======================================================================
    // GetInstance  
    // - 
    //======================================================================
    public static ActionTable GetInstance
    {
        get
        {
            lock (padlock)
            {
                if (null == Instance)
                    Instance = new ActionTable();
                return Instance;
            }
        }
    }

    public Dictionary<int, ActionTableData> m_DataList = null;

    //============================================================================================
    // LoadTable()
    // - 테이블 로드
    //============================================================================================
    public void LoadTable()
    {
        if (m_DataList == null)
            m_DataList = new Dictionary<int, ActionTableData>();
        m_DataList.Clear();

        // TODO : 추후 에셋번들에서 로드
        TextAsset pText = (TextAsset)Resources.Load("Table/ActionTable");
        string s = pText.text;
        ArrayList arrList = CSVReader.GetInstance.CSVLineInfo(s);

        int index = 0;
        ActionTableData pData = null;

        for (int i = 1; i < arrList.Count; i++)
        {
            s = (string)arrList[i];
            s = s.Trim();
            if (s.Length < 3)
                continue;

            pData = new ActionTableData();
            string[] words = s.Split(',');
            index = 0;

            pData.Action = Convert.ToInt32(words[index]);
            index++;
            pData.IgnoreAction = Convert.ToInt32(words[index]);
            index++;
            pData.ActionTime = (float)Convert.ToDouble(words[index]);
            index++;
            pData.Type = Convert.ToInt32(words[index]);
            index++;
            pData.RndCnt = Convert.ToInt32(words[index]);
            index++;
            pData.ActionTalk[0] = words[index];
            index++;
            pData.ActionTalk[1] = words[index];
            index++;
            pData.ActionTalk[2] = words[index];
            index++;
            pData.ActionAnim[0] = words[index];
            index++;
            pData.ActionAnim[1] = words[index];
            index++;
            pData.ActionAnim[2] = words[index];
            index++;
            pData.Sound[0] = words[index];
            index++;
            pData.Sound[1] = words[index];
            index++;
            pData.Sound[2] = words[index];
            index++;

            m_DataList.Add(pData.Action, pData);
        }
    }

    //============================================================================================
    // FindData
    // -
    //============================================================================================
    public ActionTableData FindData(int DoAction, int CurAction)
    {
        ActionTableData pData = null;
        if (m_DataList.TryGetValue(DoAction, out pData))
        {
            if (pData.IgnoreAction > 0 && pData.IgnoreAction == CurAction)
                return null;

            return pData;
        }

        return null;
    }
}