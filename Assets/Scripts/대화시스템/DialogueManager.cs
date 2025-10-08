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
            StartDialogue(dialogueData, true);
        }
    }

    public void StartDialogue(DialogueData data, bool resetIndex = true)
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
        if (currentDialogueData == null || currentDialogueData.lines == null || currentDialogueData.lines.Count == 0)
        {
            Debug.LogWarning("⚠️ DialogueData가 비어있습니다.");
            return;
        }

        if (index < 0 || index >= currentDialogueData.lines.Count)
        {
            Debug.LogWarning($"⚠ 잘못된 대사 인덱스 접근: {index}");
            return;
        }

        DialogueLine line = currentDialogueData.lines[index];

        string playerName = PlayerPrefs.GetString("PlayerName", "플레이어");
        dialogueText.text = line.dialogueText.Replace("플레이어", playerName);

        if (line.backgroundSprite != null)
            backgroundImage.sprite = line.backgroundSprite;

        // 캐릭터 이미지 초기화
        leftCharacterImage.gameObject.SetActive(false);
        rightCharacterImage.gameObject.SetActive(false);
        centerCharacterImage.gameObject.SetActive(false);

        // 화자 처리
        string speaker = string.IsNullOrEmpty(line.speakerName) ? previousSpeaker : line.speakerName;
        speaker = speaker.Trim().Replace("플레이어", playerName);
        previousSpeaker = speaker;

        nameText.enabled = true;
        nameText.text = speaker;

        // 캐릭터 이미지 표시
        if (line.characterNames.Count == 1)
        {
            centerCharacterImage.gameObject.SetActive(true);
            centerCharacterImage.sprite = line.characterSprites[0];
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

        // 선택지 처리
        if (line.choices != null && line.choices.Count > 0 && line.nextDialogueIndexes != null && line.nextDialogueIndexes.Count == line.choices.Count)
        {
            ShowChoices(line.choices, line.nextDialogueIndexes);
        }
        else if (line.nextDialogueIndexes != null && line.nextDialogueIndexes.Count == 1)
        {
            choicePanel.SetActive(false);
            nextButton.onClick.RemoveAllListeners();
            nextButton.onClick.AddListener(() =>
            {
                int nextIndex = line.nextDialogueIndexes[0];
                if (nextIndex >= 0 && nextIndex < currentDialogueData.lines.Count)
                {
                    currentIndex = nextIndex;
                    ShowDialogue(currentIndex);
                }
                else
                {
                    Debug.LogWarning($"⚠️ 잘못된 nextDialogueIndex 접근: {nextIndex}");
                }
            });
        }
        else
        {
            choicePanel.SetActive(false);
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
                if (capturedIndex >= 0 && capturedIndex < currentDialogueData.lines.Count)
                {
                    Debug.Log($"[선택지 클릭] 이동 인덱스: {capturedIndex}");
                    choicePanel.SetActive(false);
                    currentIndex = capturedIndex;
                    ShowDialogue(currentIndex);
                }
                else
                {
                    Debug.LogWarning($"⚠️ 잘못된 nextDialogueIndex 클릭: {capturedIndex}");
                }
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

        if (currentIndex < 0 || currentIndex >= currentDialogueData.lines.Count)
        {
            Debug.LogWarning($"잘못된 인덱스 접근 시도: {currentIndex}");
            return;
        }

        if (currentIndex < currentDialogueData.lines.Count - 1)
        {
            currentIndex++;
            ShowDialogue(currentIndex);
        }
        else
        {
            string currentSceneName = SceneManager.GetActiveScene().name;
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            int nextSceneIndex = currentSceneIndex + 1;

            Debug.Log($"▶ 씬 {currentSceneName} 종료 → 다음 씬 처리");

            // Episode4Scene이면 MainScene으로 이동
            if (currentSceneName == "Episode4Scene")
            {
                Debug.Log("▶ Episode4Scene 마지막 대사 → MainScene으로 이동");
                SceneManager.LoadScene("MainScene");
            }
            else if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
            {
                currentIndex = 0; // 다음 씬 시작 시 인덱스 초기화
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
