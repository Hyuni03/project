using UnityEngine;
using UnityEngine.UI;

public class AudioSectionController : MonoBehaviour
{
    [Header("UI 연결")]
    public GameObject Content2;
    public Slider BgmSlider;
    public Slider SfxSlider;

    [Header("기본값")]
    public float defaultBgmVolume = 0.5f;
    public float defaultSfxVolume = 0.5f;

    private bool isOpen = false;

    void Start()
    {
        // 초기 설정값 적용
        BgmSlider.value = defaultBgmVolume;
        SfxSlider.value = defaultSfxVolume;
    }

    public void ToggleContent()
    {
        isOpen = !isOpen;
        Content2.SetActive(isOpen); //"오디오 설정" 버튼 클릭 시 콘텐츠 열기/닫기
    }

    public void ResetToDefault() // 기본값 복원 버튼 눌렀을 때
    {
        BgmSlider.value = defaultBgmVolume;
        SfxSlider.value = defaultSfxVolume;
        Debug.Log("오디오 설정 → 기본값으로 복원됨");
    }
}