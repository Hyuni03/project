// �ػ� ���� UI ���� ��ũ��Ʈ
// �ػ� ����� ��ġ�� ���� �� ������, �⺻�� ���� ��ɵ� ���Ե�

using UnityEngine;
using UnityEngine.UI;

public class ResolutionSectionController : MonoBehaviour
{
    [Header("UI ������Ʈ ����")]
    public GameObject Content1; // �ػ� ����� ��� �ִ� ������ ����

    public Toggle Toggle_1920x1080;
    public Toggle Toggle_1280x720;
    public Toggle Toggle_1600x900;
    public Toggle Toggle_1024x768;

    [Header("�⺻ �ػ� ����")]
    public int defaultResolutionIndex = 0; // �⺻ �ػ󵵷� ����� �ε��� (0 = 1920x1080)

    private Toggle[] resolutionToggles; // ��� �迭�� ����
    private bool isOpen = false; // ������ ���� ����

    void Start()
    {
        // �迭�� �����Ͽ� �ڵ� �����ϰ�
        resolutionToggles = new Toggle[] {
            Toggle_1920x1080,
            Toggle_1280x720,
            Toggle_1600x900,
            Toggle_1024x768
        };

        // ���� �� �⺻�� ����
        ResetToDefault();
    }

    // "�ػ� �� ȭ�� ũ��" ��ư Ŭ�� �� ������ ����/�ݱ�
    public void ToggleContent()
    {
        isOpen = !isOpen;
        Content1.SetActive(isOpen);
    }

    // �⺻�� ���� ��ư ������ �� ȣ��Ǵ� �Լ�
    public void ResetToDefault()
    {
        for (int i = 0; i < resolutionToggles.Length; i++)
        {
            resolutionToggles[i].isOn = (i == defaultResolutionIndex);

            if (i == defaultResolutionIndex)
            {
                // ���õ� ��ۿ� ��Ŀ�� ���� (�ð��� ���� ȿ�� ����)
                UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(resolutionToggles[i].gameObject);
            }
        }

        Debug.Log("�ػ� �⺻������ ���� �Ϸ�");
    }
}
