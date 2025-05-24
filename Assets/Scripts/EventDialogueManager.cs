using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class EventDialogueManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject dialoguePanel;
    public TMP_Text dialogueText;
    public GameObject choicePanel;
    public GameObject choiceButtonPrefab;

    [Header("대사 데이터")]
    public List<DialogueLine> dialogueLines;
    private int currentLineIndex = 0;

    void Start()
    {
        ShowNextLine();
    }

    void ShowNextLine()
    {
        if (currentLineIndex >= dialogueLines.Count)
        {
            dialoguePanel.SetActive(false);
            choicePanel.SetActive(false);
            return;
        }

        DialogueLine line = dialogueLines[currentLineIndex];

        dialogueText.text = line.text;

        if (line.choices.Count > 0)
        {
            ShowChoices(line.choices);
        }
        else
        {
            choicePanel.SetActive(false);
            Invoke("NextLineAfterDelay", 2f); // 2초 후 자동 진행
        }
    }

    void NextLineAfterDelay()
    {
        currentLineIndex++;
        ShowNextLine();
    }

    void ShowChoices(List<DialogueChoice> choices)
    {
        choicePanel.SetActive(true);

        // 기존 선택지 제거
        foreach (Transform child in choicePanel.transform)
            Destroy(child.gameObject);

        foreach (DialogueChoice choice in choices)
        {
            GameObject buttonObj = Instantiate(choiceButtonPrefab, choicePanel.transform);
            TMP_Text buttonText = buttonObj.GetComponentInChildren<TMP_Text>();
            buttonText.text = choice.choiceText;

            Button btn = buttonObj.GetComponent<Button>();
            btn.onClick.AddListener(() =>
            {
                currentLineIndex = choice.nextLineIndex;
                ShowNextLine();
            });
        }
    }
}
