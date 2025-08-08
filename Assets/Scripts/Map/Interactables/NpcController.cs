using DefaultNamespace.DialogueSystem;
using UnityEngine;


namespace Map.Interactables
{
    public class NpcController : Interactable
    {
        public override void Interact()
        {
            
        }

        private void Start()
        {
            DialogSystem.Instance.LoadDialog("TestDialogue");
        }
    }
}