using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class EndingManager : MonoBehaviour
{
    [Header("�˾� ������Ʈ")]
    public GameObject confirmationPopup;
    public GameObject proceedingPopup;

    [Header("�˾� UI ���")]
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

            questionText.text = $"{selectedHeroineName}��(��) �����Ͻðڽ��ϱ�?";
            confirmationPopup.SetActive(true);
        }
        else
        {
            Debug.LogError("OnHeroineClicked�� ���޵� ������ ������ �߸��Ǿ����ϴ�. '�̸�,���̸�' �����̾�� �մϴ�.");
        }
    }

    private void OnYesClicked()
    {
        confirmationPopup.SetActive(false);
        proceedingText.text = $"{selectedHeroineName}���� ������ ����˴ϴ�.";
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

        Debug.Log($"{selectedHeroineName} ����, {selectedHeroineSceneName} ������ �̵��մϴ�.");
        SceneManager.LoadScene(selectedHeroineSceneName);
    }
}