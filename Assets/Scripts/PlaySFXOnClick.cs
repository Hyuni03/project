using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(AudioSource))]
public class PlaySFXOnClick : MonoBehaviour
{
    [Header("효과음 설정")]
    public AudioClip clickSound; // 클릭 소리 (ex: ButtonClick.wav)
    public float volume = 0.5f;  // 기본 음량

    private AudioSource sfxSource;

    void Awake()
    {
        // 오디오 소스 자동 설정
        sfxSource = GetComponent<AudioSource>();
        sfxSource.playOnAwake = false;
    }

    void Start()
    {
        // 모든 버튼을 찾아서 클릭 이벤트 등록
        Button[] allButtons = FindObjectsOfType<Button>(true);

        foreach (Button btn in allButtons)
        {
            // 중복 등록 방지
            btn.onClick.RemoveListener(PlayClickSound);
            btn.onClick.AddListener(PlayClickSound);
        }

        Debug.Log($"✅ {allButtons.Length}개의 버튼에 클릭 사운드 적용 완료");
    }

    public void PlayClickSound()
    {
        if (clickSound != null)
        {
            sfxSource.PlayOneShot(clickSound, volume);
        }
    }

    // SFX 볼륨 변경 (AudioSectionController에서 호출)
    public void SetSfxVolume(float newVolume)
    {
        volume = newVolume;
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    // 저장된 볼륨 불러오기
    void OnEnable()
    {
        if (PlayerPrefs.HasKey("SFXVolume"))
        {
            volume = PlayerPrefs.GetFloat("SFXVolume");
        }
    }
}
