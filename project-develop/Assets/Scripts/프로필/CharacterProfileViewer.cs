using UnityEngine;
using TMPro;
using System.Collections.Generic;


[System.Serializable]
public class CharacterProfileData
{
    public string characterName;

    [TextArea(10, 15)]
    public string profileText;

    
    public GameObject profilePanelObject;
}

public class CharacterProfileViewer : MonoBehaviour
{
    
    public List<CharacterProfileData> characterProfiles = new List<CharacterProfileData>();

    
    private int currentIndex = 0;

    void Start()
    {
        
        ShowProfile(currentIndex);
    }

    
    public void ShowProfile(int index)
    {
        if (index < 0 || index >= characterProfiles.Count) return;

        currentIndex = index;

        
        for (int i = 0; i < characterProfiles.Count; i++)
        {
            if (characterProfiles[i].profilePanelObject != null)
            {
                characterProfiles[i].profilePanelObject.SetActive(false);
            }
        }

        
        GameObject activePanel = characterProfiles[currentIndex].profilePanelObject;
        if (activePanel != null)
        {
            activePanel.SetActive(true);

            
            TextMeshProUGUI textDisplay = activePanel.GetComponentInChildren<TextMeshProUGUI>();
            if (textDisplay != null)
            {
                string formattedText = FormatText(characterProfiles[currentIndex].profileText);
                textDisplay.text = formattedText;
            }
        }
    }

    
    public void ShowNextProfile()
    {
        int nextIndex = (currentIndex + 1) % characterProfiles.Count;
        ShowProfile(nextIndex);
    }

    public void ShowPreviousProfile()
    {
        int prevIndex = (currentIndex - 1 + characterProfiles.Count) % characterProfiles.Count;
        ShowProfile(prevIndex);
    }

    private string FormatText(string originalText)
    {
        if (string.IsNullOrEmpty(originalText)) return "";
        string[] lines = originalText.Split('\n');
        System.Text.StringBuilder resultBuilder = new System.Text.StringBuilder();
        foreach (string line in lines)
        {
            int dotIndex = line.IndexOf('.');
            if (dotIndex > 0)
            {
                string boldPart = $"<b>{line.Substring(0, dotIndex + 1)}</b>";
                string restPart = line.Substring(dotIndex + 1);
                resultBuilder.AppendLine(boldPart + restPart);
            }
            else
            {
                resultBuilder.AppendLine(line);
            }
        }
        return resultBuilder.ToString();
    }
}