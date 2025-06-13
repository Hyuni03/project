using UnityEngine;
using UnityEngine.UI;

public class AudioSectionController : MonoBehaviour
{
    [Header("UI ����")]
    public GameObject Content2;           // ����� ���� ������ ����
    public Slider BgmSlider;              // BGM ���� ���� �����̴�
    public Slider SfxSlider;              // ȿ���� ���� ���� �����̴�

    [Header("�⺻��")]
    public float defaultBgmVolume = 0.5f; // �⺻ BGM ����
    public float defaultSfxVolume = 0.5f; // �⺻ ȿ���� ����

    [Header("����� �ҽ�")]
    public AudioSource bgmSource;         // ������ ����� BGM �ҽ� (BGMPlayer�� ���� AudioSource)

    private bool isOpen = false;          // ������ ���� ����

    void Start()
    {
        // �ʱ� ���� ����
        BgmSlider.value = defaultBgmVolume;
        SfxSlider.value = defaultSfxVolume;

        // �����̴� �̺�Ʈ ���
        BgmSlider.onValueChanged.AddListener(SetBgmVolume);
    }

    // "����� ����" ��ư Ŭ�� �� ������ ����/�ݱ�
    public void ToggleContent()
    {
        isOpen = !isOpen;
        Content2.SetActive(isOpen);
    }

    // BGM ���� �����̴��� ����� �� ȣ��Ǵ� �Լ�
    public void SetBgmVolume(float volume)
    {
        if (bgmSource != null)
        {
            bgmSource.volume = volume;
        }
    }

    // "�⺻�� ����" ��ư Ŭ�� ��
    public void ResetToDefault()
    {
        BgmSlider.value = defaultBgmVolume;
        SfxSlider.value = defaultSfxVolume;

        // BGM ������ ��� ����
        SetBgmVolume(defaultBgmVolume);

        Debug.Log("����� ���� �� �⺻������ ������");
    }
}
