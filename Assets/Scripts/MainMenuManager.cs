using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    public GameObject quitPopup;        // ���� ���� �г�
    public GameObject savePopup;        // ���� ���� �г�
    public GameObject messagePopup;     // �޽��� ǥ�� �г�
    public TMP_Text messageText;        // �޽��� �ؽ�Ʈ

    // ���� ����
    public void StartGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void ContinueGame()
    {
        SceneManager.LoadScene("ContinueGameScene");
    }

    public void OpenProfile()
    {
        SceneManager.LoadScene("ProfileScene");
    }

    public void OpenSettings()
    {
        SceneManager.LoadScene("SettingsScene");
    }

    // ���� ��ư ������ ��
    public void QuitGame()
    {
        Debug.Log("���� ���� ��ư Ŭ����!");
        quitPopup.SetActive(true);
    }

    // ���� ���� �г� - ��
    public void YesQuitGame()
    {
        quitPopup.SetActive(false);
        savePopup.SetActive(true);
    }

    // ���� ���� �г� - �ƴϿ�
    public void NoQuitGame()
    {
        quitPopup.SetActive(false);
    }

    // ���� ���� �г� - ��
    public void YesSaveGame()
    {
        savePopup.SetActive(false);
        StartCoroutine(SaveAndReturnToMain(true));
    }

    // ���� ���� �г� - �ƴϿ�
    public void NoSaveGame()
    {
        savePopup.SetActive(false);
        StartCoroutine(SaveAndReturnToMain(false));
    }

    // ���� ó��: ���� ���ο� ���� �޽��� ��� �� 3�� �� ���θ޴� �̵�
    private IEnumerator SaveAndReturnToMain(bool willSave)
    {
        if (willSave)
        {
            messageText.text = "�����ϴ� ���Դϴ�.\n3�� ��, ���� ȭ������ ���ư��ϴ�.";
            // TODO: ���⿡ ���� ���� ���� �߰��ص� ��
        }
        else
        {
            messageText.text = "3�� ��,\n ���� ȭ������ ���ư��ϴ�.";
        }

        messagePopup.SetActive(true);
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene("MainScene");
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainScene");
    }
}
