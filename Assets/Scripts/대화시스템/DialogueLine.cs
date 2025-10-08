using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    public string speakerName;
    public string dialogueText;
    public List<string> characterNames;
    public List<Sprite> characterSprites;
    public Sprite backgroundSprite;

    public List<string> choices;
    public List<int> nextDialogueIndexes;

    public bool isFinalLine; // 마지막 줄 여부 직접 표시
}

