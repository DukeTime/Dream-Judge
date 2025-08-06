namespace DefaultNamespace.DialogueSystem
{
    using System.Collections.Generic;
    using UnityEngine;
    
    [CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue System/Dialogue Data")]
    public class DialogueData : ScriptableObject
    {
        public string entryPointID;
        public List<DialogueBranch> branches = new List<DialogueBranch>();
        
        public DialogueBranch GetBranch(int index)
        {
            if (index >= 0 && index < branches.Count)
                return branches[index];
            return null;
        }
    }
}