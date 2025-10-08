using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HeroineProfileCard : MonoBehaviour
{
    [Header("UI 참조")]
    public Image profileImage;
    public TMP_Text nameText;
    public GameObject darkOverlay;
    public Button cardButton;

    private HeroineData data;
    private bool isUnlocked;

    /// <summary>
    /// 카드에 히로인 정보를 설정하고 버튼 이벤트 연결
    /// </summary>
    public void Setup(HeroineData data, bool isUnlocked)
    {
        this.data = data;
        this.isUnlocked = isUnlocked;

        // 이미지 및 이름 표시
        if (profileImage != null)
            profileImage.sprite = isUnlocked ? data.unlockedImage : data.lockedImage;

        if (darkOverlay != null)
            darkOverlay.SetActive(!isUnlocked);

        if (nameText != null)
        {
            nameText.text = isUnlocked ? data.heroineName : "???";
            nameText.color = isUnlocked ? Color.white : Color.gray;
            nameText.gameObject.SetActive(true);
        }

        // 버튼 이벤트 연결
        if (cardButton == null)
        {
            Debug.LogWarning("⚠ 카드 버튼이 연결되지 않았습니다.");
            return;
        }

        cardButton.onClick.RemoveAllListeners();
        cardButton.onClick.AddListener(OnClick);
    }

    /// <summary>
    /// 카드 클릭 시 호출되는 함수
    /// </summary>
    private void OnClick()
    {
        Debug.Log($"[카드 클릭됨] {data.heroineName}, 해금 상태: {isUnlocked}");

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