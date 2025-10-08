using UnityEngine;
using UnityEditor;         // 에디터 관련 기능 사용
using TMPro;               // TextMeshPro 관련 기능
using UnityEngine.UI;      // UI Button 접근

public class KeyboardKeyAutoSetter : MonoBehaviour
{
    // 메뉴에 새 항목 추가. 클릭 시 AutoAssignKeys 실행됨.
    [MenuItem("Tools/Auto Assign Keyboard Keys")]
    public static void AutoAssignKeys()
    {
        Debug.Log("키 자동 설정 시작");

        // 씬 내 모든 Button 컴포넌트를 가져옴
        Button[] buttons = GameObject.FindObjectsOfType<Button>();
        Debug.Log($"전체 버튼 개수: {buttons.Length}");

        foreach (Button button in buttons)
        {
            // null 체크
            if (button == null)
            {
                Debug.LogWarning("null 버튼 발견");
                continue;
            }

            // 버튼 자식 오브젝트에서 TextMeshPro 텍스트 컴포넌트 찾기
            TextMeshProUGUI tmpText = button.GetComponentInChildren<TextMeshProUGUI>();
            if (tmpText == null)
            {
                Debug.LogWarning($"버튼 '{button.name}'에 TMP 텍스트 없음");
                continue;
            }

            // 텍스트 앞뒤 공백 제거 후 변수에 저장
            string fullText = tmpText.text.Trim();
            Debug.Log($"버튼 '{button.name}' 텍스트: {fullText}");

            // 텍스트가 비어있으면 건너뜀
            if (string.IsNullOrEmpty(fullText)) continue;

            // KeyValue 컴포넌트를 버튼에서 찾거나 새로 추가
            KeyValue kv = button.GetComponent<KeyValue>();
            if (kv == null)
            {
                kv = button.gameObject.AddComponent<KeyValue>();
                Debug.Log($"KeyValue 추가됨: {button.name}");
            }

            // 텍스트 길이가 1인 경우 (ex. a, b, 1, @)
            if (fullText.Length == 1)
            {
                char c = fullText[0];

                // 알파벳이면 소문자/대문자로 나눠서 설정
                if (char.IsLetter(c))
                {
                    kv.normalKey = char.ToLower(c).ToString();
                    kv.shiftKey = char.ToUpper(c).ToString();
                }
                else
                {
                    // 알파벳이 아니면 둘 다 동일하게 설정
                    kv.normalKey = fullText;
                    kv.shiftKey = fullText;
                }
            }
            // 텍스트 길이가 2인 경우 (ex. `~`, 1!, 2@ 등)
            else if (fullText.Length == 2)
            {
                kv.normalKey = fullText[0].ToString();  // 일반 키
                kv.shiftKey = fullText[1].ToString();   // 쉬프트 키
            }
            // 그 외 (예외 케이스)
            else
            {
                kv.normalKey = fullText;
                kv.shiftKey = fullText;
                Debug.LogWarning($"예상 밖 텍스트 형식: '{fullText}'");
            }

            // 에디터에 변경 표시 (저장 대상으로 표시됨)
            EditorUtility.SetDirty(kv);
        }

        Debug.Log("모든 버튼에 KeyValue 자동 설정 완료!");
    }
}
