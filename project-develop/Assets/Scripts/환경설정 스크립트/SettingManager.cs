// ���� â �ݱ� �� ����ȭ������ �̵��ϴ� ���

using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsManager : MonoBehaviour
{
    [Header("UI ������Ʈ ����")]
    public GameObject settingsPanel; // ��ü ȯ�漳�� UI �г�

    // ����â �ݱ� ��ư���� ȣ��
    public void CloseSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    // ����ȭ������ ���ư��� ��ư���� ȣ��
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainScene"); // ���� �� �̸��� �°� ����
    }
}
