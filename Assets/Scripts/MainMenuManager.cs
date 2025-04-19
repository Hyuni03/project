using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // �� �̵��� ���� �ʿ�

public class MainMenuManager : MonoBehaviour
{
    // "���� ����" ��ư Ŭ�� �� ����
    public void StartGame()
    {
        SceneManager.LoadScene("GameScene"); // "GameScene"���� �̵�
    }

    // "�̾ �ϱ�" ��ư Ŭ�� �� ����
    public void ContinueGame()
    {
        SceneManager.LoadScene("ContinueGameScene"); // "ContinueGameScene"���� �̵�
    }

    // "������" ��ư Ŭ�� �� ����
    public void OpenProfile()
    {
        SceneManager.LoadScene("ProfileScene"); // "ProfileScene"���� �̵�
    }

    // "ȯ�漳��" ��ư Ŭ�� �� ����
    public void OpenSettings()
    {
        SceneManager.LoadScene("SettingsScene"); // "SettingsScene"���� �̵�
    }

    // ���� ���� Ȯ�� �˾�
    public GameObject quitPopup;

    // ���� ���� ��ư Ŭ�� �� �˾� ����
    public void QuitGame()
    {
        Debug.Log("���� ���� ��ư Ŭ����!"); // Ȯ�ο� �α�
        quitPopup.SetActive(true);  // �˾��� Ȱ��ȭ (���̰� �ϱ�)
    }

    // "��" ��ư Ŭ�� �� ���� ����
    public void YesQuitGame()
    {
        Application.Quit();  // ���� ����
        Debug.Log("���� ����");  // ����Ƽ �����Ϳ��� ������� �����Ƿ� �α� ���
    }

    // "�ƴϿ�" ��ư Ŭ�� �� �˾� �ݱ�
    public void NoQuitGame()
    {
        quitPopup.SetActive(false);  // �˾� ��Ȱ��ȭ (�����)
    }

    // "������" ��ư�� Ŭ���ϸ� ���� �޴��� �̵�
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainScene"); // ���� ȭ������ �̵�
    }
}
