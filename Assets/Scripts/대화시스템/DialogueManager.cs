using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // 씬 전환을 위해 필요

public class DialogueManager : MonoBehaviour
{
    public DialogueData dialogueData;         // 대사 데이터 (ScriptableObject 형식)
    public string nextSceneName;              // 마지막 대사 후 넘어갈 씬 이름

    // UI 텍스트 및 이미지 요소
    public TMP_Text nameText;                 // 캐릭터 이름 출력 텍스트
    public TMP_Text dialogueText;             // 대사 텍스트
    public Image backgroundImage;             // 배경 이미지

    // 캐릭터 이미지들
    public Image centerCharacterImage;
    public Image leftCharacterImage;
    public Image rightCharacterImage;

    // 네비게이션 버튼
    public Button nextButton;
    public Button prevButton;

    // 선택지 관련 UI
    public List<Button> choiceButtons;        // 선택지 버튼들
    public List<TMP_Text> choiceTexts;        // 선택지 텍스트
    public GameObject choicePanel;            // 선택지 전체 패널

    private int currentIndex = 0;             // 현재 대사 인덱스

    void Start()
    {
        // 이어하기 기능: 저장된 인덱스가 있으면 해당 위치부터 시작
        if (PlayerPrefs.HasKey("ContinueIndex"))
        {
            currentIndex = PlayerPrefs.GetInt("ContinueIndex", 0);
            PlayerPrefs.DeleteKey("ContinueIndex"); // 불러온 후 삭제 (한 번만 사용)
        }

        // 첫 대사 표시
        ShowDialogue(currentIndex);

        // 버튼 이벤트 등록
        nextButton.onClick.AddListener(ShowNextDialogue);
        prevButton.onClick.AddListener(ShowPreviousDialogue);
    }

    // index 번째 대사를 화면에 출력
    void ShowDialogue(int index)
    {
        if (dialogueData == null || dialogueData.lines.Count == 0)
            return;

        DialogueLine line = dialogueData.lines[index];
        dialogueText.text = line.dialogueText;

        // 배경 이미지 설정
        if (line.backgroundSprite != null)
            backgroundImage.sprite = line.backgroundSprite;

        // 캐릭터 이미지 초기화
        centerCharacterImage.gameObject.SetActive(false);
        leftCharacterImage.gameObject.SetActive(false);
        rightCharacterImage.gameObject.SetActive(false);

        // 이름 표시
        nameText.text = line.speakingCharacterName;

        // 캐릭터가 2명 등장하는 경우
        if (line.characterNames.Count == 2)
        {
            string leftName = line.characterNames[0];
            string rightName = line.characterNames[1];

            leftCharacterImage.sprite = line.characterSprites[0];
            rightCharacterImage.sprite = line.characterSprites[1];

            // 말하는 캐릭터는 선명하게, 아닌 쪽은 반투명하게 표시
            leftCharacterImage.color = (line.speakingCharacterName == leftName) ? Color.white : new Color(1f, 1f, 1f, 0.5f);
            rightCharacterImage.color = (line.speakingCharacterName == rightName) ? Color.white : new Color(1f, 1f, 1f, 0.5f);

            leftCharacterImage.gameObject.SetActive(true);
            rightCharacterImage.gameObject.SetActive(true);
        }
        // 캐릭터가 1명 등장하는 경우
        else if (line.characterNames.Count == 1)
        {
            nameText.text = line.characterNames[0];
            centerCharacterImage.sprite = line.characterSprites[0];
            centerCharacterImage.color = Color.white;
            centerCharacterImage.gameObject.SetActive(true);
        }
    }

    // 선택지 표시 함수 (선택 시 다음 인덱스로 이동)
    void ShowChoices(List<string> choices, List<int> nextIndexes)
    {
        choicePanel.SetActive(true); // 선택지 패널 열기

        for (int i = 0; i < choiceButtons.Count; i++)
        {
            if (i < choices.Count)
            {
                choiceButtons[i].gameObject.SetActive(true);
                choiceTexts[i].text = choices[i];

                int nextIndex = nextIndexes[i];
                string selectedText = choices[i];

                // 플레이어 닉네임 불러오기
                string playerName = PlayerPrefs.GetString("PlayerName", "플레이어");

                choiceButtons[i].onClick.RemoveAllListeners();
                choiceButtons[i].onClick.AddListener(() =>
                {
                    // 선택된 텍스트 출력 (선택 효과)
                    dialogueText.text = selectedText;
                    nameText.text = playerName;

                    currentIndex = nextIndex;

                    // 잠깐 보여준 뒤 다음 대사 출력
                    StartCoroutine(DelayedShowDialogue(0.8f));
                    choicePanel.SetActive(false);
                });
            }
            else
            {
                // 사용하지 않는 버튼은 숨기기
                choiceButtons[i].gameObject.SetActive(false);
            }
        }
    }

    // 딜레이 후 다음 대사 출력
    IEnumerator DelayedShowDialogue(float delay)
    {
        yield return new WaitForSeconds(delay);
        ShowDialogue(currentIndex);
    }

    // 다음 대사로 이동
    public void ShowNextDialogue()
    {
        if (currentIndex < dialogueData.lines.Count - 1)
        {
            currentIndex++;
            ShowDialogue(currentIndex);
        }
        else
        {
            // 마지막 대사일 경우 → 다음 씬으로 전환
            if (!string.IsNullOrEmpty(nextSceneName))
            {
                SceneManager.LoadScene(nextSceneName);
            }
        }
    }

    // 이전 대사로 이동
    public void ShowPreviousDialogue()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            ShowDialogue(currentIndex);
        }
    }

    // 저장 함수 (팝업에서 '예' 버튼과 연결해 사용)
    public void SaveGame()
    {
        PlayerPrefs.SetInt("ContinueIndex", currentIndex);
        PlayerPrefs.Save();
        Debug.Log("게임 저장됨. 인덱스: " + currentIndex);
    }
}
