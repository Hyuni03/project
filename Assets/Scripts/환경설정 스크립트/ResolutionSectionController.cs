// 해상도 섹션 UI 제어 스크립트
// 해상도 목록을 펼치고 닫을 수 있으며, 기본값 복원 기능도 포함됨

using UnityEngine;
using UnityEngine.UI;

public class ResolutionSectionController : MonoBehaviour
{
    [Header("UI 오브젝트 연결")]
    public GameObject Content1; // 해상도 목록을 담고 있는 콘텐츠 영역

    public Toggle Toggle_1920x1080;
    public Toggle Toggle_1280x720;
    public Toggle Toggle_1600x900;
    public Toggle Toggle_1024x768;

    [Header("기본 해상도 설정")]
    public int defaultResolutionIndex = 0; // 기본 해상도로 사용할 인덱스 (0 = 1920x1080)

    private Toggle[] resolutionToggles; // 토글 배열로 관리
    private bool isOpen = false; // 콘텐츠 열림 상태

    void Start()
    {
        // 배열로 정리하여 코드 간결하게
        resolutionToggles = new Toggle[] {
            Toggle_1920x1080,
            Toggle_1280x720,
            Toggle_1600x900,
            Toggle_1024x768
        };

        // 시작 시 기본값 복원
        ResetToDefault();
    }

    // "해상도 및 화면 크기" 버튼 클릭 시 콘텐츠 열기/닫기
    public void ToggleContent()
    {
        isOpen = !isOpen;
        Content1.SetActive(isOpen);
    }

    // 기본값 복원 버튼 눌렀을 때 호출되는 함수
    public void ResetToDefault()
    {
        for (int i = 0; i < resolutionToggles.Length; i++)
        {
            resolutionToggles[i].isOn = (i == defaultResolutionIndex);

            if (i == defaultResolutionIndex)
            {
                // 선택된 토글에 포커스 설정 (시각적 강조 효과 가능)
                UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(resolutionToggles[i].gameObject);
            }
        }

        Debug.Log("해상도 기본값으로 복원 완료");
    }
}
