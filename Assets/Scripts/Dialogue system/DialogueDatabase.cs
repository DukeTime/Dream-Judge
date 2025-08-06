using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "New Dialogue Phrase", menuName = "Dialogue/Dialogue Phrase")]
public class DialoguePhrase : ScriptableObject
{
    public string id;
    public string characterName;
    public Sprite characterIcon;
    [TextArea(3, 10)] public string text;
    public List<DialogueChoice> choices = new List<DialogueChoice>();
    public Vector2 graphPosition;
}

[Serializable]
public class DialogueChoice
{
    public string text;
    public string nextPhraseId;
    public string condition; // Условие для показа этого выбора (например, "hasItem:key")
}

[Serializable]
public class DialogueEntryPoint
{
    public string id;
    public string startPhraseId;
    public string condition; // Условие для запуска диалога (например, "questStage:2")
}

[CreateAssetMenu(fileName = "New Dialogue Database", menuName = "Dialogue/Dialogue Database")]
public class DialogueDatabase : ScriptableObject
{
    public List<DialogueEntryPoint> entryPoints = new List<DialogueEntryPoint>();
    public List<DialoguePhrase> phrases = new List<DialoguePhrase>();

    public DialoguePhrase GetPhraseById(string id)
    {
        return phrases.Find(p => p.id == id);
    }
}