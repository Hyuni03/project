using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KeyboardInputManager : MonoBehaviour
{
    [Header("닉네임 확인 패널 관련")]
    public GameObject confirmationPanel;
    public TMP_Text confirmationText;
    public Button yesButton;
    public Button noButton;

    [Header("페이드 관련")]
    public TMP_InputField nicknameInput;
    public TMP_Text leftShiftKeyText;
    public TMP_Text rightShiftKeyText;
    public TMP_Text capsLockKeyText;
    public TMP_Text langKeyText;
    public CanvasGroup fadeCanvasGroup;

    public Button[] keyboardButtons;

    private bool isLeftShiftActive = false;
    private bool isRightShiftActive = false;
    private bool isCapsLockActive = false;
    private bool isKorean = true;
    private int maxCharacters = 12;

    private Dictionary<string, string> koreanKeyMap = new Dictionary<string, string>()
    {
        { "q", "ㅂ" }, { "w", "ㅈ" }, { "e", "ㄷ" }, { "r", "ㄱ" },
        { "t", "ㅅ" }, { "y", "ㅛ" }, { "u", "ㅕ" }, { "i", "ㅑ" },
        { "o", "ㅐ" }, { "p", "ㅔ" }, { "a", "ㅁ" }, { "s", "ㄴ" },
        { "d", "ㅇ" }, { "f", "ㄹ" }, { "g", "ㅎ" }, { "h", "ㅗ" },
        { "j", "ㅓ" }, { "k", "ㅏ" }, { "l", "ㅣ" }, { "z", "ㅋ" },
        { "x", "ㅌ" }, { "c", "ㅊ" }, { "v", "ㅍ" }, { "b", "ㅠ" },
        { "n", "ㅜ" }, { "m", "ㅡ" }
    };

    void Start()
    {
        if (keyboardButtons == null || keyboardButtons.Length == 0)
        {
            GameObject keyboardPanel = GameObject.FindGameObjectWithTag("Keyboard");
            if (keyboardPanel != null)
                keyboardButtons = keyboardPanel.GetComponentsInChildren<Button>();
        }

        Debug.Log($"찾은 키보드 버튼 개수: {(keyboardButtons != null ? keyboardButtons.Length.ToString() : "NULL")}");

        if (keyboardButtons == null || keyboardButtons.Length == 0)
        {
            Debug.LogError("keyboardButtons 배열이 설정되지 않음!");
            return;
        }

        foreach (Button button in keyboardButtons)
        {
            if (button == null) continue;

            KeyValue keyValue = button.GetComponent<KeyValue>();
            if (keyValue == null)
            {
                Debug.LogError($"버튼 '{button.name}'에 KeyValue 컴포넌트가 없음!");
                continue;
            }

            button.onClick.RemoveAllListeners();
            Button capturedButton = button;
            button.onClick.AddListener(() => OnKeyPress(capturedButton));
        }

        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.alpha = 0f;
            fadeCanvasGroup.blocksRaycasts = false;
        }
    }

    public void OnKeyPress(Button button)
    {
        if (nicknameInput == null)
        {
            Debug.LogError("nicknameInput이 설정되지 않음!");
            return;
        }

        KeyValue keyValue = button.GetComponent<KeyValue>();
        if (keyValue == null)
        {
            Debug.LogError($"버튼 '{button.name}'에 KeyValue 컴포넌트가 없음!");
            return;
        }

        string normalKey = keyValue.normalKey;
        Debug.Log($"눌린 키: {normalKey}");

        if (normalKey == "Backspace")
        {
            if (nicknameInput.text.Length > 0)
                nicknameInput.text = nicknameInput.text.Substring(0, nicknameInput.text.Length - 1);
            return;
        }

        if (normalKey == "LeftShift") { isLeftShiftActive = !isLeftShiftActive; UpdateKeyStates(); return; }
        if (normalKey == "RightShift") { isRightShiftActive = !isRightShiftActive; UpdateKeyStates(); return; }
        if (normalKey == "CapsLock") { isCapsLockActive = !isCapsLockActive; UpdateKeyStates(); return; }
        if (normalKey == "Lang") { isKorean = !isKorean; UpdateKeyStates(); return; }
        if (normalKey == "Space") { nicknameInput.text += " "; return; }

        string inputKey = (isLeftShiftActive || isRightShiftActive) ? keyValue.shiftKey : keyValue.normalKey;
        if (string.IsNullOrEmpty(inputKey)) return;
        if (nicknameInput.text.Length >= maxCharacters) return;

        if (isKorean && inputKey.Length == 1 && koreanKeyMap.ContainsKey(inputKey.ToLower()))
            inputKey = koreanKeyMap[inputKey.ToLower()];
        else if (!isKorean && inputKey.Length == 1 && char.IsLetter(inputKey[0]))
        {
            bool shiftActive = isLeftShiftActive || isRightShiftActive;
            bool toUpper = isCapsLockActive ^ shiftActive;
            inputKey = toUpper ? inputKey.ToUpper() : inputKey.ToLower();
        }

        nicknameInput.text += inputKey;
        if (isLeftShiftActive || isRightShiftActive)
        {
            isLeftShiftActive = false;
            isRightShiftActive = false;
            UpdateKeyStates();
        }
    }

    private void UpdateKeyStates()
    {
        leftShiftKeyText.color = isLeftShiftActive ? Color.gray : Color.white;
        rightShiftKeyText.color = isRightShiftActive ? Color.gray : Color.white;
        capsLockKeyText.color = isCapsLockActive ? Color.gray : Color.white;
        langKeyText.text = isKorean ? "한" : "영";
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            ShowConfirmationPanel();
        }
    }

    public void OnYesButtonClicked()
    {
        StartCoroutine(ConfirmNickname());
    }

    public void OnNoButtonClicked()
    {
        confirmationPanel.SetActive(false);
    }

    public void ShowConfirmationPanel()
    {
        confirmationText.text = $"이름을 \"{nicknameInput.text}\"로 하시겠습니까?";
        confirmationPanel.SetActive(true);

        yesButton.onClick.RemoveAllListeners();
        yesButton.onClick.AddListener(() => StartCoroutine(ConfirmNickname()));

        noButton.onClick.RemoveAllListeners();
        noButton.onClick.AddListener(() => confirmationPanel.SetActive(false));

        Debug.Log("확인 패널이 표시되었습니다.");
    }

    private IEnumerator ConfirmNickname()
    {
        confirmationPanel.SetActive(false);

        fadeCanvasGroup.blocksRaycasts = true;

        float duration = 1.5f;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / duration);
            yield return null;
        }
        PlayerPrefs.SetString("PlayerName", nicknameInput.text); // ✅ 저장
        PlayerPrefs.Save();
        SceneManager.LoadScene("PrologueTemplate");
    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        float duration = 1.5f;
        float timer = 0f;

        fadeCanvasGroup.blocksRaycasts = true;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / duration);
            yield return null;
        }

        fadeCanvasGroup.alpha = 0f;
        fadeCanvasGroup.blocksRaycasts = false;
    }
}
