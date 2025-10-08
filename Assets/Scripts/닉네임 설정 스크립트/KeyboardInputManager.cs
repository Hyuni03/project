using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class KeyboardInputManager : MonoBehaviour
{
    // 닉네임 확인 패널 관련 UI 요소
    [Header("닉네임 확인 패널 관련")]
    public GameObject confirmationPanel;
    public TMP_Text confirmationText;
    public Button yesButton;
    public Button noButton;

    // 키보드 및 페이드 관련 UI 요소
    [Header("페이드 관련")]
    public TMP_InputField nicknameInput;
    public TMP_Text leftShiftKeyText;
    public TMP_Text rightShiftKeyText;
    public TMP_Text capsLockKeyText;
    public TMP_Text langKeyText;
    public CanvasGroup fadeCanvasGroup;
    public Button[] keyboardButtons;

    // 상태 변수
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
        // 키보드 버튼 자동 탐색
        if (keyboardButtons == null || keyboardButtons.Length == 0)
        {
            GameObject keyboardPanel = GameObject.FindGameObjectWithTag("Keyboard");
            if (keyboardPanel != null)
                keyboardButtons = keyboardPanel.GetComponentsInChildren<Button>();
        }

        // 키보드 버튼에 클릭 리스너 연결
        foreach (Button button in keyboardButtons)
        {
            if (button == null) continue;
            button.onClick.RemoveAllListeners();
            Button capturedButton = button;
            button.onClick.AddListener(() => OnKeyPress(capturedButton));
        }

        // 페이드 캔버스 초기 설정
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

        // 1. 가상 키보드 Enter 키 처리
        if (normalKey == "Enter")
        {
            OnEnterPress();
            return;
        }

        // 특수 키 처리: Backspace
        if (normalKey == "Backspace")
        {
            if (nicknameInput.text.Length > 0)
            {
                string removedSyllable = composer.RemoveLast();

                if (removedSyllable == "SYLLABLE_REMOVED")
                {
                    // 완성된 글자 하나를 지움
                    nicknameInput.text = nicknameInput.text.Substring(0, nicknameInput.text.Length - 1);
                }
                else if (!string.IsNullOrEmpty(removedSyllable))
                {
                    // 조합 중이던 글자가 이전 상태로 돌아간 경우 (예: '닭' -> '달', '달' -> '다')
                    if (nicknameInput.text.Length > 0)
                    {
                        nicknameInput.text = nicknameInput.text.Substring(0, nicknameInput.text.Length - 1) + removedSyllable;
                    }
                }
                else
                {
                    // 마지막 자모가 완전히 제거되어 조합기가 리셋되는 경우 (예: 'ㄱ'을 지울 때)
                    nicknameInput.text = nicknameInput.text.Substring(0, nicknameInput.text.Length - 1);
                    composer.Reset();
                }
            }
            return;
        }

        // 특수 키 처리: Shift, CapsLock, Lang
        if (HandleSpecialKeys(normalKey)) return;

        string inputKey = (isLeftShiftActive || isRightShiftActive) ? keyValue?.shiftKey : keyValue?.normalKey;

        // Space 키 처리
        if (normalKey == "Space")
        {
            inputKey = " ";
        }

        if (string.IsNullOrEmpty(inputKey)) return;
        if (nicknameInput.text.Length >= maxCharacters) return;

        char jamoToCompose = '\0';
        string finalInput = "";
        bool shouldCompose = false;

        // 1. 한국어 모드 처리: 입력된 키를 자모로 매핑
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
                // 한글 자모 매핑에 없는 경우, 일반 입력으로 처리
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

        // 3. 한글 조합 로직 실행
        if (shouldCompose)
        {
            // Add를 호출하여 완성된 글자(completedSyllable)와 현재 조합 상태(currentCompose)를 가져옴
            string completedSyllable = composer.Add(jamoToCompose);
            string currentCompose = composer.Compose();

            // ⭐ 덮어쓰기 로직
            if (nicknameInput.text.Length > 0)
            {
                char lastChar = nicknameInput.text[nicknameInput.text.Length - 1];

                // 마지막 글자가 한글/자모이고, 현재 조합 중인 상태이거나 새로운 완성 글자가 있다면
                if (HangulComposer.IsHangulOrJamo(lastChar))
                {
                    // 완성 글자가 넘어왔는데, 그것이 조합기를 거친 글자이거나 (e.g., '아' -> 'ㅏ'로 분리 시)
                    // 현재 조합 중인 글자가 있을 때만 마지막 글자를 제거합니다.
                    if (!string.IsNullOrEmpty(completedSyllable) || !string.IsNullOrEmpty(currentCompose))
                    {
                        nicknameInput.text = nicknameInput.text.Substring(0, nicknameInput.text.Length - 1);
                    }
                }
            }

            // 완성된 글자(completedSyllable)와 현재 조합 중인 글자(currentCompose)를 텍스트 필드에 추가합니다.
            // completedSyllable은 이전에 확정된 글자, currentCompose는 현재 조합 중인 글자입니다.
            nicknameInput.text += completedSyllable;
            nicknameInput.text += currentCompose;
        }
        // 4. 일반 문자 입력 로직 실행 (숫자, 영어 모드 문자, 특수 기호, Space)
        else if (!string.IsNullOrEmpty(finalInput))
        {
            // 현재 조합 중이던 한글이 있다면 완성하여 필드에 추가
            string completedHangul = composer.Compose();
            if (!string.IsNullOrEmpty(completedHangul))
            {
                if (nicknameInput.text.Length > 0)
                {
                    char lastChar = nicknameInput.text[nicknameInput.text.Length - 1];

                    // 마지막 글자가 자모나 조합 중인 글자였으면 대체
                    if (HangulComposer.IsHangulOrJamo(lastChar))
                    {
                        nicknameInput.text = nicknameInput.text.Substring(0, nicknameInput.text.Length - 1);
                        nicknameInput.text += completedHangul;
                    }
                    else
                    {
                        // 일반 문자 뒤에 조합된 글자를 추가
                        nicknameInput.text += completedHangul;
                    }
                }
                else
                {
                    nicknameInput.text += completedHangul;
                }
            }

            // 일반 문자를 닉네임 필드에 추가
            nicknameInput.text += finalInput;

            // 일반 문자를 입력하면 한글 조합 상태는 리셋
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

    // 가상 키보드 Enter 및 물리 키보드 Enter 처리를 통합하는 함수
    private void OnEnterPress()
    {
        // 1. 현재 조합 중이던 한글이 있다면 완성하고 닉네임 필드 갱신
        string completedHangul = composer.Compose();

        if (!string.IsNullOrEmpty(completedHangul))
        {
            if (nicknameInput.text.Length > 0)
            {
                char lastChar = nicknameInput.text[nicknameInput.text.Length - 1];

                if (HangulComposer.IsHangulOrJamo(lastChar))
                {
                    // 조합 중인 글자(자모 또는 조합형 한글)가 있었다면 최종 확정 글자로 대체
                    nicknameInput.text = nicknameInput.text.Substring(0, nicknameInput.text.Length - 1) + completedHangul;
                }
                else
                {
                    // 마지막 글자가 자모가 아니었다면 그냥 추가
                    nicknameInput.text += completedHangul;
                }
                composer.Reset();
            }
            else
            {
                nicknameInput.text += completedHangul;
                composer.Reset();
            }
        }

        // 2. 확정 패널을 띄웁니다.
        if (!string.IsNullOrEmpty(nicknameInput.text) && nicknameInput.text.Trim().Length > 0)
        {
            ShowConfirmationPanel();
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
    }

    public void OnValueChangedCheck(string currentText)
    {
    }

    // On End Edit 이벤트 연결용 함수 (물리적 Enter 키 처리 담당)
    public void OnEndEditNickname(string finalName)
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            OnEnterPress();
        }
    }


    public void ShowConfirmationPanel()
    {
        string completedHangul = composer.Compose();
        if (!string.IsNullOrEmpty(completedHangul))
        {
            if (nicknameInput.text.Length > 0)
            {
                char lastChar = nicknameInput.text[nicknameInput.text.Length - 1];
                if (HangulComposer.IsHangulOrJamo(lastChar))
                {
                    nicknameInput.text = nicknameInput.text.Substring(0, nicknameInput.text.Length - 1);
                }
            }
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

    // ===== HangulComposer 클래스 (최종 수정됨) =====
    public class HangulComposer
    {
        private char cho = '\0';
        private char jung = '\0';
        private char jong = '\0';

        public char Cho
        {
            get { return cho; }
        }

        // 한글 자모 정의
        public static readonly string CHO = "ㄱㄲㄴㄷㄸㄹㅁㅂㅃㅅㅆㅇㅈㅉㅊㅋㅌㅍㅎ";
        public static readonly string JUNG = "ㅏㅐㅑㅒㅓㅔㅕㅖㅗㅘㅙㅚㅛㅜㅝㅞㅟㅠㅡㅢㅣ";
        public static readonly string JONG = "\0ㄱㄲㄳㄴㄵㄶㄷㄹㄺㄻㄼㄽㄾㄿㅀㅁㅂㅄㅅㅆㅇㅈㅊㅋㅌㅍㅎ"; // 첫 번째는 공백(받침 없음)

        public bool IsNewSyllableStarted = false;

        // 쌍자음 조합
        private static readonly Dictionary<(char, char), char> CompoundCho = new Dictionary<(char, char), char>
        {
            { ('ㄱ', 'ㄱ'), 'ㄲ' }, { ('ㄷ', 'ㄷ'), 'ㄸ' }, { ('ㅂ', 'ㅂ'), 'ㅃ' },
            { ('ㅅ', 'ㅅ'), 'ㅆ' }, { ('ㅈ', 'ㅈ'), 'ㅉ' }
        };

        // 복합 모음 조합
        private static readonly Dictionary<(char, char), char> CompoundJung = new Dictionary<(char, char), char>
        {
            { ('ㅗ', 'ㅏ'), 'ㅘ' }, { ('ㅗ', 'ㅐ'), 'ㅙ' }, { ('ㅗ', 'ㅣ'), 'ㅚ' },
            { ('ㅜ', 'ㅓ'), 'ㅝ' }, { ('ㅜ', 'ㅔ'), 'ㅞ' }, { ('ㅜ', 'ㅣ'), 'ㅟ' },
            { ('ㅡ', 'ㅣ'), 'ㅢ' }
        };

        // 겹받침 조합
        private static readonly Dictionary<(char, char), char> CompoundJong = new Dictionary<(char, char), char>
        {
            { ('ㄱ', 'ㅅ'), 'ㄳ' }, { ('ㄴ', 'ㅈ'), 'ㄵ' }, { ('ㄴ', 'ㅎ'), 'ㄶ' },
            { ('ㄹ', 'ㄱ'), 'ㄺ' }, { ('ㄹ', 'ㅁ'), 'ㄻ' }, { ('ㄹ', 'ㅂ'), 'ㄼ' },
            { ('ㄹ', 'ㅅ'), 'ㄽ' }, { ('ㄹ', 'ㅌ'), 'ㄾ' }, { ('ㄹ', 'ㅍ'), 'ㄿ' },
            { ('ㄹ', 'ㅎ'), 'ㅀ' }, { ('ㅂ', 'ㅅ'), 'ㅄ' }
        };

        // 종성 분리 (다음 초성으로 사용될 수 있는 경우)
        private static readonly Dictionary<char, (char, char)> SeparateJong = new Dictionary<char, (char, char)>
        {
            { 'ㄳ', ('ㄱ', 'ㅅ') }, { 'ㄵ', ('ㄴ', 'ㅈ') }, { 'ㄶ', ('ㄴ', 'ㅎ') },
            { 'ㄺ', ('ㄹ', 'ㄱ') }, { 'ㄻ', ('ㄹ', 'ㅁ') }, { 'ㄼ', ('ㄹ', 'ㅂ') },
            { 'ㄽ', ('ㄹ', 'ㅅ') }, { 'ㄾ', ('ㄹ', 'ㅌ') }, { 'ㄿ', ('ㄹ', 'ㅍ') },
            { 'ㅀ', ('ㄹ', 'ㅎ') }, { 'ㅄ', ('ㅂ', 'ㅅ') }
        };

        // 받침 분해 (BackSpace 시)
        private static readonly Dictionary<char, char> DisassembleJong = new Dictionary<char, char>
        {
            { 'ㄲ', 'ㄱ' }, { 'ㅆ', 'ㅅ' },
            { 'ㄳ', 'ㄱ' }, { 'ㄵ', 'ㄴ' }, { 'ㄶ', 'ㄴ' },
            { 'ㄺ', 'ㄹ' }, { 'ㄻ', 'ㄹ' }, { 'ㄼ', 'ㄹ' },
            { 'ㄽ', 'ㄹ' }, { 'ㄾ', 'ㄹ' }, { 'ㄿ', 'ㄹ' },
            { 'ㅀ', 'ㄹ' }, { 'ㅄ', 'ㅂ' }
        };

        // 한글 완성형 또는 자모인지 확인
        public static bool IsHangulOrJamo(char c)
        {
            return (c >= 0xAC00 && c <= 0xD7A3) || (c >= 0x1100 && c <= 0x11FF) || (c >= 0x3130 && c <= 0x318F);
        }

        // 자모만 있는지 확인 (초성, 중성)
        public static bool IsJamoOnly(char c)
        {
            return CHO.Contains(c) || JUNG.Contains(c);
        }

        // 한글 완성형인지 확인
        public static bool IsHangul(char c)
        {
            return c >= 0xAC00 && c <= 0xD7A3;
        }

        public string Add(char c)
        {
            IsNewSyllableStarted = false;
            string completedSyllable = "";

            bool isInputCho = CHO.Contains(c);
            bool isInputJung = JUNG.Contains(c);

            // 1. 초성 입력 (자음)
            if (isInputCho)
            {
                if (cho == '\0') // 1.1. 초성 없음 (새 글자 시작)
                {
                    // 중성만 있는 상태였다면, 이전 중성을 확정하고 리셋 (문제 2 해결)
                    if (jung != '\0')
                    {
                        completedSyllable = jung.ToString();
                        Reset();
                        IsNewSyllableStarted = true;
                    }

                    cho = c; // 새 초성 시작
                }
                else if (jung == '\0') // 1.2. 초성만 있음 (쌍자음 시도)
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
                else if (jong == '\0') // 1.3. 초+중성 상태 (종성으로 합치기)
                {
                    jong = c;
                }
                else // 1.4. 초+중+종성 상태 (겹받침 시도 또는 다음 글자 시작)
                {
                    if (CompoundJong.TryGetValue((jong, c), out char compoundJong))
                    {
                        jong = compoundJong; // 겹받침으로 갱신
                    }
                    else
                    {
                        completedSyllable = Compose();
                        Reset();
                        cho = c; // 새 자음을 새 초성으로 설정
                        IsNewSyllableStarted = true;
                    }
                }
            }
            // 2. 중성 입력 (모음)
            else if (isInputJung)
            {
                if (cho == '\0') // 2.1. 초성 없음 (모음 단독 출력)
                {
                    if (jung == '\0')
                    {
                        // 모음 단독 입력: 현재 모음을 조합 중인 상태로 유지 (ㅏㅏㅏㅏㅏ 처리를 위해)
                        jung = c;
                    }
                    else
                    {
                        if (CompoundJung.TryGetValue((jung, c), out char compoundJung))
                        {
                            jung = compoundJung; // 복합 모음 조합 (예: ㅗ + ㅏ -> ㅘ)
                        }
                        else
                        {
                            // 조합 불가능: 이전 모음 확정 후 새 모음 시작 (문제 1 해결)
                            completedSyllable = jung.ToString();
                            Reset();
                            jung = c; // 새 모음으로 조합 시작
                            IsNewSyllableStarted = true;
                        }
                    }
                }
                else if (jong == '\0') // 2.2. 초성만 있거나 초+중성 상태
                {
                    if (jung == '\0')
                    {
                        jung = c; // 초성 + 모음
                    }
                    else
                    {
                        if (CompoundJung.TryGetValue((jung, c), out char compoundJung))
                        {
                            jung = compoundJung; // 복합 모음 합치기
                        }
                        else
                        {
                            // 조합 불가능: 기존 글자 완성 후 Reset, 새 모음으로 새 조합 시작 (문제 1, 3 해결)
                            completedSyllable = Compose();
                            Reset();
                            jung = c; // 새 모음으로 새 조합 시작
                            IsNewSyllableStarted = true;
                        }
                    }
                }
                else // 2.3. 초+중+종성 상태 (종성 분리하여 다음 글자 시작)
                {
                    // 종성 분리 로직 (예: '닭'에서 'ㄱ'을 다음 글자의 초성으로)
                    char newCho;
                    char finalJong;

                    if (SeparateJong.TryGetValue(jong, out var separated))
                    {
                        finalJong = separated.Item1;
                        newCho = separated.Item2;
                    }
                    else
                    {
                        finalJong = '\0';
                        newCho = jong;
                    }

                    completedSyllable = ComposeSingleSyllable(cho, jung, finalJong);
                    Reset();
                    cho = newCho;
                    jung = c;
                    IsNewSyllableStarted = true;
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

        public string RemoveLast()
        {
            if (jong != '\0')
            {
                if (DisassembleJong.TryGetValue(jong, out char disassembledJong))
                {
                    jong = disassembledJong;
                }
                else
                {
                    jong = '\0';
                }
            }
            else if (jung != '\0')
            {
                // 복합 모음 분해 로직 (미구현 상태이므로, 일단 통째로 제거)
                jung = '\0';
            }
            else if (cho != '\0')
            {
                // 쌍자음 초성 분해 로직
                if (DisassembleJong.TryGetValue(cho, out char disassembledCho))
                {
                    cho = disassembledCho;
                }
                else
                {
                    cho = '\0';
                }
            }
            else
            {
                return "";
            }

            // 모든 자모가 제거되었다면, 이전에 완성된 글자 하나를 지워야 함
            if (cho == '\0' && jung == '\0' && jong == '\0')
            {
                return "SYLLABLE_REMOVED";
            }

            return Compose();
        }


        public string Compose()
        {
            if (cho == '\0' && jung == '\0' && jong == '\0') return "";

            // 초성만 있거나 중성만 있는 경우 (ㅏㅏㅏㅏ 처리를 위해 중성도 반환)
            if (cho != '\0' && jung == '\0' && jong == '\0') return cho.ToString();
            if (cho == '\0' && jung != '\0' && jong == '\0') return jung.ToString();

            if (cho != '\0' && jung != '\0')
            {
                int choIndex = CHO.IndexOf(cho);
                int jungIndex = JUNG.IndexOf(jung);
                int jongIndex = JONG.IndexOf(jong);

                // 유효한 인덱스 체크
                if (choIndex == -1 || jungIndex == -1 || jongIndex == -1)
                {
                    return "";
                }

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

                if (choIndex == -1 || jungIndex == -1 || jongIndex == -1)
                {
                    return "";
                }

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