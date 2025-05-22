using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PrologueManager : MonoBehaviour
{
    public GameObject prologuePanel;
    public TextMeshProUGUI dialogueText;
    public GameObject triangleButton;
    public GameObject characterObject;  // 캐릭터 등장 오브젝트
    public GameObject itemObject;       // 아이템 또는 장면에 나올 요소

    public float typingSpeed = 0.05f;

    [TextArea(3, 10)]
    public List<string> prologueParagraphs;

    private int currentParagraphIndex = 0;
    private bool isTyping = false;
    private bool canContinue = false;

    void Start()
    {
        prologuePanel.SetActive(true);
        triangleButton.SetActive(false);

        characterObject.SetActive(false);
        itemObject.SetActive(false);

        StartCoroutine(TypeParagraph());
    }

    IEnumerator TypeParagraph()
    {
        isTyping = true;
        canContinue = false;
        dialogueText.text = "";

        triangleButton.SetActive(false); // 세모 버튼 숨김
        StopCoroutine(BlinkTriangle());  // 이전 깜빡임 코루틴 강제 종료

        string currentText = prologueParagraphs[currentParagraphIndex];

        foreach (char c in currentText)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        canContinue = true;
        triangleButton.SetActive(true);
        StartCoroutine(BlinkTriangle());
    }


    IEnumerator BlinkTriangle()
    {
        Image triangleImage = triangleButton.GetComponent<Image>();

        while (canContinue)
        {
            triangleImage.enabled = !triangleImage.enabled;
            yield return new WaitForSeconds(1.0f); // 혹은 1.0f 로 더 느리게
        }

        triangleImage.enabled = true; // 보이도록 정리
    }

    public void OnTriangleButtonPressed()
    {
        if (!canContinue) return;  // 중복 클릭 방지

        canContinue = false;  // 중복 방지
        StopAllCoroutines();  // 이전 코루틴을 강제로 중단

        currentParagraphIndex++;

        if (currentParagraphIndex < prologueParagraphs.Count)
        {
            // 특정 문단에서 캐릭터와 아이템 등장
            if (currentParagraphIndex == 2) // 원하는 문단 번호에 맞춰 조정
            {
                characterObject.SetActive(true);
                itemObject.SetActive(true);
            }

            StartCoroutine(TypeParagraph());  // 문장 타이핑 다시 시작
        }
        else
        {
            prologuePanel.SetActive(false);  // 모든 문장이 끝나면 패널 숨기기
        }
    }

}
