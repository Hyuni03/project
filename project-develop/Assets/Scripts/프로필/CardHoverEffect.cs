using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class EndingManager : MonoBehaviour
{
    [Header("팝업 오브젝트")]
    public GameObject confirmationPopup;
    public GameObject proceedingPopup;

    [Header("팝업 UI 요소")]
    public TextMeshProUGUI questionText;
    public TextMeshProUGUI proceedingText;
    public Button yesButton;
    public Button noButton;

    private string selectedHeroineName;
    private string selectedHeroineSceneName;

    void Start()
    {
        confirmationPopup.SetActive(false);
        proceedingPopup.SetActive(false);
        yesButton.onClick.AddListener(OnYesClicked);
        noButton.onClick.AddListener(OnNoClicked);
    }

    public void OnHeroineClicked(string heroineData)
    {
        string[] data = heroineData.Split(',');
        if (data.Length == 2)
        {
            selectedHeroineName = data[0];
            selectedHeroineSceneName = data[1];

            questionText.text = $"{selectedHeroineName}을(를) 선택하시겠습니까?";
            confirmationPopup.SetActive(true);
        }
        else
        {
            Debug.LogError("OnHeroineClicked에 전달된 데이터 형식이 잘못되었습니다. '이름,씬이름' 형식이어야 합니다.");
        }
    }

    private void OnYesClicked()
    {
        confirmationPopup.SetActive(false);
        proceedingText.text = $"{selectedHeroineName}과의 엔딩이 진행됩니다.";
        proceedingPopup.SetActive(true);
        StartCoroutine(StartEndingSequence(2.0f));
    }

    private void OnNoClicked()
    {
        confirmationPopup.SetActive(false);
    }

    private IEnumerator StartEndingSequence(float delay)
    {
        yield return new WaitForSeconds(delay);
        proceedingPopup.SetActive(false);

        Debug.Log($"{selectedHeroineName} 선택, {selectedHeroineSceneName} 씬으로 이동합니다.");
        SceneManager.LoadScene(selectedHeroineSceneName);
    }
}