using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    public List<string> characterNames = new List<string>();
    public List<Sprite> characterSprites = new List<Sprite>();
    public Sprite backgroundSprite;
    public string dialogueText;
    public List<string> choices = new List<string>();
    public List<int> nextDialogueIndexes = new List<int>();
    public string speakerName;
}