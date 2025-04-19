using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // 씬 이동을 위해 필요

public class MainMenuManager : MonoBehaviour
{
    // "게임 시작" 버튼 클릭 시 실행
    public void StartGame()
    {
        SceneManager.LoadScene("GameScene"); // "GameScene"으로 이동
    }

    // "이어서 하기" 버튼 클릭 시 실행
    public void ContinueGame()
    {
        SceneManager.LoadScene("ContinueGameScene"); // "ContinueGameScene"으로 이동
    }

    // "프로필" 버튼 클릭 시 실행
    public void OpenProfile()
    {
        SceneManager.LoadScene("ProfileScene"); // "ProfileScene"으로 이동
    }

    // "환경설정" 버튼 클릭 시 실행
    public void OpenSettings()
    {
        SceneManager.LoadScene("SettingsScene"); // "SettingsScene"으로 이동
    }

    // 게임 종료 확인 팝업
    public GameObject quitPopup;

    // 게임 종료 버튼 클릭 시 팝업 띄우기
    public void QuitGame()
    {
        Debug.Log("게임 종료 버튼 클릭됨!"); // 확인용 로그
        quitPopup.SetActive(true);  // 팝업을 활성화 (보이게 하기)
    }

    // "예" 버튼 클릭 시 게임 종료
    public void YesQuitGame()
    {
        Application.Quit();  // 게임 종료
        Debug.Log("게임 종료");  // 유니티 에디터에서 종료되지 않으므로 로그 출력
    }

    // "아니오" 버튼 클릭 시 팝업 닫기
    public void NoQuitGame()
    {
        quitPopup.SetActive(false);  // 팝업 비활성화 (숨기기)
    }

    // "나가기" 버튼을 클릭하면 메인 메뉴로 이동
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainScene"); // 메인 화면으로 이동
    }
}
