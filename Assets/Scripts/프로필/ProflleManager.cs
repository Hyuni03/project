using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ProfileManager : MonoBehaviour
{
    public static ProfileManager Instance;

    [Header("������ ScriptableObject ����Ʈ")]
    public List<HeroineData> heroineList;

    [Header("������ ī��")]
    public List<HeroineProfileCard> heroineCards;

    [Header("���� ȭ��")]
    public GameObject profileScreen;

    [Header("�����κ� �� ȭ��")]
    public GameObject DetailHeroine1;
    public GameObject DetailHeroine2;
    public GameObject DetailHeroine3;
    public GameObject DetailHeroine4;

    [Header("�ȳ� �˾� UI")]
    public GameObject noticePanel;
    public TMP_Text noticeText;
    public Button noticeCloseButton;

    [Header("���� ���� ����")]
    public int currentStoryStage = 2;

    private Dictionary<string, int> affinityDict = new Dictionary<string, int>();

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        InitAffinity();
        SetupCards();

        profileScreen.SetActive(true);
        DeactivateAllDetailScreens();
        noticePanel.SetActive(false);

        if (noticeCloseButton != null)
            noticeCloseButton.onClick.AddListener(() => noticePanel.SetActive(false));
    }

    void InitAffinity()
    {
        affinityDict["Heroine1"] = 50;
        affinityDict["Heroine2"] = 25;
        affinityDict["Heroine3"] = 10;
        affinityDict["Heroine4"] = 0;
    }

    void SetupCards()
    {
        for (int i = 0; i < heroineList.Count && i < heroineCards.Count; i++)
        {
            var data = heroineList[i];
            var card = heroineCards[i];
            bool unlocked = IsProfileUnlocked(data);
            card.Setup(data, unlocked);
        }
    }

    bool IsProfileUnlocked(HeroineData data)
    {
        int affinity = affinityDict.ContainsKey(data.heroineName)
            ? affinityDict[data.heroineName]
            : 0;

        return (affinity >= data.requiredAffinity && currentStoryStage >= data.requiredStoryStage);
    }

    public void ShowDetail(HeroineData data)
    {
        profileScreen.SetActive(false);
        DeactivateAllDetailScreens();

        switch (data.heroineName)
        {
            case "Heroine1":
                DetailHeroine1?.SetActive(true);
                break;
            case "Heroine2":
                DetailHeroine2?.SetActive(true);
                break;
            case "Heroine3":
                DetailHeroine3?.SetActive(true);
                break;
            case "Heroine4":
                DetailHeroine4?.SetActive(true);
                break;
            default:
                Debug.LogWarning("Unknown heroine name: " + data.heroineName);
                break;
        }
    }

    public void CloseAllDetails()
    {
        DeactivateAllDetailScreens();              // ��� �� ������
        profileScreen.SetActive(true);             // ���� ������ ȭ�� �ٽ� ������
    }

    void DeactivateAllDetailScreens()
    {
        if (DetailHeroine1 != null) DetailHeroine1.SetActive(false);
        if (DetailHeroine2 != null) DetailHeroine2.SetActive(false);
        if (DetailHeroine3 != null) DetailHeroine3.SetActive(false);
        if (DetailHeroine4 != null) DetailHeroine4.SetActive(false);
    }

    public void ShowNotice(string message)
    {
        if (noticeText != null) noticeText.text = message;
        if (noticePanel != null) noticePanel.SetActive(true);
    }
}
