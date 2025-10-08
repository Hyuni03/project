using UnityEngine;
using UnityEngine.UI;

public class AudioSectionController : MonoBehaviour
{
    [Header("UI 연결")]
    public GameObject Content2;            // 오디오 설정 콘텐츠 영역
    public Slider BgmSlider;               // BGM 볼륨 조절 슬라이더
    public Slider SfxSlider;               // 효과음 볼륨 조절 슬라이더

    [Header("기본값")]
    public float defaultBgmVolume = 0.5f;  // 기본 BGM 볼륨
    public float defaultSfxVolume = 0.5f;  // 기본 효과음 볼륨

    [Header("오디오 소스")]
    public AudioSource bgmSource;          // 실제로 재생할 BGM 소스 (BGMPlayer에 붙은 AudioSource)
    public AudioSource sfxSource;          // 효과음을 재생할 AudioSource (UI 클릭 등)

    [Header("효과음 클립")]
    public AudioClip buttonClickClip;      // 버튼 클릭 효과음 클립

    private bool isOpen = false;           // 콘텐츠 열림 상태

    private const string PREF_BGM = "BgmVolume";
    private const string PREF_SFX = "SfxVolume";

    void Start()
    {
        Debug.Log("🎧 AudioSectionController.Start() 호출됨");

        // ✅ BGM 소스 연결
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

        // ✅ 저장된 볼륨 불러오기 (없으면 기본값 사용)
        float savedBgmVolume = PlayerPrefs.GetFloat(PREF_BGM, defaultBgmVolume);
        float savedSfxVolume = PlayerPrefs.GetFloat(PREF_SFX, defaultSfxVolume);

        BgmSlider.value = savedBgmVolume;
        SfxSlider.value = savedSfxVolume;

        // ✅ 슬라이더 값 변경 시 호출될 함수 등록
        BgmSlider.onValueChanged.AddListener(SetBgmVolume);
        SfxSlider.onValueChanged.AddListener(SetSfxVolume);

        // ✅ 초기 볼륨 적용
        SetBgmVolume(savedBgmVolume);
        SetSfxVolume(savedSfxVolume);

        // ✅ 모든 버튼에 효과음 자동 적용
        AddSfxToAllButtons();
    }

    // "오디오 설정" 버튼 클릭 시 콘텐츠 열기/닫기
    public void ToggleContent()
    {
        isOpen = !isOpen;
        Content2.SetActive(isOpen);

        // 설정창 열기/닫기에도 효과음 재생
        PlayClickSound(buttonClickClip);
    }

    // BGM 볼륨 슬라이더 변경 시 호출
    public void SetBgmVolume(float volume)
    {
        if (bgmSource != null)
        {
            bgmSource.volume = volume;
            PlayerPrefs.SetFloat(PREF_BGM, volume); // 볼륨 저장
        }
    }

    // SFX 볼륨 슬라이더 변경 시 호출
    public void SetSfxVolume(float volume)
    {
        if (sfxSource != null)
        {
            sfxSource.volume = volume;
            PlayerPrefs.SetFloat(PREF_SFX, volume); // 효과음 볼륨 저장
        }
    }

    // 버튼 클릭 시 테스트용 효과음 재생
    public void PlayClickSound(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip); // 효과음 1회 재생
        }
        else
        {
            Debug.LogWarning("⚠️ sfxSource 또는 clip이 비어 있음");
        }
    }

    // 기본값 복원 버튼 클릭 시
    public void ResetToDefault()
    {
        BgmSlider.value = defaultBgmVolume;
        SfxSlider.value = defaultSfxVolume;

        SetBgmVolume(defaultBgmVolume);
        SetSfxVolume(defaultSfxVolume);

        PlayerPrefs.DeleteKey(PREF_BGM);
        PlayerPrefs.DeleteKey(PREF_SFX);

        PlayClickSound(buttonClickClip); // 복원 버튼에도 클릭 사운드 재생

        Debug.Log("🔄 오디오 설정 → 기본값으로 복원됨");
    }

    // ✅ 모든 버튼에 자동으로 클릭 사운드 등록
    private void AddSfxToAllButtons()
    {
        Button[] buttons = FindObjectsOfType<Button>();

        foreach (Button btn in buttons)
        {
            btn.onClick.AddListener(() =>
            {
                PlayClickSound(buttonClickClip);
            });
        }

        Debug.Log($"✅ {buttons.Length}개의 버튼에 클릭 사운드 자동 등록 완료");
    }
}
