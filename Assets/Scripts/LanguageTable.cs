using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class LanguageTableData
{
    public int Num = 0;
    public string[] Talk = new string[4];
}

public class LanguageTable
{
    static LanguageTable Instance = null;
    static readonly object padlock = new object();

    //======================================================================
    // GetInstance  
    // - 
    //======================================================================
    public static LanguageTable GetInstance
    {
        get
        {
            lock (padlock)
            {
                if (null == Instance)
                    Instance = new LanguageTable();
                return Instance;
            }
        }
    }

    public Dictionary<int, LanguageTableData> m_DataList = null;

    //============================================================================================
    // LoadTable()
    // - 테이블 로드
    //============================================================================================
    public void LoadTable()
    {
        if (m_DataList == null)
            m_DataList = new Dictionary<int, LanguageTableData>();
        m_DataList.Clear();

        // TODO : 추후 에셋번들에서 로드
        TextAsset pText = (TextAsset)Resources.Load("Table/LanguageTable");
        string s = pText.text;
        ArrayList arrList = CSVReader.GetInstance.CSVLineInfo(s);

        int index = 0;
        LanguageTableData pData = null;

        for (int i = 1; i < arrList.Count; i++)
        {
            s = (string)arrList[i];
            s = s.Trim();
            if (s.Length < 3)
                continue;

            pData = new LanguageTableData();
            string[] words = s.Split(',');
            index = 0;


            Debug.Log(s);


            pData.Num = Convert.ToInt32(words[index]);
            index++;
            pData.Talk[0] = words[index];
            index++;
            pData.Talk[1] = words[index];
            index++;
            pData.Talk[2] = words[index];
            index++;
            pData.Talk[3] = words[index];
            index++;

            m_DataList.Add(pData.Num, pData);
        }
    }

    //============================================================================================
    // FindData
    // -
    //============================================================================================
    public string FindData(int DoAction, string sLanguage)
    {
        LanguageTableData pData = null;
        if (m_DataList.TryGetValue(DoAction, out pData))
        {
            if(sLanguage == "KOR")
                return pData.Talk[0];
            else if (sLanguage == "ENG")
                return pData.Talk[1];
            if (sLanguage == "CHN")
                return pData.Talk[2];
            if (sLanguage == "JPN")
                return pData.Talk[3];
        }
        return "";
    }

    //============================================================================================
    // FindData
    // -
    //============================================================================================
    public string FindData(string DoAction, string sLanguage)
    {
        int iDoAction = Convert.ToInt32(DoAction);
        LanguageTableData pData = null;
        if (m_DataList.TryGetValue(iDoAction, out pData))
        {
            if (sLanguage == "KOR")
                return pData.Talk[0];
            else if (sLanguage == "ENG")
                return pData.Talk[1];
            if (sLanguage == "CHN")
                return pData.Talk[2];
            if (sLanguage == "JPN")
                return pData.Talk[3];
        }
        return "";
    }
}