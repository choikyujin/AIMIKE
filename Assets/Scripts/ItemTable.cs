using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class ItemTableData
{
    public int ID = 0;
    public string Model = "";
    public int Type = 0;
    public Vector3 Pos = Vector3.zero;
    public Quaternion Rot = Quaternion.identity;
    public Vector3 Scale = Vector3.zero;
    public int HideEar = 0;
    public string MatAlpha = "";
}

public class ItemTable
{
    static ItemTable Instance = null;
    static readonly object padlock = new object();

    //======================================================================
    // GetInstance  
    // - 
    //======================================================================
    public static ItemTable GetInstance
    {
        get
        {
            lock (padlock)
            {
                if (null == Instance)
                    Instance = new ItemTable();
                return Instance;
            }
        }
    }

    public Dictionary<int, ItemTableData> m_DataList = null;

    //============================================================================================
    // LoadTable()
    // - 테이블 로드
    //============================================================================================
    public void LoadTable()
    {
        if (m_DataList == null)
            m_DataList = new Dictionary<int, ItemTableData>();
        m_DataList.Clear();

        // TODO : 추후 에셋번들에서 로드
        TextAsset pText = (TextAsset)Resources.Load("Table/ItemTable");
        string s = pText.text;
        ArrayList arrList = CSVReader.GetInstance.CSVLineInfo(s);

        float x, y, z;
        int index = 0;
        ItemTableData pData = null;

        for (int i = 1; i < arrList.Count; i++)
        {
            s = (string)arrList[i];
            s = s.Trim();
            if (s.Length < 3)
                continue;

            pData = new ItemTableData();
            string[] words = s.Split(',');
            index = 0;

            pData.ID = Convert.ToInt32(words[index]);
            index++;
            pData.Model = words[index];
            index++;
            pData.Type = Convert.ToInt32(words[index]);
            index++;

            string[] words2 = words[index].Split('_');
            x = (float)Convert.ToDouble(words2[0]);
            y = (float)Convert.ToDouble(words2[1]);
            z = (float)Convert.ToDouble(words2[2]);
            pData.Pos = new Vector3(x, y, z);
            index++;

            words2 = words[index].Split('_');
            x = (float)Convert.ToDouble(words2[0]);
            y = (float)Convert.ToDouble(words2[1]);
            z = (float)Convert.ToDouble(words2[2]);
            pData.Rot = Quaternion.Euler(x, y, z);
            index++;

            words2 = words[index].Split('_');
            x = (float)Convert.ToDouble(words2[0]);
            y = (float)Convert.ToDouble(words2[1]);
            z = (float)Convert.ToDouble(words2[2]);
            pData.Scale = new Vector3(x, y, z);
            index++;

            pData.HideEar = Convert.ToInt32(words[index]);
            index++;

            pData.MatAlpha = words[index];
            index++;

            m_DataList.Add(pData.ID, pData);
        }
    }
}