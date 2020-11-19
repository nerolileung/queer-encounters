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

    private AudioClip[] musicFiles = new AudioClip[2];
    private float musicVolOrig;
    private float musicTimerCurrent;
    private static float musicTimerMax;
    private AudioClip musicCurrent;

    // Start is called before the first frame update
    void Start()
    {
        mixer = Resources.Load<AudioMixer>("Audio/MainMixer");
        musicFiles[0] = Resources.Load<AudioClip>("Audio/RelaxingPianoMusic");
        musicFiles[1] = Resources.Load<AudioClip>("Audio/UnnaturalSituation");
        musicTimerMax = 2f;
        musicTimerCurrent = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (musicTimerCurrent > 0) {
            musicTimerCurrent -= Time.deltaTime;

        }
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

    public void PlayMusic(string clipName){
        int index = musicFiles.Length;
        switch (clipName){
            case "RelaxingPianoMusic":
                index = 0;
            break;
            case "UnnaturalSituation":
                index = 1;
            break;
            default:
                Debug.Log("Unknown bgm file: "+clipName);
            break;
        }

        if (index < musicFiles.Length){
            musicCurrent = musicFiles[index];
            if (musicSource.isPlaying){
                musicTimerCurrent = musicTimerMax;
                mixer.GetFloat("volBGM", out musicVolOrig);
            }
            else {
                musicSource.clip = musicCurrent;
                musicSource.Play();
            }
        }

    }
    public void PlayVoice(AudioClip clip){
        voiceSource.clip = clip;
        if (!voiceSource.isPlaying)
            voiceSource.Play();
    }
    public bool isVoicePlaying(){
        return voiceSource.isPlaying;
    }
    public void StopVoice(){
        voiceSource.Stop();
    }
}