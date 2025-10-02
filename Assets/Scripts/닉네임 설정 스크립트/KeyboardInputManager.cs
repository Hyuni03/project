using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
    private HangulComposer composer = new HangulComposer();

    // QWERTY 자판의 한글 매핑 (기본 자모)
    private Dictionary<string, char> koreanKeyMap = new Dictionary<string, char>()
    {
        { "q", 'ㅂ' }, { "w", 'ㅈ' }, { "e", 'ㄷ' }, { "r", 'ㄱ' },
        { "t", 'ㅅ' }, { "y", 'ㅛ' }, { "u", 'ㅕ' }, { "i", 'ㅑ' },
        { "o", 'ㅐ' }, { "p", 'ㅔ' }, { "a", 'ㅁ' }, { "s", 'ㄴ' },
        { "d", 'ㅇ' }, { "f", 'ㄹ' }, { "g", 'ㅎ' }, { "h", 'ㅗ' },
        { "j", 'ㅓ' }, { "k", 'ㅏ' }, { "l", 'ㅣ' }, { "z", 'ㅋ' },
        { "x", 'ㅌ' }, { "c", 'ㅊ' }, { "v", 'ㅍ' }, { "b", 'ㅠ' },
        { "n", 'ㅜ' }, { "m", 'ㅡ' }
    };

    // Shift 상태에서의 자모 매핑 (쌍자음/쌍모음)
    private Dictionary<string, char> koreanShiftKeyMap = new Dictionary<string, char>()
    {
        { "q", 'ㅃ' }, { "w", 'ㅉ' }, { "e", 'ㄸ' }, { "r", 'ㄲ' },
        { "t", 'ㅆ' }, { "o", 'ㅒ' }, { "p", 'ㅖ' }
    };

    void Start()
    {
        if (keyboardButtons == null || keyboardButtons.Length == 0)
        {
            GameObject keyboardPanel = GameObject.FindGameObjectWithTag("Keyboard");
            if (keyboardPanel != null)
                keyboardButtons = keyboardPanel.GetComponentsInChildren<Button>();
        }

        foreach (Button button in keyboardButtons)
        {
            if (button == null) continue;
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
        if (nicknameInput == null) return;
        KeyValue keyValue = button.GetComponent<KeyValue>();

        string normalKey = keyValue != null ? keyValue.normalKey : button.name;

        // 특수 키 처리: Backspace
        if (normalKey == "Backspace")
        {
            if (nicknameInput.text.Length > 0)
            {
                // Backspace 입력 시, 현재 조합 중인 글자를 먼저 분리 시도 (더 정확한 백스페이스 로직)
                string removedSyllable = composer.RemoveLast();

                if (removedSyllable == "SYLLABLE_REMOVED")
                {
                    // 현재 조합 중이던 한 글자가 완전히 지워졌을 때
                    nicknameInput.text = nicknameInput.text.Substring(0, nicknameInput.text.Length - 1);
                }
                else if (!string.IsNullOrEmpty(removedSyllable))
                {
                    // 종성 등이 분해되어 글자가 갱신되었을 때
                    if (nicknameInput.text.Length > 0)
                    {
                        nicknameInput.text = nicknameInput.text.Substring(0, nicknameInput.text.Length - 1) + removedSyllable;
                    }
                }
                else
                {
                    // 현재 조합 중인 글자가 없거나, 조합기에서 제거할 것이 없을 때 마지막 글자 단순 삭제
                    nicknameInput.text = nicknameInput.text.Substring(0, nicknameInput.text.Length - 1);
                    composer.Reset();
                }
            }
            return;
        }

        // 특수 키 처리: Shift, CapsLock, Lang
        if (HandleSpecialKeys(normalKey)) return;
        if (normalKey == "Space") { nicknameInput.text += " "; composer.Reset(); return; }

        string inputKey = (isLeftShiftActive || isRightShiftActive) ? keyValue?.shiftKey : keyValue?.normalKey;
        if (string.IsNullOrEmpty(inputKey)) return;
        if (nicknameInput.text.Length >= maxCharacters) return;

        char jamoToCompose = '\0';
        string finalInput = "";
        bool shouldCompose = false;

        // 1. 한국어 모드 처리
        if (isKorean)
        {
            bool shiftActive = isLeftShiftActive || isRightShiftActive;
            string keyLower = normalKey.ToLower();

            if (shiftActive && koreanShiftKeyMap.ContainsKey(keyLower))
            {
                jamoToCompose = koreanShiftKeyMap[keyLower];
                shouldCompose = true;
            }
            else if (koreanKeyMap.ContainsKey(keyLower))
            {
                jamoToCompose = koreanKeyMap[keyLower];
                shouldCompose = true;
            }
            else
            {
                finalInput = inputKey;
            }
        }
        // 2. 영어 모드 처리
        else
        {
            if (inputKey.Length == 1 && char.IsLetter(inputKey[0]))
            {
                bool shiftActive = isLeftShiftActive || isRightShiftActive;
                bool toUpper = isCapsLockActive ^ shiftActive;
                finalInput = toUpper ? inputKey.ToUpper() : inputKey.ToLower();
            }
            else
            {
                finalInput = inputKey;
            }
        }

        // 3. 한글 조합 로직 실행 (핵심 해결 부분)
        if (shouldCompose)
        {
            string completedSyllable = composer.Add(jamoToCompose);

            // 1단계: 완성된 이전 글자 (completedSyllable)와 현재 조합 중인 글자(currentCompose)를
            // 합쳐서 닉네임 필드를 갱신합니다. 이 방식이 중복을 막습니다.
            string currentCompose = composer.Compose();

            if (!string.IsNullOrEmpty(currentCompose))
            {
                // 현재 조합 중인 글자가 있다면:
                if (nicknameInput.text.Length > 0)
                {
                    // 마지막 글자(이전 조합 글자 또는 완성 글자)를 지우고
                    nicknameInput.text = nicknameInput.text.Substring(0, nicknameInput.text.Length - 1);
                }

                // 완성된 이전 글자를 먼저 추가 (있다면)
                nicknameInput.text += completedSyllable;

                // 현재 조합 중인 글자를 이어서 추가
                nicknameInput.text += currentCompose;
            }
            else
            {
                // 현재 조합 중인 글자가 없는 경우 (보통 completedSyllable에만 값이 있음)
                nicknameInput.text += completedSyllable;
            }
        }
        // 4. 일반 문자 입력 로직 실행
        else if (!string.IsNullOrEmpty(finalInput))
        {
            // 조합 중인 한글이 있다면 먼저 완성
            string completedHangul = composer.Compose();
            if (!string.IsNullOrEmpty(completedHangul))
            {
                if (nicknameInput.text.Length > 0)
                    nicknameInput.text = nicknameInput.text.Substring(0, nicknameInput.text.Length - 1);

                nicknameInput.text += completedHangul;
            }

            // 일반 문자 추가
            nicknameInput.text += finalInput;
            composer.Reset();
        }

        // Shift 키는 한 번 사용 후 비활성화
        if (isLeftShiftActive || isRightShiftActive)
        {
            isLeftShiftActive = false;
            isRightShiftActive = false;
            UpdateKeyStates();
        }
    }

    private bool HandleSpecialKeys(string key)
    {
        if (key == "LeftShift") { isLeftShiftActive = !isLeftShiftActive; UpdateKeyStates(); return true; }
        if (key == "RightShift") { isRightShiftActive = !isRightShiftActive; UpdateKeyStates(); return true; }
        if (key == "CapsLock") { isCapsLockActive = !isCapsLockActive; UpdateKeyStates(); return true; }
        if (key == "Lang")
        {
            isKorean = !isKorean;
            UpdateKeyStates();
            composer.Reset();
            return true;
        }
        return false;
    }

    private void UpdateKeyStates()
    {
        if (leftShiftKeyText) leftShiftKeyText.color = isLeftShiftActive ? Color.gray : Color.white;
        if (rightShiftKeyText) rightShiftKeyText.color = isRightShiftActive ? Color.gray : Color.white;
        if (capsLockKeyText) capsLockKeyText.color = isCapsLockActive ? Color.gray : Color.white;
        if (langKeyText) langKeyText.text = isKorean ? "한" : "영";
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            ShowConfirmationPanel();
        }
    }

    public void ShowConfirmationPanel()
    {
        string completedHangul = composer.Compose();
        if (!string.IsNullOrEmpty(completedHangul))
        {
            if (nicknameInput.text.Length > 0)
                nicknameInput.text = nicknameInput.text.Substring(0, nicknameInput.text.Length - 1);
            nicknameInput.text += completedHangul;
            composer.Reset();
        }

        confirmationText.text = $"이름을 \"{nicknameInput.text}\"로 하시겠습니까?";
        confirmationPanel.SetActive(true);

        yesButton.onClick.RemoveAllListeners();
        yesButton.onClick.AddListener(() => StartCoroutine(ConfirmNickname()));

        noButton.onClick.RemoveAllListeners();
        noButton.onClick.AddListener(() => confirmationPanel.SetActive(false));
    }

    private IEnumerator ConfirmNickname()
    {
        fadeCanvasGroup.blocksRaycasts = true;

        float duration = 1.5f;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / duration);
            yield return null;
        }

        PlayerPrefs.SetString("PlayerName", nicknameInput.text);
        PlayerPrefs.Save();
        SceneManager.LoadScene("PrologueTemplate");
    }

    // ===== HangulComposer 클래스 (백스페이스 로직 추가) =====
    public class HangulComposer
    {
        private char cho = '\0';
        private char jung = '\0';
        private char jong = '\0';

        public static readonly string CHO = "ㄱㄲㄴㄷㄸㄹㅁㅂㅃㅅㅆㅇㅈㅉㅊㅋㅌㅍㅎ";
        public static readonly string JUNG = "ㅏㅐㅑㅒㅓㅔㅕㅖㅗㅘㅙㅚㅛㅜㅝㅞㅟㅠㅡㅢㅣ";
        public static readonly string JONG = "\0ㄱㄲㄳㄴㄵㄶㄷㄹㄺㄻㄼㄽㄾㄿㅀㅁㅂㅄㅅㅆㅇㅈㅊㅋㅌㅍㅎ";

        public bool IsNewSyllableStarted = false;

        private static readonly Dictionary<(char, char), char> CompoundCho = new Dictionary<(char, char), char>
        {
            { ('ㄱ', 'ㄱ'), 'ㄲ' }, { ('ㄷ', 'ㄷ'), 'ㄸ' }, { ('ㅂ', 'ㅂ'), 'ㅃ' }, { ('ㅅ', 'ㅅ'), 'ㅆ' }, { ('ㅈ', 'ㅈ'), 'ㅉ' }
        };

        private static readonly Dictionary<(char, char), char> CompoundJung = new Dictionary<(char, char), char>
        {
            { ('ㅗ', 'ㅏ'), 'ㅘ' }, { ('ㅗ', 'ㅐ'), 'ㅙ' }, { ('ㅗ', 'ㅣ'), 'ㅚ' },
            { ('ㅜ', 'ㅓ'), 'ㅝ' }, { ('ㅜ', 'ㅔ'), 'ㅞ' }, { ('ㅜ', 'ㅣ'), 'ㅟ' },
            { ('ㅡ', 'ㅣ'), 'ㅢ' }
        };

        private static readonly Dictionary<(char, char), char> CompoundJong = new Dictionary<(char, char), char>
        {
            { ('ㄱ', 'ㅅ'), 'ㄳ' }, { ('ㄴ', 'ㅈ'), 'ㄵ' }, { ('ㄴ', 'ㅎ'), 'ㄶ' },
            { ('ㄹ', 'ㄱ'), 'ㄺ' }, { ('ㄹ', 'ㅁ'), 'ㄻ' }, { ('ㄹ', 'ㅂ'), 'ㄼ' },
            { ('ㄹ', 'ㅅ'), 'ㄽ' }, { ('ㄹ', 'ㅌ'), 'ㄾ' }, { ('ㄹ', 'ㅍ'), 'ㄿ' },
            { ('ㄹ', 'ㅎ'), 'ㅀ' }, { ('ㅂ', 'ㅅ'), 'ㅄ' }
        };

        private static readonly Dictionary<char, (char, char)> SeparateJong = new Dictionary<char, (char, char)>
        {
            { 'ㄳ', ('ㄱ', 'ㅅ') }, { 'ㄵ', ('ㄴ', 'ㅈ') }, { 'ㄶ', ('ㄴ', 'ㅎ') },
            { 'ㄺ', ('ㄹ', 'ㄱ') }, { 'ㄻ', ('ㄹ', 'ㅁ') }, { 'ㄼ', ('ㄹ', 'ㅂ') },
            { 'ㄽ', ('ㄹ', 'ㅅ') }, { 'ㄾ', ('ㄹ', 'ㅌ') }, { 'ㄿ', ('ㄹ', 'ㅍ') },
            { 'ㅀ', ('ㄹ', 'ㅎ') }, { 'ㅄ', ('ㅂ', 'ㅅ') }
        };

        // 종성 분리 시 합쳐진 복합 종성에서 이전 상태로 되돌리기 위한 맵 (Backspace용)
        private static readonly Dictionary<char, (char, char)> DisassembleJong = new Dictionary<char, (char, char)>
        {
            { 'ㄲ', ('ㄱ', 'ㄱ') }, { 'ㅆ', ('ㅅ', 'ㅅ') },
            { 'ㄳ', ('ㄱ', 'ㅅ') }, { 'ㄵ', ('ㄴ', 'ㅈ') }, { 'ㄶ', ('ㄴ', 'ㅎ') },
            { 'ㄺ', ('ㄹ', 'ㄱ') }, { 'ㄻ', ('ㄹ', 'ㅁ') }, { 'ㄼ', ('ㄹ', 'ㅂ') },
            { 'ㄽ', ('ㄹ', 'ㅅ') }, { 'ㄾ', ('ㄹ', 'ㅌ') }, { 'ㄿ', ('ㄹ', 'ㅍ') },
            { 'ㅀ', ('ㄹ', 'ㅎ') }, { 'ㅄ', ('ㅂ', 'ㅅ') }
        };


        public string Add(char c)
        {
            IsNewSyllableStarted = false;
            string completedSyllable = "";

            bool isInputCho = CHO.Contains(c);
            bool isInputJung = JUNG.Contains(c);

            // 1. 초성 입력 (자음)
            if (isInputCho)
            {
                if (cho == '\0')
                {
                    cho = c;
                }
                else if (jung == '\0')
                {
                    if (CompoundCho.TryGetValue((cho, c), out char compoundCho))
                    {
                        cho = compoundCho;
                    }
                    else
                    {
                        completedSyllable = ComposeSingleCho();
                        Reset();
                        cho = c;
                        IsNewSyllableStarted = true;
                    }
                }
                else if (jong == '\0')
                {
                    jong = c;
                }
                else
                {
                    if (CompoundJong.TryGetValue((jong, c), out char compoundJong))
                    {
                        jong = compoundJong;
                    }
                    else
                    {
                        completedSyllable = Compose();
                        Reset();
                        cho = c;
                        IsNewSyllableStarted = true;
                    }
                }
            }
            // 2. 중성 입력 (모음)
            else if (isInputJung)
            {
                if (cho == '\0')
                {
                    cho = 'ㅇ';
                    jung = c;
                    IsNewSyllableStarted = true;
                }
                else if (jong == '\0')
                {
                    if (jung != '\0' && CompoundJung.TryGetValue((jung, c), out char compoundJung))
                    {
                        jung = compoundJung;
                    }
                    else
                    {
                        jung = c;
                    }
                }
                else // 2.3. 초+중+종 상태에서 모음 입력 시 종성 분리
                {
                    // 복합 종성 분리 (예: '밝' + 'ㅏ' -> '발' + '가')
                    if (SeparateJong.TryGetValue(jong, out var separated))
                    {
                        completedSyllable = ComposeSingleSyllable(cho, jung, separated.Item1); // '발' 완성
                        Reset();
                        cho = separated.Item2; // 'ㄱ'을 새 초성으로
                        jung = c; // 새 중성 'ㅏ'
                        IsNewSyllableStarted = true;
                    }
                    // 홑종성 분리 (예: '한' + 'ㅏ' -> '하' + '나')
                    else
                    {
                        char oldJong = jong;
                        jong = '\0'; // 종성 제거하여 현재 글자 완성
                        completedSyllable = Compose(); // '하' 완성
                        Reset();
                        cho = oldJong; // 종성을 새 초성으로
                        jung = c; // 새 중성 'ㅏ'
                        IsNewSyllableStarted = true;
                    }
                }
            }
            else // 3. 한글 자모가 아닌 경우
            {
                completedSyllable = Compose();
                Reset();
                IsNewSyllableStarted = true;
            }

            return completedSyllable;
        }

        // Backspace 처리를 위한 로직 추가
        public string RemoveLast()
        {
            if (jong != '\0') // 종성이 있는 경우
            {
                if (DisassembleJong.ContainsKey(jong)) // 복합 종성인 경우 (예: 'ㄳ' -> 'ㄱ')
                {
                    jong = DisassembleJong[jong].Item1;
                }
                else // 홑종성인 경우 (예: '안' -> '아')
                {
                    jong = '\0';
                }
            }
            else if (jung != '\0') // 중성만 있는 경우
            {
                // 복합 중성 분리 로직 (복잡하므로 여기서는 단순화하여 전체 제거)
                // 완벽한 구현을 위해서는 ㅘ -> ㅗ + ㅏ 분리 로직이 필요하나, 현재는 중성 전체 제거
                jung = '\0';

            }
            else if (cho != '\0') // 초성만 있는 경우
            {
                cho = '\0';
            }
            else
            {
                return ""; // 지울 글자가 없음
            }

            if (cho == '\0' && jung == '\0' && jong == '\0')
            {
                // 글자 하나가 완전히 지워진 상태
                return "SYLLABLE_REMOVED";
            }

            // 갱신된 글자를 반환
            return Compose();
        }


        public string Compose()
        {
            if (cho == '\0' && jung == '\0' && jong == '\0') return "";
            if (cho != '\0' && jung == '\0') return cho.ToString();

            if (cho != '\0' && jung != '\0')
            {
                int choIndex = CHO.IndexOf(cho);
                int jungIndex = JUNG.IndexOf(jung);
                int jongIndex = JONG.IndexOf(jong);

                char syllable = (char)(0xAC00 + (choIndex * 21 * 28) + (jungIndex * 28) + jongIndex);
                return syllable.ToString();
            }
            return "";
        }

        private string ComposeSingleCho()
        {
            return (cho != '\0' && jung == '\0' && jong == '\0') ? cho.ToString() : "";
        }

        private string ComposeSingleSyllable(char c, char j, char g)
        {
            if (c != '\0' && j != '\0')
            {
                int choIndex = CHO.IndexOf(c);
                int jungIndex = JUNG.IndexOf(j);
                int jongIndex = JONG.IndexOf(g);
                char syllable = (char)(0xAC00 + (choIndex * 21 * 28) + (jungIndex * 28) + jongIndex);
                return syllable.ToString();
            }
            return "";
        }

        public void Reset()
        {
            cho = jung = jong = '\0';
        }
    }
}
    