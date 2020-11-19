using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "Data", menuName = "Passage", order = 1)]
public class Passage : ScriptableObject
{
    [TextArea(3,10)]
    public string text;
    public AudioClip voice;
    public Passage next1;
    public Passage next2;
    public string music;
}
