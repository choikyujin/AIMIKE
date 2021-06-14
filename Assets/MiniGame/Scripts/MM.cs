using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class MM : MonoBehaviour
{
    static string normalColor = "<color=#636363>";
    public static void m(/*LogKInd logKInd,*/ params object[] args)
    {
        string textcolor = normalColor;  //글자 색깔지정
        StringBuilder stringBuilder1 = new StringBuilder();

        stringBuilder1.Append("<color=#505050>");
        stringBuilder1.Append(string.Format("[{0:n2}]", Time.realtimeSinceStartup));
        stringBuilder1.Append("</color>");

        for (int i = 0; i < args.Length; ++i)
        {

            if (i % 2 == 0)
            {
                stringBuilder1.Append(textcolor);
                if (i == 0)
                {
                    stringBuilder1.Append("▶ ");
                }
                stringBuilder1.Append(args[i]?.ToString());
                stringBuilder1.Append(" : ");
                stringBuilder1.Append("</color>");
            }
            else
            {
                stringBuilder1.Append(args[i]?.ToString());
                if (i != args.Length - 1)
                {
                    stringBuilder1.Append(textcolor);
                    stringBuilder1.Append(" ,");


                    stringBuilder1.Append("</color>");
                }
            }
        }
        UnityEngine.Debug.Log(stringBuilder1.ToString());
    }

    public static void OpenScale(GameObject obj)
    {
        obj.SetActive(true);
        obj.transform.localScale = Vector3.one;
        LeanTween.scale(obj, new Vector3(1.1f, 1.1f, 1), 0.12f).setEase(LeanTweenType.easeOutQuad).setLoopPingPong(1);
    }
    public static void CloseScale(GameObject obj)
    {
        obj.transform.localScale = Vector3.one;
        LeanTween.scale(obj, new Vector3(0.5f, 0.5f, 1), 0.12f).setEase(LeanTweenType.easeOutQuad).setOnComplete(() =>
        {
            obj.SetActive(false);
        });
    }
}
