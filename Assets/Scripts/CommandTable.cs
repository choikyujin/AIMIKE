using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class CommandTableData
{
    public int Action = 0;
    public string[] CheckVoice = new string[3];
}

public class CommandTable
{
    static CommandTable Instance = null;
    static readonly object padlock = new object();

    //======================================================================
    // GetInstance  
    // - 
    //======================================================================
    public static CommandTable GetInstance
    {
        get
        {
            lock (padlock)
            {
                if (null == Instance)
                    Instance = new CommandTable();
                return Instance;
            }
        }
    }

    public ArrayList m_DataList = null;

    //============================================================================================
    // LoadTable()
    // - 테이블 로드
    //============================================================================================
    public void LoadTable()
    {
        if (m_DataList == null)
            m_DataList = new ArrayList();
        m_DataList.Clear();

        // TODO : 추후 에셋번들에서 로드
        TextAsset pText = (TextAsset)Resources.Load("Table/CommandTable");
        string s = pText.text;
        ArrayList arrList = CSVReader.GetInstance.CSVLineInfo(s);

        int index = 0;
        CommandTableData pData = null;

        for (int i = 1; i < arrList.Count; i++)
        {
            s = (string)arrList[i];
            s = s.Trim();
            if (s.Length < 3)
                continue;

            pData = new CommandTableData();
            string[] words = s.Split(',');
            index = 0;

            pData.Action = Convert.ToInt32(words[index]);
            index++;

            pData.CheckVoice[0] = words[index];
            index++;
            pData.CheckVoice[1] = words[index];
            index++;
            pData.CheckVoice[2] = words[index];
            index++;

            m_DataList.Add(pData);
        }
    }

    //============================================================================================
    // FindData
    // -
    //============================================================================================
    public CommandTableData FindData(string sInput)
    {
        CommandTableData pData = null;

        for(int i=0; i< m_DataList.Count; i++)
        {
            pData = (CommandTableData)m_DataList[i];
            if (sInput.Contains(pData.CheckVoice[0]) && sInput.Contains(pData.CheckVoice[1]) && sInput.Contains(pData.CheckVoice[2]))
                return pData;
        }

        return null;
    }
}