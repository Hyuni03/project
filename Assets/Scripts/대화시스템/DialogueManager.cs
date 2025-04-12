using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public DialogueData dialogueData;

    public Image backgroundImage;
    public Image characterImage;
    public TMP_Text speakerNameText;
    public TMP_Text dialogueText;

    private int currentIndex = 0;

    void Start()
    {
        ShowDialogue(currentIndex);
    }

    public void OnClickNext()
    {
        currentIndex++;
        if (currentIndex < dialogueData.entries.Length)
        {
            ShowDialogue(currentIndex);
        }
        else
        {
            Debug.Log("프롤로그 종료 or 다음 씬 이동");
            // 예: SceneManager.LoadScene("NextScene");
        }
    }

    private void ShowDialogue(int index)
    {
        var entry = dialogueData.entries[index];
        speakerNameText.text = entry.speakerName;
        dialogueText.text = entry.dialogueText;

        if (entry.backgroundImage != null)
            backgroundImage.sprite = entry.backgroundImage;

        if (entry.characterImage != null)
            characterImage.sprite = entry.characterImage;
        else
            characterImage.sprite = null; // 캐릭터 없음 처리
    }
}

