using UnityEngine;
using UnityEngine.UI;

public class CardHoverEffect : MonoBehaviour
{
    [Header("ȿ���� ������ UI ���")]
    
    public Image characterImage;

    [Header("���콺 ���� �� ����")]
    public Color hoverColor = new Color(1f, 0.8f, 0.85f); 

    
    private Color originalCharacterColor;

    void Start()
    {
        
        if (characterImage != null) originalCharacterColor = characterImage.color;
    }

    
    public void OnHoverEnter()
    {
        
        if (characterImage != null) characterImage.color = originalCharacterColor * hoverColor;
    }

    
    public void OnHoverExit()
    {
        
        if (characterImage != null) characterImage.color = originalCharacterColor;
    }
}