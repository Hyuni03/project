using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueEntry
{
    public string speakerName;          // 대사를 말하는 사람의 이름
    [TextArea]
    public string dialogueText;        // 대사 내용
    public Sprite backgroundImage;     // 배경 이미지
    public Sprite characterImage;      // 캐릭터 이미지
}

[CreateAssetMenu(fileName = "NewDialogueData", menuName = "Dialogue/Dialogue Data")]
public class DialogueData : ScriptableObject // MonoBehaviour -> ScriptableObject로 변경
{
    public DialogueEntry[] entries;    // 여러 개의 대사를 배열로 저장
}

