namespace DefaultNamespace.DialogueSystem
{
    using UnityEngine;
    using System.Collections.Generic;

    [System.Serializable]
    public class DialoguePhrase
    {
        public string characterName;
        [TextArea(3, 5)] public string text;
        public Sprite characterIcon;
        public bool isPlayerChoice;
        public DialogueChoice[] choices;
    }
    
    [System.Serializable]
    public class DialogueChoice
    {
        public string choiceText;
        public int nextBranchIndex;
    }
    
    [System.Serializable]
    public class DialogueBranch
    {
        public string branchName;
        public List<DialoguePhrase> phrases;
    }
}