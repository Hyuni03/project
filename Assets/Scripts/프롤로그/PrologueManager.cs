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
    // 본편으로 전환할 DialogueData ScriptableObject
    public DialogueData newDialogueData; // 인스펙터에서 연결
    public DialogueManager dialogueManager; // 본편 DialogueManager 참조


    public float typingSpeed = 0.05f;

    [TextArea(3, 10)]
    public List<string> prologueParagraphs;

    public List<UIRevealInfo> uiRevealInfos; // 원하는 문단에 오브젝트 등장 설정

    private int currentParagraphIndex = 0;
    private bool isTyping = false;
    private bool canContinue = false;

    private Coroutine blinkCoroutine;

    void Start()
    {
        dialogueText.richText = false;  // Start()나 Awake() 등에서 설정 가능

        prologuePanel.SetActive(true);
        triangleButton.SetActive(false);
        dialogueText.text = "";

        // 모든 오브젝트 미리 숨기기
        foreach (var info in uiRevealInfos)
        {
            if (info.uiObjectToReveal != null)
                info.uiObjectToReveal.SetActive(false);
        }

        StartCoroutine(TypeParagraph());
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
        Debug.Log($"▶ 문단 {currentParagraphIndex} 출력: {currentText}");

        foreach (char c in currentText)
        {
            dialogueText.text += c;

            // 문자 중간 끊김 방지
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
            yield return new WaitForSeconds(1.0f); // 느리게 깜빡임
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

            // 현재 문단에 해당하는 UI 오브젝트들을 등장시킴
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
            prologuePanel.SetActive(false); // 프롤로그 숨김

            if (dialogueManager != null && newDialogueData != null)
            {
                dialogueManager.gameObject.SetActive(true); // DialogueManager 켜기
                dialogueManager.StartDialogue(newDialogueData); // 본편 대사 시작
            }
            else
            {
                Debug.LogWarning("DialogueManager 또는 newDialogueData가 연결되지 않았습니다.");
            }
        }


    }
}