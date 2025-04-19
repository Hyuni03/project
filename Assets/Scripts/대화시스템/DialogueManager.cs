using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public DialogueData dialogueData;
    public TMP_Text nameText;
    public TMP_Text dialogueText;
    public Image backgroundImage;

    public Image centerCharacterImage;
    public Image leftCharacterImage;
    public Image rightCharacterImage;

    public Button nextButton;
    public Button prevButton;
    public List<Button> choiceButtons;  // 선택지 버튼들
    public List<TMP_Text> choiceTexts;  // 선택지 텍스트들
    public GameObject choicePanel;      // 선택지 전체 패널

    private int currentIndex = 0;

    void Start()
    {
        ShowDialogue(currentIndex);
        nextButton.onClick.AddListener(ShowNextDialogue);
        prevButton.onClick.AddListener(ShowPreviousDialogue);
    }

    void ShowDialogue(int index)
    {
        if (dialogueData == null || dialogueData.lines.Count == 0)
            return;

        DialogueLine line = dialogueData.lines[index];
        dialogueText.text = line.dialogueText;

        // 배경
        if (line.backgroundSprite != null)
            backgroundImage.sprite = line.backgroundSprite;

        // 캐릭터 이미지 초기화
        centerCharacterImage.gameObject.SetActive(false);
        leftCharacterImage.gameObject.SetActive(false);
        rightCharacterImage.gameObject.SetActive(false);

        // 이름은 말하는 캐릭터만 표시
        nameText.text = line.speakingCharacterName;

        // 캐릭터 두 명 화면에 고정
        if (line.characterNames.Count == 2)
        {
            string leftName = line.characterNames[0];
            string rightName = line.characterNames[1];

            leftCharacterImage.sprite = line.characterSprites[0];
            rightCharacterImage.sprite = line.characterSprites[1];

            // 누가 말하는지 확인하고 색상 설정
            leftCharacterImage.color = (line.speakingCharacterName == leftName) ?
                new Color(1f, 1f, 1f, 1f) : new Color(1f, 1f, 1f, 0.5f);
            rightCharacterImage.color = (line.speakingCharacterName == rightName) ?
                new Color(1f, 1f, 1f, 1f) : new Color(1f, 1f, 1f, 0.5f);

            leftCharacterImage.gameObject.SetActive(true);
            rightCharacterImage.gameObject.SetActive(true);
        }
        else if (line.characterNames.Count == 1)
        {
            // 캐릭터 1명만 등장할 때
            nameText.text = line.characterNames[0];
            centerCharacterImage.sprite = line.characterSprites[0];
            centerCharacterImage.color = new Color(1f, 1f, 1f, 1f);
            centerCharacterImage.gameObject.SetActive(true);
        }
    }


    void ShowChoices(List<string> choices, List<int> nextIndexes)
    {
        choicePanel.SetActive(true); // 선택지 패널 보이기

        for (int i = 0; i < choiceButtons.Count; i++)
        {
            if (i < choices.Count)
            {
                choiceButtons[i].gameObject.SetActive(true);
                choiceTexts[i].text = choices[i];

                int nextIndex = nextIndexes[i]; // 선택한 후 넘어갈 인덱스 저장
                choiceButtons[i].onClick.RemoveAllListeners(); // 기존 이벤트 제거
                choiceButtons[i].onClick.AddListener(() =>
                {
                    currentIndex = nextIndex;
                    ShowDialogue(currentIndex);
                    choicePanel.SetActive(false); // 선택지 숨기기
                });
            }
            else
            {
                choiceButtons[i].gameObject.SetActive(false); // 사용하지 않는 버튼은 숨김
            }
        }
    }

    public void ShowNextDialogue()
    {
        if (currentIndex < dialogueData.lines.Count - 1)
        {
            currentIndex++;
            ShowDialogue(currentIndex);
        }
    }

    public void ShowPreviousDialogue()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            ShowDialogue(currentIndex);
        }
    }
}

