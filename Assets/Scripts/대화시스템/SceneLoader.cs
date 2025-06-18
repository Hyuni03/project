using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [Header("전환할 씬 이름")]
    public string nextSceneName = "Episode1Scene"; // 전환할 대상 씬 이름

    // 씬을 로드하는 함수 (외부에서 호출)
    public void LoadNextScene()
    {
        Debug.Log($"▶ 씬 전환 중: {nextSceneName}");
        SceneManager.LoadScene(nextSceneName);
    }
}
