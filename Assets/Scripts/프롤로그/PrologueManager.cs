using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[System.Serializable]
public class UIRevealInfo
{
    public int paragraphIndex;
    public GameObject uiObjectToReveal;
}

public class PrologueManager : MonoBehaviour
{
    public GameObject prologuePanel;
    public TextMeshProUGUI dialogueText;
    public GameObject triangleButton;

    public DialogueData newDialogueData; // 프롤로그 종료 후 시작할 대사
    public DialogueManager dialogueManager; // 같은 씬 내 DialogueManager 참조

    public float typingSpeed = 0.05f;

    [TextArea(3, 10)]
    public List<string> prologueParagraphs;
    public List<UIRevealInfo> uiRevealInfos;

    private int currentParagraphIndex = 0;
    private bool isTyping = false;
    private bool canContinue = false;
    private Coroutine blinkCoroutine;

    void Start()
    {
        dialogueText.richText = false;
        prologuePanel.SetActive(false);
        triangleButton.SetActive(false);
        dialogueText.text = "";

        foreach (var info in uiRevealInfos)
        {
            if (info.uiObjectToReveal != null)
                info.uiObjectToReveal.SetActive(false);
        }

        FadeInEffect fade = FindObjectOfType<FadeInEffect>();
        if (fade != null)
        {
            fade.onFadeComplete = () =>
            {
                prologuePanel.SetActive(true);
                StartCoroutine(TypeParagraph());
            };
        }
        else
        {
            prologuePanel.SetActive(true);
            StartCoroutine(TypeParagraph());
        }
    }

    IEnumerator TypeParagraph()
    {
        isTyping = true;
        canContinue = false;
        dialogueText.text = "";
        triangleButton.SetActive(false);

        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
            blinkCoroutine = null;
        }

        string currentText = prologueParagraphs[currentParagraphIndex];
        foreach (char c in currentText)
        {
            dialogueText.text += c;
            if (!isTyping) yield break;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        canContinue = true;
        triangleButton.SetActive(true);
        blinkCoroutine = StartCoroutine(BlinkTriangle());
    }

    IEnumerator BlinkTriangle()
    {
        Image triangleImage = triangleButton.GetComponent<Image>();
        while (canContinue)
        {
            triangleImage.enabled = !triangleImage.enabled;
            yield return new WaitForSeconds(1.0f);
        }
        triangleImage.enabled = true;
    }

    public void OnTriangleButtonPressed()
    {
        if (!canContinue) return;

        canContinue = false;
        StopAllCoroutines();
        currentParagraphIndex++;

        if (currentParagraphIndex < prologueParagraphs.Count)
        {
            StartCoroutine(TypeParagraph());

            foreach (UIRevealInfo info in uiRevealInfos)
            {
                if (info.paragraphIndex == currentParagraphIndex)
                {
                    if (info.uiObjectToReveal != null)
                        info.uiObjectToReveal.SetActive(true);
                }
            }
        }
        else
        {
            Debug.Log("▶ 프롤로그 종료 → 본편 대사 시작");

            prologuePanel.SetActive(false); // 프롤로그 UI 비활성화
            if (dialogueManager != null && newDialogueData != null)
            {
                dialogueManager.gameObject.SetActive(true);
                dialogueManager.StartDialogue(newDialogueData); // 본편 대사 시작
            }
            else
            {
                Debug.LogWarning("DialogueManager 또는 newDialogueData가 연결되지 않음");
            }
        }
    }
}
