using UnityEngine;
using UnityEngine.UI;

public class AudioSectionController : MonoBehaviour
{
    [Header("UI ����")]
    public GameObject Content2;
    public Slider BgmSlider;
    public Slider SfxSlider;

    [Header("�⺻��")]
    public float defaultBgmVolume = 0.5f;
    public float defaultSfxVolume = 0.5f;

    private bool isOpen = false;

    void Start()
    {
        // �ʱ� ������ ����
        BgmSlider.value = defaultBgmVolume;
        SfxSlider.value = defaultSfxVolume;
    }

    public void ToggleContent()
    {
        isOpen = !isOpen;
        Content2.SetActive(isOpen); //"����� ����" ��ư Ŭ�� �� ������ ����/�ݱ�
    }

    public void ResetToDefault() // �⺻�� ���� ��ư ������ ��
    {
        BgmSlider.value = defaultBgmVolume;
        SfxSlider.value = defaultSfxVolume;
        Debug.Log("����� ���� �� �⺻������ ������");
    }
}