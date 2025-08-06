namespace DefaultNamespace.DialogueSystem
{
    using UnityEngine;

    public class DialogueTrigger : MonoBehaviour
    {
        public DialogueData dialogue;
        public string requiredFlag;
    
        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                // Проверка условий (можно заменить на свою систему флагов)
                if (string.IsNullOrEmpty(requiredFlag) || PlayerFlags.HasFlag(requiredFlag))
                {
                    DialogueManager.Instance.StartDialogue(dialogue);
                }
            }
        }
    }
}