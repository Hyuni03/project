using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue Data", menuName = "Dialogue/DialogueData")]
public class DialogueData : ScriptableObject
{
    public List<DialogueLine> lines = new List<DialogueLine>();
}

