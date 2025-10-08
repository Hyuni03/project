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

    private bool isOpen = false;           // 콘텐츠 열림 상태

    private const string PREF_BGM = "BgmVolume";
    private const string PREF_SFX = "SfxVolume";

    void Start()
    {
        Debug.Log("🟢 AudioSectionController.Start() 호출됨");

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
    }

    // 🎚 "오디오 설정" 버튼 클릭 시 콘텐츠 열기/닫기
    public void ToggleContent()
    {
        isOpen = !isOpen;
        Content2.SetActive(isOpen);
    }

    // 🎵 BGM 볼륨 슬라이더 변경 시 호출
    public void SetBgmVolume(float volume)
    {
        Debug.Log($"🔊 SetBgmVolume 호출됨: {volume}");

        if (bgmSource != null)
        {
            bgmSource.volume = volume;
            PlayerPrefs.SetFloat(PREF_BGM, volume); // 🔹 볼륨 저장
        }
        else
        {
            Debug.LogWarning("❗ bgmSource가 null입니다");
        }
    }

    // 🎧 SFX 볼륨 슬라이더 변경 시 호출
    public void SetSfxVolume(float volume)
    {
        Debug.Log($"🎧 SetSfxVolume 호출됨: {volume}");

        if (sfxSource != null)
        {
            sfxSource.volume = volume;
            PlayerPrefs.SetFloat(PREF_SFX, volume); // 🔹 효과음 볼륨 저장
        }
        else
        {
            Debug.LogWarning("❗ sfxSource가 null입니다");
        }
    }

    // 🧩 버튼 클릭 시 테스트용 효과음 재생
    public void PlayClickSound(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip); // 🔹 효과음 1회 재생
        }
    }

    // 🔁 "기본값 복원" 버튼 클릭 시
    public void ResetToDefault()
    {
        BgmSlider.value = defaultBgmVolume;
        SfxSlider.value = defaultSfxVolume;

        SetBgmVolume(defaultBgmVolume);
        SetSfxVolume(defaultSfxVolume);

        PlayerPrefs.DeleteKey(PREF_BGM);
        PlayerPrefs.DeleteKey(PREF_SFX);

        Debug.Log("오디오 설정 → 기본값으로 복원됨");
    }
}
