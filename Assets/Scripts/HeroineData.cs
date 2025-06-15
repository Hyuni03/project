using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/HeroineData", fileName = "NewHeroineData")]
public class HeroineData : ScriptableObject
{
    [Header("히로인 식별용 이름 (예: Heroine1)")]
    public string heroineName;

    [Header("화면에 표시될 이름 (예: 유이)")]
    public string displayName;

    [TextArea(2, 5)]
    [Header("히로인 설명")]
    public string description;

    [Header("해금 시 표시될 이미지")]
    public Sprite unlockedImage;

    [Header("잠금 상태 이미지 (실루엣 등)")]
    public Sprite lockedImage;

    [Header("해금 조건: 호감도")]
    public int requiredAffinity;

    [Header("해금 조건: 스토리 진행도")]
    public int requiredStoryStage;
}
