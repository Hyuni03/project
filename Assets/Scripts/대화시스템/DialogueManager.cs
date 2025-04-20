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
    public List<Button> choiceButtons;
    public List<TMP_Text> choiceTexts;
    public GameObject choicePanel;

    private int currentIndex = 0;
    private string previousSpeaker = "";

    void Awake()
    {
        Debug.Log("[Awake] 초기화 시작");

        nextButton.onClick.RemoveAllListeners();
        prevButton.onClick.RemoveAllListeners();

        nextButton.onClick.AddListener(ShowNextDialogue);
        prevButton.onClick.AddListener(ShowPreviousDialogue);
    }

    void Start()
    {
        ShowDialogue(currentIndex); // 대사는 여기서!
    }


    void ShowDialogue(int index)
    {
        if (dialogueData == null || dialogueData.lines.Count == 0 || index < 0 || index >= dialogueData.lines.Count)
            return;

        Debug.Log($"[ShowDialogue] index: {index}, total: {dialogueData.lines.Count}");

        DialogueLine line = dialogueData.lines[index];
        Debug.Log($"[DialogueLine] speaker: {line.speakerName}, dialogue: {line.dialogueText}");

        // ✅ 선택지 있는 경우: 대사 진행 멈추고 선택지만 보여줌
        if (line.choices != null && line.choices.Count > 0)
        {
            ShowChoices(line.choices, line.nextDialogueIndexes);
            return;
        }
        else
        {
            choicePanel.SetActive(false); // 선택지 없으면 패널 끔
        }

        dialogueText.text = line.dialogueText;

        if (line.backgroundSprite != null)
            backgroundImage.sprite = line.backgroundSprite;

        leftCharacterImage.gameObject.SetActive(false);
        rightCharacterImage.gameObject.SetActive(false);
        centerCharacterImage.gameObject.SetActive(false);

        if (line.characterNames.Count == 1)
        {
            centerCharacterImage.gameObject.SetActive(true);
            centerCharacterImage.sprite = line.characterSprites[0];
            leftCharacterImage.color = new Color(1, 1, 1, 0);
            rightCharacterImage.color = new Color(1, 1, 1, 0);
            nameText.text = line.characterNames[0];
            previousSpeaker = line.characterNames[0];
        }
        else if (line.characterNames.Count == 2)
        {
            leftCharacterImage.gameObject.SetActive(true);
            rightCharacterImage.gameObject.SetActive(true);

            leftCharacterImage.sprite = line.characterSprites[0];
            rightCharacterImage.sprite = line.characterSprites[1];

            string speaker = line.speakerName;

            // speakerName이 비어있거나 잘못되면 이전 화자 사용
            if (string.IsNullOrEmpty(speaker) || !line.characterNames.Contains(speaker))
            {
                speaker = previousSpeaker;
            }

            nameText.text = speaker;

            // 화자 강조 색상
            if (speaker == line.characterNames[0])
            {
                leftCharacterImage.color = Color.white;
                rightCharacterImage.color = new Color(0.7f, 0.7f, 0.7f);
            }
            else if (speaker == line.characterNames[1])
            {
                leftCharacterImage.color = new Color(0.7f, 0.7f, 0.7f);
                rightCharacterImage.color = Color.white;
            }
            else
            {
                leftCharacterImage.color = new Color(0.7f, 0.7f, 0.7f);
                rightCharacterImage.color = new Color(0.7f, 0.7f, 0.7f);
            }

            previousSpeaker = speaker;
        }
    }

    void ShowChoices(List<string> choices, List<int> nextIndexes)
    {
        // 버튼 리스너 초기화 및 텍스트 설정
        for (int i = 0; i < choiceButtons.Count; i++)
        {
            if (i < choices.Count)
            {
                choiceButtons[i].gameObject.SetActive(true);
                choiceTexts[i].text = choices[i];

                int nextIndex = nextIndexes[i]; // 클로저 캡처 주의

                choiceButtons[i].onClick.RemoveAllListeners();
                choiceButtons[i].onClick.AddListener(() =>
                {
                    choicePanel.SetActive(false);     // 선택지 숨기기
                    currentIndex = nextIndex;
                    ShowDialogue(currentIndex);       // 다음 대사 보여주기
                });
            }
            else
            {
                choiceButtons[i].gameObject.SetActive(false);
            }
        }

        choicePanel.SetActive(true); // 마지막에 켜기
    }

    public void ShowNextDialogue()
    {
        Debug.Log($"[👉 ShowNextDialogue CALLED] currentIndex BEFORE: {currentIndex}");

        if (currentIndex < dialogueData.lines.Count - 1)
        {
            currentIndex++;
            Debug.Log($"[✅ SHOWING] currentIndex AFTER: {currentIndex}");
            ShowDialogue(currentIndex);
        }
        else
        {
            Debug.Log("[⛔ 끝까지 도달함]");
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