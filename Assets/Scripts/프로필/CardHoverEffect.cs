using UnityEngine;
using UnityEngine.UI;

public class CardHoverEffect : MonoBehaviour
{
    [Header("효과를 적용할 UI 요소")]
    
    public Image characterImage;

    [Header("마우스 오버 시 색상")]
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