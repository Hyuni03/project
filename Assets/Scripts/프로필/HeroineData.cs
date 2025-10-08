using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/HeroineData", fileName = "NewHeroineData")]
public class HeroineData : ScriptableObject
{
    [Header("������ �ĺ��� �̸� (��: Heroine1)")]
    public string heroineName;

    [Header("ȭ�鿡 ǥ�õ� �̸� (��: ����)")]
    public string displayName;

    [TextArea(2, 5)]
    [Header("������ ����")]
    public string description;

    [Header("�ر� �� ǥ�õ� �̹���")]
    public Sprite unlockedImage;

    [Header("��� ���� �̹��� (�Ƿ翧 ��)")]
    public Sprite lockedImage;

    [Header("�ر� ����: ȣ����")]
    public int requiredAffinity;

    [Header("�ر� ����: ���丮 ���൵")]
    public int requiredStoryStage;
}
