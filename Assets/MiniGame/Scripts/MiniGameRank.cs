using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniGameRank : MonoBehaviour
{
    //public Sprite[] medalSprits;
    public Text scoreText;
    public Image listBg;
    public Image[] medalImages;
    public Text rankText;
    public void SetScoreText(int score, bool isMine, int rank)
    {
        rankText.text = (rank + 1).ToString();
        if (score == 0)
        {
            for(int i = 0; i < medalImages.Length; ++i)
            {
                medalImages[i].gameObject.SetActive(false);
            }
            listBg.sprite = MiniGameRankPopup.instance.listBg_other;
            scoreText.text = "-";
            return;
        }

        if (isMine)
        {
            listBg.sprite = MiniGameRankPopup.instance.listBg_me;
        }
        else
        {
            listBg.sprite = MiniGameRankPopup.instance.listBg_other;
        }
        scoreText.text = score.ToString();

        if(rank < 3)
        {
            for (int i = 0; i < medalImages.Length; ++i)
            {
                medalImages[i].gameObject.SetActive(rank == i);
            }
            //medalImage.gameObject.SetActive(true);
            rankText.gameObject.SetActive(false);
            //medalImage.sprite = medalSprits[rank];/* MiniGameRankPopup.instance.medalSprites[rank];*/
        }
        else
        {
            for (int i = 0; i < medalImages.Length; ++i)
            {
                medalImages[i].gameObject.SetActive(false);
            }
            //medalImage.gameObject.SetActive(false);
            rankText.gameObject.SetActive(true);
           
            MM.m("rankText", rankText.text);
        }
    }
}
