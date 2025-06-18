using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [Header("��ȯ�� �� �̸�")]
    public string nextSceneName = "Episode1Scene"; // ��ȯ�� ��� �� �̸�

    // ���� �ε��ϴ� �Լ� (�ܺο��� ȣ��)
    public void LoadNextScene()
    {
        Debug.Log($"�� �� ��ȯ ��: {nextSceneName}");
        SceneManager.LoadScene(nextSceneName);
    }
}
