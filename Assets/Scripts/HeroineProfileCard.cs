using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HeroineProfileCard : MonoBehaviour
{
    public Image profileImage;
    public TMP_Text nameText;
    public GameObject darkOverlay;
    public Button cardButton;

    private HeroineData data;
    private bool isUnlocked;

    public void Setup(HeroineData data, bool isUnlocked)
    {
        this.data = data;
        this.isUnlocked = isUnlocked;

        profileImage.sprite = isUnlocked ? data.unlockedImage : data.lockedImage;
        darkOverlay.SetActive(!isUnlocked);
        nameText.text = isUnlocked ? data.heroineName : "???";
        nameText.color = isUnlocked ? Color.white : Color.gray;
        nameText.gameObject.SetActive(true);

        cardButton.onClick.RemoveAllListeners();
        cardButton.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        if (isUnlocked)
        {
            ProfileManager.Instance.ShowDetail(data);
        }
        else
        {
            ProfileManager.Instance.ShowNotice("조건을 미달성했습니다.");
        }
    }
}
