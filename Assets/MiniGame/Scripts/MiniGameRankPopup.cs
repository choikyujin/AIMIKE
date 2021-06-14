using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MiniGameRankPopup : MonoBehaviour
{
    public static MiniGameRankPopup instance;
    public MiniGameRank[] miniGameRanks;
    public Sprite listBg_me;
    public Sprite listBg_other;
    public Sprite[] medalSprites;
    public GameObject animObj;
    public void Active(string gameName, int myScore)
    {
        AudioSourceManager.it.PlaySound(AudioClipManager.it.resultPopup);
        MM.OpenScale(animObj);
        //int myScore = 0;
        int myIndex = 0;
        List<int> aa = new List<int>();
        for (int i = 0; i < miniGameRanks.Length; ++i)
        {
            aa.Add(PlayerPrefs.GetInt(gameName + i, 0));
        }
        for (int i = 0; i < miniGameRanks.Length; ++i)
        {
            if (myScore > aa[i])
            {
                aa.Insert(i, myScore);
                myIndex = i;
                break;
            }
        }
        for (int i = 0; i < miniGameRanks.Length; ++i)
        {
            PlayerPrefs.SetInt(gameName + i, aa[i]);
            miniGameRanks[i].SetScoreText(aa[i], myIndex == i, i);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        animObj.SetActive(false);
    }
    public void BtnOutGame()
    {
        SceneManager.LoadScene("Mike_AI");
    }
    public void BtnReGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
