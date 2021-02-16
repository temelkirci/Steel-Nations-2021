using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;
    public static AudioManager Instance
    {
        get { return instance; }
    }

    public AudioSource audioSource;
    public Sounds[] audioList;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    public void PlayVoice(VOICE_TYPE voiceType, bool isLoop = false)
    {

        if (isLoop)
        {
            audioSource.loop = true;
            audioSource.PlayOneShot(GetVoice(voiceType));
        }
        else
        {
            audioSource.loop = false;
            audioSource.PlayOneShot(GetVoice(voiceType));
        }
    }

    public AudioClip GetVoice(VOICE_TYPE voiceType)
    {
        foreach (Sounds voice in audioList)
        {
            if (voice.voiceType == voiceType)
                return voice.audio;
        }

        return null;
    }
}
