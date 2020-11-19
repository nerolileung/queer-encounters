using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static bool gameActive;
    private List<Passage> passageBuffer = new List<Passage>();
    [SerializeField]
    private Text dialogue; 
    private AudioManager audioManager;

    [SerializeField]
    private GameObject choiceTimerUI;
    private Text choiceTimerText;
    private float choiceTimerCurrent;
    private static float choiceTimerMax;
    private float fadeTimerCurrent;
    private static float fadeTimerMax;
    private static bool fadeOut;

    [SerializeField]
    private GameObject skipButton;
    [SerializeField]
    private GameObject menuButton;
    [SerializeField]
    private Button quitButton;
    [SerializeField]
    private GameObject mainMenu;
    [SerializeField]
    private GameObject gameCanvas;

    // Start is called before the first frame update
    void Start()
    {
        audioManager = Object.FindObjectOfType<AudioManager>();

        quitButton.onClick.AddListener(Application.Quit);

        choiceTimerMax = 5.0f;
        choiceTimerCurrent = 0f;
        choiceTimerText = choiceTimerUI.GetComponentInChildren<Text>();
        choiceTimerUI.SetActive(false);

        fadeTimerMax = 1.0f;
        fadeTimerCurrent = 0f;
        fadeOut = true;

        gameActive = false;
    }

    // Update is called once per frame
    void Update()
    {
        // return to main menu using esc
        if (Input.GetKeyDown(KeyCode.Escape)){
            if (!mainMenu.activeInHierarchy){
                gameActive = false;
                audioManager.StopVoice();
                // audioManager.playMusic("menuMusic");
                gameCanvas.SetActive(false);
                mainMenu.SetActive(true);
            }
        }

        if (gameActive){
            // countdown
            if (choiceTimerCurrent > 0){
                choiceTimerCurrent -= Time.deltaTime;
                choiceTimerUI.GetComponent<Image>().fillAmount = choiceTimerCurrent/choiceTimerMax;
                choiceTimerText.text = Mathf.CeilToInt(choiceTimerCurrent).ToString();

                if (Input.anyKeyDown && passageBuffer[0].next2!=null){
                    // ignore mouse and escape
                    if (!Input.GetMouseButtonDown(0) && !Input.GetMouseButtonDown(1) && !Input.GetMouseButtonDown(2)) {
                            EndPassage(false);
                    }
                }
                
                if (choiceTimerCurrent <= 0){
                    choiceTimerUI.SetActive(false);
                    EndPassage(true);
                }
            }
            else {
                // has passage finished?
                if (!audioManager.isVoicePlaying()){
                    if (passageBuffer[0].next2 != null){
                        choiceTimerCurrent = choiceTimerMax;
                        choiceTimerUI.SetActive(true);
                    }
                    else if (passageBuffer[0].next1 != null){
                        EndPassage(true);
                    }
                    else {
                        // this is an ending!
                        choiceTimerCurrent = 0f;
                        skipButton.SetActive(false);
                        menuButton.SetActive(true);
                    }
                }
            }

            // passage transition out/in
            if (fadeTimerCurrent > 0){
                fadeTimerCurrent -= Time.deltaTime;
                Color fadeColour = dialogue.color;
                if (fadeOut){
                    fadeColour.a = Mathf.Lerp(0,1,fadeTimerCurrent/fadeTimerMax);
                    if (fadeTimerCurrent <= 0) {
                        PlayPassage();
                        
                        fadeColour.a = 1;
                        fadeOut = false;
                        fadeTimerCurrent = fadeTimerMax;
                    }
                }
                else fadeColour.a = Mathf.Lerp(1,0,fadeTimerCurrent/fadeTimerMax);
                
                dialogue.color = fadeColour;
            }
        }
    }

    public void Init(){
        passageBuffer.Clear();
        menuButton.SetActive(false);
        skipButton.SetActive(true);
        
        LoadPassage("Explanation");
        LoadPassage(passageBuffer[0].next1.name);
        LoadPassage(passageBuffer[0].next2.name);
        PlayPassage();
    }
    private void LoadPassage(string passName){
        for (int i = 0; i < passageBuffer.Count; i++){
            if (passageBuffer[i].name.Equals(passName)) return;
        }
        passageBuffer.Add(Resources.Load<Passage>("Passages/"+passName));
    }
    private void PlayPassage(){
        if (passageBuffer[0].music != "") audioManager.PlayMusic(passageBuffer[0].music);
        dialogue.text = passageBuffer[0].text;
        if (passageBuffer[0].voice) audioManager.PlayVoice(passageBuffer[0].voice);
    }
    private void EndPassage(bool passiveCont){
        Passage nextPassage;
        // remove unchosen passage from buffer
        if (passiveCont) {
            nextPassage = passageBuffer[0].next1;
            if (passageBuffer[0].next2!=null) passageBuffer.Remove(passageBuffer[0].next2);
        }
        else {
            nextPassage = passageBuffer[0].next2;
            if (passageBuffer[0].next1!=null)passageBuffer.Remove(passageBuffer[0].next1);
        }
        passageBuffer.RemoveAt(0);
        
        // nextPassage should be at index 0 of buffer
        if (passageBuffer[0].next2 != null) LoadPassage(passageBuffer[0].next2.name);
        if (passageBuffer[0].next1 != null) {
            LoadPassage(passageBuffer[0].next1.name);
            // fade out current text
                fadeTimerCurrent = fadeTimerMax;
                fadeOut = true;
        }
    }

    public void SkipPassage(){
        if (choiceTimerCurrent > 0) { // in countdown
            EndPassage(true);
        }
        else audioManager.StopVoice();
    }

    public void ToggleGame(){
        gameActive = !gameActive;
    }
}
