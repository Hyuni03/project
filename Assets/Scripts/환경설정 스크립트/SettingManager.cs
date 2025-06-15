// 설정 창 닫기 및 메인화면으로 이동하는 기능

using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsManager : MonoBehaviour
{
    [Header("UI 오브젝트 연결")]
    public GameObject settingsPanel; // 전체 환경설정 UI 패널

    // 설정창 닫기 버튼에서 호출
    public void CloseSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    // 메인화면으로 돌아가는 버튼에서 호출
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainScene"); // 메인 씬 이름에 맞게 수정
    }
}
