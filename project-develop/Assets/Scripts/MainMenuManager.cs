using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    public GameObject quitPopup;        // 종료 여부 패널
    public GameObject savePopup;        // 저장 여부 패널
    public GameObject messagePopup;     // 메시지 표시 패널
    public TMP_Text messageText;        // 메시지 텍스트

    // 게임 시작
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

    // 종료 버튼 눌렀을 때
    public void QuitGame()
    {
        Debug.Log("게임 종료 버튼 클릭됨!");
        quitPopup.SetActive(true);
    }

    // 종료 여부 패널 - 예
    public void YesQuitGame()
    {
        quitPopup.SetActive(false);
        savePopup.SetActive(true);
    }

    // 종료 여부 패널 - 아니오
    public void NoQuitGame()
    {
        quitPopup.SetActive(false);
    }

    // 저장 여부 패널 - 예
    public void YesSaveGame()
    {
        savePopup.SetActive(false);
        StartCoroutine(SaveAndReturnToMain(true));
    }

    // 저장 여부 패널 - 아니오
    public void NoSaveGame()
    {
        savePopup.SetActive(false);
        StartCoroutine(SaveAndReturnToMain(false));
    }

    // 공통 처리: 저장 여부에 따라 메시지 출력 후 3초 뒤 메인메뉴 이동
    private IEnumerator SaveAndReturnToMain(bool willSave)
    {
        if (willSave)
        {
            messageText.text = "저장하는 중입니다.\n3초 후, 메인 화면으로 돌아갑니다.";
            // TODO: 여기에 실제 저장 로직 추가해도 됨
        }
        else
        {
            messageText.text = "3초 후,\n 메인 화면으로 돌아갑니다.";
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
