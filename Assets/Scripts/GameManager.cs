using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    private static STATE gameState;
    enum STATE {
        MENU, DIALOGUE, CHOICE, TRANS_OUT, TRANS_IN, ENDING
    }
    private AudioManager audioManager;
    private List<Passage> passageBuffer = new List<Passage>();
    [SerializeField]
    private Text dialogue; 
    #region timers
    [SerializeField]
    private GameObject choiceTimerUI;
    private Text choiceTimerText;
    private float choiceTimerCurrent;
    private static float choiceTimerMax;
    private float fadeTimerCurrent;
    private static float fadeTimerMax;
    #endregion
    #region ui connections
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
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        audioManager = Object.FindObjectOfType<AudioManager>();

        quitButton.onClick.AddListener(Application.Quit);

        choiceTimerMax = 5.0f;
        choiceTimerCurrent = 0f;
        choiceTimerText = choiceTimerUI.GetComponentInChildren<Text>();

        fadeTimerMax = 1.0f;
        fadeTimerCurrent = 0f;
        
        gameState = STATE.MENU;
    }

    // Update is called once per frame
    void Update()
    {
        // return to main menu using esc
        if (Input.GetKeyDown(KeyCode.Escape) && gameState != STATE.MENU){
            ChangeState(STATE.MENU);
        }

        switch (gameState){
            case STATE.DIALOGUE:
                // has passage finished?
                if (!audioManager.isVoicePlaying()){
                    if (passageBuffer[0].next2 != null) // choice available!
                        ChangeState(STATE.CHOICE);
                    else if (passageBuffer[0].next1 != null) // go to next passage
                        EndPassage(true);
                    else // this is an ending!
                        ChangeState(STATE.ENDING);
                }
            break;
            case STATE.CHOICE:
                ChoiceTimerTick();
            break;
            case STATE.TRANS_IN:
            case STATE.TRANS_OUT:
                // passage transition out/in
                FadeTimerTick();
            break;
        }
    }

    public void Init(){
        passageBuffer.Clear();
        LoadPassage("Explanation");
        LoadPassage(passageBuffer[0].next1.name);
        LoadPassage(passageBuffer[0].next2.name);
        PlayPassage();
        ChangeState(STATE.DIALOGUE);
    }
    #region passage actions
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
        if (passageBuffer[0].next1 != null) LoadPassage(passageBuffer[0].next1.name);
        
        // fade out current text
        ChangeState(STATE.TRANS_OUT);
        
    }
    public void SkipPassage(){
        if (gameState == STATE.CHOICE) {
            EndPassage(true);
        }
        else audioManager.StopVoice(); // go to end of dialogue logic
        EventSystem.current.SetSelectedGameObject(null);
    }
    #endregion

    private void ChoiceTimerTick(){
        choiceTimerCurrent -= Time.deltaTime;
        choiceTimerUI.GetComponent<Image>().fillAmount = choiceTimerCurrent/choiceTimerMax;
        choiceTimerText.text = Mathf.CeilToInt(choiceTimerCurrent).ToString();

        if (Input.anyKeyDown && (!Input.GetMouseButtonDown(0) && !Input.GetMouseButtonDown(1) && !Input.GetMouseButtonDown(2))) {
            EndPassage(false);
        }
        if (choiceTimerCurrent <= 0){
            EndPassage(true);
        }
    }
    private void FadeTimerTick(){
        fadeTimerCurrent -= Time.deltaTime;
        Color fadeColour = dialogue.color;
        if (gameState==STATE.TRANS_OUT){
            fadeColour.a = Mathf.Lerp(0,1,fadeTimerCurrent/fadeTimerMax);
            if (fadeTimerCurrent <= 0) ChangeState(STATE.TRANS_IN);
        }
        else {
            fadeColour.a = Mathf.Lerp(1,0,fadeTimerCurrent/fadeTimerMax);
            if (fadeTimerCurrent <= 0) ChangeState(STATE.DIALOGUE);
        }
        dialogue.color = fadeColour;
    }

    public void GameToMenu(){
        ChangeState(STATE.MENU);
    }
    private void ChangeState(STATE state){
        switch (state){
            case STATE.MENU:
                audioManager.StopVoice();
                // audioManager.playMusic("menuMusic");
                gameCanvas.SetActive(false);
                mainMenu.SetActive(true);
            break;
            case STATE.DIALOGUE:
                choiceTimerUI.SetActive(false);
                skipButton.SetActive(true);
                menuButton.SetActive(false);
            break;
            case STATE.CHOICE:
                choiceTimerUI.SetActive(true);
                choiceTimerCurrent = choiceTimerMax;
            break;
            case STATE.TRANS_OUT:
                if (gameState == STATE.CHOICE){
                    choiceTimerUI.SetActive(false);
                }
                skipButton.SetActive(false);
                fadeTimerCurrent = fadeTimerMax;
            break;
            case STATE.TRANS_IN:
                PlayPassage();
                fadeTimerCurrent = fadeTimerMax;
            break;
            case STATE.ENDING:
                skipButton.SetActive(false);
                menuButton.SetActive(true);
            break;
        }
        gameState = state;
    }
}
