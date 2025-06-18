using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
    [Header("데이터 연결")]
    public DialogueData dialogueData;

    [Header("UI 연결")]
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
    public GameObject dialogueRoot;

    [Header("페이드 설정")]
    public CanvasGroup fadeCanvasGroup;
    public float fadeDuration = 1f;

    [Header("자동 시작 여부")]
    public bool autoStartOnSceneLoad = false;

    private int currentIndex = 0;
    private string previousSpeaker = "";
    private DialogueData currentDialogueData;

    void Start()
    {
        nextButton.onClick.AddListener(ShowNextDialogue);
        prevButton.onClick.AddListener(ShowPreviousDialogue);

        if (autoStartOnSceneLoad && dialogueData != null)
        {
            StartDialogue(dialogueData, true); // 자동 시작 시 초기화 O
        }
    }

    public void StartDialogue(DialogueData data)
    {
        StartDialogue(data, true);
    }

    public void StartDialogue(DialogueData data, bool resetIndex)
    {
        Debug.Log("▶ StartDialogue() 호출됨");

        currentDialogueData = data;

        if (resetIndex)
            currentIndex = 0;

        if (dialogueRoot != null)
            dialogueRoot.SetActive(true);

        ShowDialogue(currentIndex);
    }

    void ShowDialogue(int index)
    {
        if (currentDialogueData == null || currentDialogueData.lines.Count == 0)
            return;

        DialogueLine line = currentDialogueData.lines[index];

        string playerName = PlayerPrefs.GetString("PlayerName", "플레이어");
        dialogueText.text = line.dialogueText.Replace("플레이어", playerName);

        if (line.backgroundSprite != null)
            backgroundImage.sprite = line.backgroundSprite;

        leftCharacterImage.gameObject.SetActive(false);
        rightCharacterImage.gameObject.SetActive(false);
        centerCharacterImage.gameObject.SetActive(false);

        string speaker = string.IsNullOrEmpty(line.speakerName) ? previousSpeaker : line.speakerName;
        speaker = speaker.Trim().Replace("플레이어", playerName);
        previousSpeaker = speaker;

        nameText.enabled = true;
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

        if (line.choices != null && line.choices.Count > 0 && line.nextDialogueIndexes != null && line.nextDialogueIndexes.Count == line.choices.Count)
        {
            ShowChoices(line.choices, line.nextDialogueIndexes);
        }
        else if (line.nextDialogueIndexes != null && line.nextDialogueIndexes.Count == 1)
        {
            nextButton.onClick.RemoveAllListeners();
            nextButton.onClick.AddListener(() =>
            {
                currentIndex = line.nextDialogueIndexes[0];
                ShowDialogue(currentIndex);
            });

            choicePanel.SetActive(false);
        }
        else
        {
            choicePanel.SetActive(false);

            // ✅ 마지막 대사에서 Next 버튼 리스너를 다시 연결
            nextButton.onClick.RemoveAllListeners();
            nextButton.onClick.AddListener(ShowNextDialogue);
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
            choiceButtons[i].onClick.RemoveAllListeners();
            choiceButtons[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < choices.Count && i < choiceButtons.Count; i++)
        {
            var button = choiceButtons[i];
            var text = choiceTexts[i];

            int capturedIndex = nextIndexes[i];

            button.gameObject.SetActive(true);
            text.text = choices[i];

            button.onClick.AddListener(() =>
            {
                Debug.Log($"[선택지 클릭] 이동 인덱스: {capturedIndex}");
                choicePanel.SetActive(false);
                currentIndex = capturedIndex;
                ShowDialogue(currentIndex);
            });
        }
    }

    public void ShowNextDialogue()
    {
        if (currentDialogueData == null || currentDialogueData.lines == null)
        {
            Debug.LogWarning("DialogueData가 없어서 진행할 수 없습니다.");
            return;
        }

        if (currentIndex < currentDialogueData.lines.Count - 1)
        {
            currentIndex++;
            ShowDialogue(currentIndex);
        }
        else
        {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            int nextSceneIndex = currentSceneIndex + 1;

            Debug.Log($"▶ 씬 {currentSceneIndex} 종료 → 다음 씬 {nextSceneIndex}으로 이동");

            if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(nextSceneIndex);
            }
            else
            {
                Debug.LogWarning("▶ 다음 씬 없음: 마지막 씬입니다.");
            }
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