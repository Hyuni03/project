using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    public List<string> characterNames;       // 화면에 등장하는 캐릭터들
    public List<Sprite> characterSprites;     // 등장 캐릭터 이미지
    public Sprite backgroundSprite;
    public string dialogueText;

    public string speakingCharacterName;      // 이번 대사에서 말하는 캐릭터 이름

    public List<string> choices;
    public List<int> nextDialogueIndexes;
}