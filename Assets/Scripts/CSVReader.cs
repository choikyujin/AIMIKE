using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public class CSVReader
{
    public string m_FilePath = "";
	static CSVReader Instance = null;
	static readonly object padlock = new object();
	
	public static CSVReader GetInstance
	{
		get
		{
			lock( padlock )
			{
				if( null == Instance )
				{
					Instance = new CSVReader();
					Instance.Init();
				}
				return Instance;
			}
		}
	}
	
	//============================================================================================
	// Init()
	// 
	//============================================================================================
	public void Init()
	{
		m_FilePath = Application.persistentDataPath;
	}

	//============================================================================================
	// CreateLog()
	// 
	//============================================================================================
	public void CreateLog( string sData, string FileName)
	{
		//파일을 열고 없으면 생성
        string sPath = m_FilePath + "/" + FileName;

        //추가모드로 열기 없으면 생성까지..
		FileStream stream = new FileStream( sPath, FileMode.Append ); 
		StreamWriter fleWrite = new StreamWriter(stream);

		//한줄단위로 쓴다. 다음에 이어서 쓸때 다음줄 부터 써진다.
		fleWrite.WriteLine(sData);  
		fleWrite.Flush(); 
		fleWrite.Close();
		stream.Close();
	}
	
	//============================================================================================
	// CreateText()
	// 
	//============================================================================================
	public void CreateText( ArrayList kArray, string FileName)
	{
        string sPath = m_FilePath + "/" + FileName;

        Debug.Log("CreateText-" + sPath);
		StreamWriter sw = new StreamWriter(sPath , false, System.Text.Encoding.UTF8 );
	
		foreach (string sData in kArray)
		{
			if(sData != "")
				sw.WriteLine(sData);
		}
		
		sw.Flush(); 
		sw.Close();
	}
	
	//============================================================================================
	// LoadText()
	// 
	//============================================================================================
	public ArrayList LoadText( string FileName)
	{
		string sData = "";
		string sPath = m_FilePath + "/" + FileName;
			
		ArrayList arrList = new ArrayList();
	
        if(!File.Exists( sPath ))
            return arrList;
		
		StreamReader sr = new StreamReader( sPath, System.Text.Encoding.UTF8 );
		while ((sData = sr.ReadLine()) != null) 
		{
			arrList.Add(sData);
		}
		
		sr.Close(); 
		return arrList;
	}

    //============================================================================================
    // CSVLineInfo()  
    // - 주로 CSV 로드에서 사용. 텍스트로 구성된 문자열을 Arraylist로 변형해서 리턴한다. 
    //============================================================================================
    public ArrayList CSVLineInfo(string t)
    {
        string[] lineTemp = t.Split("\n"[0]);
        ArrayList valueArray = new ArrayList();
        foreach (string element in lineTemp)
        {
            string key_value = element;
            valueArray.Add(key_value);
        }
        return valueArray;
    }

    //============================================================================================
    // CSVLineInfoList()  
    // -
    //============================================================================================
    public List<string> CSVLineInfoList(string t)
    {
        string[] lineTemp = t.Split("\n"[0]);
        List<string> valueArray = new List<string>();
        foreach (string element in lineTemp)
        {   
            valueArray.Add(element.Replace("\r", ""));
        }
        return valueArray;
    }
}
