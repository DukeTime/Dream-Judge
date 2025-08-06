namespace DefaultNamespace.Dialogue_system
{
    using UnityEngine;
    using System.Collections.Generic;

    public class DialogueSystem : MonoBehaviour
    {
        public DialogueDatabase database;
        private DialoguePhrase currentPhrase;
        private DialogueEntryPoint currentEntryPoint;

        public void StartDialogue(string entryPointId)
        {
            var entryPoint = database.entryPoints.Find(ep => ep.id == entryPointId);
            if (entryPoint == null)
            {
                Debug.LogError($"Entry point {entryPointId} not found!");
                return;
            }

            if (!CheckCondition(entryPoint.condition))
            {
                Debug.Log($"Entry point {entryPointId} condition not met: {entryPoint.condition}");
                return;
            }

            currentEntryPoint = entryPoint;
            currentPhrase = database.GetPhraseById(entryPoint.startPhraseId);
            ShowCurrentPhrase();
        }

        private void ShowCurrentPhrase()
        {
            if (currentPhrase == null)
            {
                EndDialogue();
                return;
            }

            // Здесь реализуйте отображение фразы в UI
            Debug.Log($"{currentPhrase.characterName}: {currentPhrase.text}");

            // Если нет вариантов выбора, автоматически переходим к следующей фразе
            if (currentPhrase.choices.Count == 0)
            {
                NextPhrase("");
            }
        }

        public void NextPhrase(string choiceText = "")
        {
            if (currentPhrase == null) return;

            // Если есть варианты выбора, находим выбранный
            if (currentPhrase.choices.Count > 0)
            {
                var selectedChoice = currentPhrase.choices.Find(c => c.text == choiceText);
                if (selectedChoice != null && CheckCondition(selectedChoice.condition))
                {
                    currentPhrase = database.GetPhraseById(selectedChoice.nextPhraseId);
                }
                else
                {
                    Debug.LogWarning("Invalid choice or condition not met");
                    return;
                }
            }
            else
            {
                // Если нет вариантов, просто берем первый возможный переход
                if (currentPhrase.choices.Count > 0)
                {
                    currentPhrase = database.GetPhraseById(currentPhrase.choices[0].nextPhraseId);
                }
                else
                {
                    currentPhrase = null;
                }
            }

            ShowCurrentPhrase();
        }

        private bool CheckCondition(string condition)
        {
            if (string.IsNullOrEmpty(condition) || condition == "always")
            {
                return true;
            }

            // Здесь реализуйте проверку условий
            // Например: "questStage:2" или "hasItem:key"
            // В реальном проекте это будет сложнее
            
            Debug.LogWarning("Condition checking not fully implemented!");
            return true;
        }

        private void EndDialogue()
        {
            currentPhrase = null;
            currentEntryPoint = null;
            Debug.Log("Dialogue ended");
        }
    }
}