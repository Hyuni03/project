using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KeyboardInputManager : MonoBehaviour
{
    // 닉네임을 입력받는 TMP_InputField
    public TMP_InputField nicknameInput;

    // 기능 키 상태 표시 텍스트들
    public TMP_Text leftShiftKeyText;    // 왼쪽 Shift 키의 텍스트 (색상 변경용)
    public TMP_Text rightShiftKeyText;   // 오른쪽 Shift 키의 텍스트 (색상 변경용)
    public TMP_Text capsLockKeyText;     // CapsLock 키의 텍스트 (색상 변경용)
    public TMP_Text langKeyText;         // 언어 전환 키의 텍스트 (한/영 표시용)

    // 모든 키보드 버튼을 담는 배열
    public Button[] keyboardButtons;

    // 현재 키 상태 변수들
    private bool isLeftShiftActive = false;   // 왼쪽 Shift 키가 눌렸는지 여부
    private bool isRightShiftActive = false;  // 오른쪽 Shift 키가 눌렸는지 여부
    private bool isCapsLockActive = false;    // CapsLock이 활성화되었는지 여부
    private bool isKorean = true;             // 현재 입력 언어가 한글인지 여부

    // 입력 가능한 최대 글자 수 제한
    private int maxCharacters = 12;

    // 영어 알파벳 → 한글 자음/모음 매핑 딕셔너리
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

    // 시작 시 모든 버튼에 이벤트 등록
    void Start()
    {
        // 버튼이 미리 등록되지 않았다면 태그로 자동 탐색
        if (keyboardButtons == null || keyboardButtons.Length == 0)
        {
            GameObject keyboardPanel = GameObject.FindGameObjectWithTag("Keyboard");
            if (keyboardPanel != null)
            {
                keyboardButtons = keyboardPanel.GetComponentsInChildren<Button>();
            }
        }

        Debug.Log($"찾은 키보드 버튼 개수: {(keyboardButtons != null ? keyboardButtons.Length.ToString() : "NULL")}");

        // 버튼 연결 실패 시 경고
        if (keyboardButtons == null || keyboardButtons.Length == 0)
        {
            Debug.LogError("keyboardButtons 배열이 설정되지 않음! Inspector에서 버튼을 할당했는지 확인하세요!");
            return;
        }

        // 버튼마다 클릭 이벤트 연결
        foreach (Button button in keyboardButtons)
        {
            if (button == null) continue;

            KeyValue keyValue = button.GetComponent<KeyValue>();
            if (keyValue == null)
            {
                Debug.LogError($"버튼 '{button.name}'에 KeyValue 컴포넌트가 없음!");
                continue;
            }

            // 기존 리스너 제거 후 새로 연결
            button.onClick.RemoveAllListeners();
            Button capturedButton = button;
            button.onClick.AddListener(() => OnKeyPress(capturedButton));
        }
    }

    // 사용자가 가상 키보드에서 버튼을 클릭했을 때 호출되는 함수
    // 클릭된 버튼(Button)에 따라 어떤 키인지 판별하고, 그에 맞는 동작을 수행함
    public void OnKeyPress(Button button)
    {
        // nicknameInput이 연결되어 있지 않으면 오류 출력 후 실행 중지
        if (nicknameInput == null)
        {
            Debug.LogError("nicknameInput이 설정되지 않음!");  // 입력 필드 연결 누락 시 디버그용 로그
            return;
        }

        // 클릭된 버튼에서 KeyValue 컴포넌트를 가져옴 (이 컴포넌트에는 키 값 정보가 담겨 있음)
        KeyValue keyValue = button.GetComponent<KeyValue>();
        if (keyValue == null)
        {
            Debug.LogError($"버튼 '{button.name}'에 KeyValue 컴포넌트가 없음!");  // 키 정보가 없으면 오류 출력
            return;
        }

        // 누른 키의 기본 값(normalKey)을 가져옴 (예: A, B, Backspace 등)
        string normalKey = keyValue.normalKey;

        // 눌린 키를 디버그 로그에 출력 (테스트나 로그 확인용)
        Debug.Log($"눌린 키: {normalKey}");

        // 기능 키별 처리 시작 ----------------------------------------

        // Backspace 키가 눌렸을 경우
        if (normalKey == "Backspace")
        {
            // 입력된 텍스트가 1글자 이상일 경우 마지막 글자를 제거함
            if (nicknameInput.text.Length > 0)
                nicknameInput.text = nicknameInput.text.Substring(0, nicknameInput.text.Length - 1);
            return;  // 키 입력 처리 종료
        }

        // 왼쪽 Shift 키가 눌렸을 경우
        if (normalKey == "LeftShift")
        {
            // 왼쪽 Shift 키 상태를 반전시킴 (눌림/해제 전환)
            isLeftShiftActive = !isLeftShiftActive;

            // 키 상태 시각적으로 반영 (색상 등 변경)
            UpdateKeyStates();
            return;
        }

        // 오른쪽 Shift 키가 눌렸을 경우
        if (normalKey == "RightShift")
        {
            // 오른쪽 Shift 키 상태를 반전시킴
            isRightShiftActive = !isRightShiftActive;

            // 키 상태 UI 갱신
            UpdateKeyStates();
            return;
        }

        // CapsLock 키가 눌렸을 경우
        if (normalKey == "CapsLock")
        {
            // 디버그 로그 출력: CapsLock이 눌렸음을 표시
            Debug.Log("CapsLock 키 눌림");

            // CapsLock 상태 반전 (켜짐 ↔ 꺼짐)
            isCapsLockActive = !isCapsLockActive;

            // 상태 변화 로그 출력
            Debug.Log($"[CapsLock] 상태 전환됨: {isCapsLockActive}");

            // 키 상태 UI 갱신
            UpdateKeyStates();
            return;
        }

        // Lang 키가 눌렸을 경우 (한/영 전환)
        if (normalKey == "Lang")
        {
            // 디버그 로그 출력: Lang 키 눌림
            Debug.Log("Lang 키 눌림");

            // isKorean 값을 반전 → 현재 언어를 한글/영어로 전환
            isKorean = !isKorean;

            // 키 상태 UI 갱신 (Lang 텍스트도 한/영으로 바뀜)
            UpdateKeyStates();
            return;
        }

        // Space 키가 눌렸을 경우
        if (normalKey == "Space")
        {
            // 입력 필드에 공백 추가
            nicknameInput.text += " ";
            return;
        }

        // 일반 키 입력 처리
        string inputKey = (isLeftShiftActive || isRightShiftActive) ? keyValue.shiftKey : keyValue.normalKey;

        if (string.IsNullOrEmpty(inputKey)) return;

        // 최대 글자 수 초과 방지
        if (nicknameInput.text.Length >= maxCharacters)
            return;

        // 한글 모드
        if (isKorean)
        {
            // 알파벳 입력 → 한글 변환
            if (inputKey.Length == 1 && koreanKeyMap.ContainsKey(inputKey.ToLower()))
            {
                inputKey = koreanKeyMap[inputKey.ToLower()];
            }
        }
        else
        {
            // 영어 모드 - 대소문자 처리
            if (inputKey.Length == 1 && char.IsLetter(inputKey[0]))
            {
                bool shiftActive = isLeftShiftActive || isRightShiftActive;
                bool toUpper = isCapsLockActive ^ shiftActive;                  // XOR: 하나만 켜졌을 때 대문자
                inputKey = toUpper ? inputKey.ToUpper() : inputKey.ToLower();
            }
        }

        // 입력 필드에 글자 추가
        nicknameInput.text += inputKey;

        // Shift는 한 번만 적용되므로 해제
        if (isLeftShiftActive || isRightShiftActive)
        {
            isLeftShiftActive = false;
            isRightShiftActive = false;
            UpdateKeyStates();
        }
    }

    // Shift, CapsLock, Lang 상태를 UI에 반영
    private void UpdateKeyStates()
    {
        // 색상으로 활성/비활성 표현
        leftShiftKeyText.color = isLeftShiftActive ? Color.gray : Color.white;
        rightShiftKeyText.color = isRightShiftActive ? Color.gray : Color.white;
        capsLockKeyText.color = isCapsLockActive ? Color.gray : Color.white;

        // 언어 표시 (한 / 영)
        langKeyText.text = isKorean ? "한" : "영";
    }
}