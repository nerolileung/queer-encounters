using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    private static AudioMixer mixer;

    [SerializeField]
    private AudioSource voiceSource;
    [SerializeField]
    private Text voiceText;
    [SerializeField]
    private AudioSource sfxSource;
    [SerializeField]
    private Text sfxText;
    [SerializeField]
    private AudioSource musicSource;
    [SerializeField]
    private Text musicText;

    // Start is called before the first frame update
    void Start()
    {
        mixer = Resources.Load<AudioMixer>("Audio/MainMixer");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeMusicVol(float value){
        mixer.SetFloat("volBGM",value);
        musicText.text = Mathf.RoundToInt(value+80).ToString()+"%";
    }
    public void ChangeSFXVol(float value){
        mixer.SetFloat("volSFX",value);
        sfxText.text = Mathf.RoundToInt(value+80).ToString()+"%";
    }
    public void ChangeVoiceVol(float value){
        mixer.SetFloat("volVoice",value);
        voiceText.text = Mathf.RoundToInt(value+80).ToString()+"%";
    }
}
