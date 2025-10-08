using UnityEngine;
using UnityEngine.UI;

public class AudioSectionController : MonoBehaviour
{
    [Header("UI 연결")]
    public GameObject Content2;           // 오디오 설정 콘텐츠 영역
    public Slider BgmSlider;              // BGM 볼륨 조절 슬라이더
    public Slider SfxSlider;              // 효과음 볼륨 조절 슬라이더

    [Header("기본값")]
    public float defaultBgmVolume = 0.5f; // 기본 BGM 볼륨
    public float defaultSfxVolume = 0.5f; // 기본 효과음 볼륨

    [Header("오디오 소스")]
    public AudioSource bgmSource;         // 실제로 재생할 BGM 소스 (BGMPlayer에 붙은 AudioSource)

    private bool isOpen = false;          // 콘텐츠 열림 상태

    void Start()
    {
        Debug.Log("🟢 AudioSectionController.Start() 호출됨");

        if (bgmSource == null)
        {
            if (BGMPlayer.Instance != null)
            {
                bgmSource = BGMPlayer.Instance.audioSource;
                Debug.Log("✅ bgmSource 연결 성공");
            }
            else
            {
                Debug.LogWarning("❌ BGMPlayer.Instance가 null입니다");
            }
        }

        BgmSlider.value = defaultBgmVolume;
        SfxSlider.value = defaultSfxVolume;

        BgmSlider.onValueChanged.AddListener(SetBgmVolume);
    }


    // "오디오 설정" 버튼 클릭 시 콘텐츠 열기/닫기
    public void ToggleContent()
    {
        isOpen = !isOpen;
        Content2.SetActive(isOpen);
    }

    // BGM 볼륨 슬라이더가 변경될 때 호출되는 함수
    public void SetBgmVolume(float volume)
    {
        Debug.Log($"🔊 SetBgmVolume 호출됨: {volume}");

        if (bgmSource != null)
        {
            bgmSource.volume = volume;
            Debug.Log($"🎵 볼륨 적용됨: {bgmSource.volume}");
        }
        else
        {
            Debug.LogWarning("❗ bgmSource가 null입니다");
        }
    }


    // "기본값 복원" 버튼 클릭 시
    public void ResetToDefault()
    {
        BgmSlider.value = defaultBgmVolume;
        SfxSlider.value = defaultSfxVolume;

        // BGM 볼륨도 즉시 적용
        SetBgmVolume(defaultBgmVolume);

        Debug.Log("오디오 설정 → 기본값으로 복원됨");
    }
}
