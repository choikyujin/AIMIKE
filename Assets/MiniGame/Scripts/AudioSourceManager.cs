using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSourceManager : MonoBehaviour
{
    public List<AudioSource> audioSources;
    public static AudioSourceManager it;
    void Start()
    {
        it = this;
    }
    //현재는 22개 오디오소스가 붙어있고 최대 22개의 중첩소리를 표시할수있다. 그 이상이면 앞에꺼 대신에 넣는다.
    public void PlaySound(AudioClip audioClip, float volume = 1)
    {
        //MM.m("audioClip", audioClip);
        for (int i = 0; i < audioSources.Count; ++i)
        {
            if(audioSources[i].clip == null)
            {
                audioSources[i].clip = audioClip;
                audioSources[i].volume = volume;
               audioSources[i].Play();
                StartCoroutine(ClipPlayDuration(audioSources[i]));
                break;
            }
            
        }
    }

    IEnumerator ClipPlayDuration(AudioSource audioSource)
    {
        //MM.m("audioSource.clip.length", audioSource.clip.length);
        yield return new WaitForSeconds(audioSource.clip.length);
        audioSource.clip = null;
    }


}
