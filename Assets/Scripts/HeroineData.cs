using UnityEngine;

[CreateAssetMenu(fileName = "HeroineData", menuName = "Profiles/New Heroine")]
public class HeroineData : ScriptableObject
{
    public string heroineName;         // ������ �̸�
    public Sprite unlockedImage;       // �رݵ� �̹���
    public Sprite lockedImage;         // ��� ���� �̹���
    [TextArea]
    public string description;         // ����
    public int requiredAffinity;       // �ʿ� ȣ����
    public int requiredStoryStage;     // �ʿ� ���丮 ���൵
}
