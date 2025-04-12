using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueEntry
{
    public string speakerName;          // ��縦 ���ϴ� ����� �̸�
    [TextArea]
    public string dialogueText;        // ��� ����
    public Sprite backgroundImage;     // ��� �̹���
    public Sprite characterImage;      // ĳ���� �̹���
}

[CreateAssetMenu(fileName = "NewDialogueData", menuName = "Dialogue/Dialogue Data")]
public class DialogueData : ScriptableObject // MonoBehaviour -> ScriptableObject�� ����
{
    public DialogueEntry[] entries;    // ���� ���� ��縦 �迭�� ����
}

