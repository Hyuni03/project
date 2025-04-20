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
    public VerticalLayoutGroup choiceLayoutGroup;

    private int currentIndex = 0;
    private string previousSpeaker = "";

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

        // 플레이어 이름 불러오기
        string playerName = PlayerPrefs.GetString("PlayerName", "플레이어");

        // 텍스트 치환해서 표시
        dialogueText.text = line.dialogueText.Replace("플레이어", playerName);

        if (line.backgroundSprite != null)
            backgroundImage.sprite = line.backgroundSprite;

        // 초기화
        leftCharacterImage.gameObject.SetActive(false);
        rightCharacterImage.gameObject.SetActive(false);
        centerCharacterImage.gameObject.SetActive(false);

        // speaker 설정
        string speaker = string.IsNullOrEmpty(line.speakerName) ? previousSpeaker : line.speakerName;
        speaker = speaker.Trim().Replace("플레이어", playerName);
        nameText.text = speaker;

        if (line.characterNames.Count == 1)
        {
            centerCharacterImage.gameObject.SetActive(true);
            centerCharacterImage.sprite = line.characterSprites[0];
            leftCharacterImage.color = new Color(1, 1, 1, 0);
            rightCharacterImage.color = new Color(1, 1, 1, 0);
        }
        else if (line.characterNames.Count == 2)
        {
            leftCharacterImage.gameObject.SetActive(true);
            rightCharacterImage.gameObject.SetActive(true);

            leftCharacterImage.sprite = line.characterSprites[0];
            rightCharacterImage.sprite = line.characterSprites[1];

            string leftName = line.characterNames[0].Trim();
            string rightName = line.characterNames[1].Trim();

            if (speaker.Equals(leftName))
            {
                leftCharacterImage.color = Color.white;
                rightCharacterImage.color = new Color(0.7f, 0.7f, 0.7f);
            }
            else if (speaker.Equals(rightName))
            {
                leftCharacterImage.color = new Color(0.7f, 0.7f, 0.7f);
                rightCharacterImage.color = Color.white;
            }
            else
            {
                leftCharacterImage.color = new Color(0.7f, 0.7f, 0.7f);
                rightCharacterImage.color = new Color(0.7f, 0.7f, 0.7f);
            }
        }

        previousSpeaker = speaker;

        if (line.choices != null && line.choices.Count > 0 && line.nextDialogueIndexes != null)
        {
            ShowChoices(line.choices, line.nextDialogueIndexes);
        }
        else
        {
            choicePanel.SetActive(false);
        }
    }

    void ShowChoices(List<string> choices, List<int> nextIndexes)
    {
        if (choiceLayoutGroup != null)
        {
            switch (choices.Count)
            {
                case 2: choiceLayoutGroup.spacing = 60f; break;
                case 3: choiceLayoutGroup.spacing = 45f; break;
                case 4: choiceLayoutGroup.spacing = 25f; break;
                default: choiceLayoutGroup.spacing = 30f; break;
            }
        }

        choicePanel.SetActive(true);

        for (int i = 0; i < choiceButtons.Count; i++)
        {
            if (i < choices.Count)
            {
                choiceButtons[i].gameObject.SetActive(true);
                choiceTexts[i].text = choices[i];

                int nextIndex = nextIndexes[i];
                choiceButtons[i].onClick.RemoveAllListeners();
                choiceButtons[i].onClick.AddListener(() =>
                {
                    currentIndex = nextIndex;
                    ShowDialogue(currentIndex);
                    choicePanel.SetActive(false);
                });
            }
            else
            {
                choiceButtons[i].gameObject.SetActive(false);
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
