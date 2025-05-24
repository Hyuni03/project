using UnityEngine;

[CreateAssetMenu(fileName = "HeroineData", menuName = "Profiles/New Heroine")]
public class HeroineData : ScriptableObject
{
    public string heroineName;         // 히로인 이름
    public Sprite unlockedImage;       // 해금된 이미지
    public Sprite lockedImage;         // 잠금 상태 이미지
    [TextArea]
    public string description;         // 설명
    public int requiredAffinity;       // 필요 호감도
    public int requiredStoryStage;     // 필요 스토리 진행도
}
