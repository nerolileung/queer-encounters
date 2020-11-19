using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PaletteSwap : MonoBehaviour
{
    private bool darkMode;
    private List<Text> uiText = new List<Text>();
    private List<Button> uiButton = new List<Button>();
    private List<Image> uiImage = new List<Image>();

    private static Color[] palette = new Color[4];
    private static ColorBlock blockLight;
    private static ColorBlock blockDark;

    [SerializeField]
    private GameObject settingsLabel;

    private AudioManager audioManager;

    // Start is called before the first frame update
    void Start()
    {
        darkMode = true;

        // light to dark
        palette[0] = new Color(0.9f,0.9f,0.9f);
        palette[1] = new Color(0.7f,0.7f,0.7f);
        palette[2] = new Color(0.2f,0.2f,0.2f);
        palette[3] = new Color(0.1f,0.1f,0.1f);
        // light colourblock
        blockLight.normalColor = palette[3];
        blockLight.highlightedColor = palette[0];
        blockLight.pressedColor = palette[1];
        blockLight.selectedColor = palette[2];
        blockLight.colorMultiplier = 1;
        // dark colourblock
        blockDark.normalColor = palette[0];
        blockDark.highlightedColor = palette[3];
        blockDark.pressedColor = palette[2];
        blockDark.selectedColor = palette[1];
        blockDark.colorMultiplier = 1;

        audioManager = audioManager = Object.FindObjectOfType<AudioManager>();

        //get all the text and buttons
        foreach (Text obj in Resources.FindObjectsOfTypeAll<Text>() as Text[])
            uiText.Add(obj);
        foreach (Button obj in Resources.FindObjectsOfTypeAll<Button>() as Button[]){
            uiButton.Add(obj);
            obj.onClick.AddListener(() => audioManager.PlayEffect("buttonsfx"));
        }
        foreach (Image obj in Resources.FindObjectsOfTypeAll<Image>() as Image[])
            uiImage.Add(obj);
    }
    public void SwapPalette(){
        if (darkMode){
            Camera.main.backgroundColor = palette[0];

            // text colour
            for (int i = 0; i < uiText.Count; i++)
                uiText[i].color = palette[3];
            // image
            for (int i = 0; i < uiImage.Count; i++){
                if (uiImage[i].color == palette[0]) // button border
                    uiImage[i].color = palette[3];
                else if (uiImage[i].color == palette[2]) // slider fill
                    uiImage[i].color = palette[1];
            }
            // button border
            for (int i = 0; i < uiButton.Count; i++)
                uiButton[i].colors = blockLight;

            settingsLabel.GetComponent<Text>().text = "Light";
            darkMode = false;
        }
        else {
            Camera.main.backgroundColor = palette[3];

            // text colour
            for (int i = 0; i < uiText.Count; i++)
                uiText[i].color = palette[0];
            // image
            for (int i = 0; i < uiImage.Count; i++){
                if (uiImage[i].color == palette[3]) // button border
                    uiImage[i].color = palette[0];
                else if (uiImage[i].color == palette[1]) // slider fill
                    uiImage[i].color = palette[2];
            }
            // button border
            for (int i = 0; i < uiButton.Count; i++)
                uiButton[i].colors = blockDark;

            settingsLabel.GetComponent<Text>().text = "Dark";
            darkMode = true;
        }
        settingsLabel.GetComponent<Text>().text += " Mode";
    }
}
