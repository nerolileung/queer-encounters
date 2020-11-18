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

    public void ChangeMusicVol(int value){
        mixer.SetFloat("volMusic",(float)value);
        musicText.text = (value+80).ToString()+"%";
    }
}
