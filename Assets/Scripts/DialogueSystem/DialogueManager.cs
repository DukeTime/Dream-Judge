namespace DefaultNamespace.DialogueSystem
{
    using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("UI References")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI characterNameText;
    public TextMeshProUGUI dialogueText;
    public Image characterIcon;
    public Transform choicesContainer;
    public GameObject choiceButtonPrefab;

    private DialogueData currentDialogue;
    private DialogueBranch currentBranch;
    private int currentPhraseIndex;
    private bool isDialogueActive;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        dialoguePanel.SetActive(false);
    }

    public void StartDialogue(DialogueData dialogue, int startingBranch = 0)
    {
        if (dialogue == null || isDialogueActive) return;
        
        currentDialogue = dialogue;
        currentBranch = dialogue.GetBranch(startingBranch);
        currentPhraseIndex = 0;
        isDialogueActive = true;
        
        dialoguePanel.SetActive(true);
        DisplayNextPhrase();
    }

    public void DisplayNextPhrase()
    {
        if (currentBranch == null || currentPhraseIndex >= currentBranch.phrases.Count)
        {
            EndDialogue();
            return;
        }

        var currentPhrase = currentBranch.phrases[currentPhraseIndex];
        
        characterNameText.text = currentPhrase.characterName;
        dialogueText.text = currentPhrase.text;
        characterIcon.sprite = currentPhrase.characterIcon;
        
        // Clear previous choices
        foreach (Transform child in choicesContainer)
        {
            Destroy(child.gameObject);
        }

        if (currentPhrase.isPlayerChoice && currentPhrase.choices != null)
        {
            // Show choices
            for (int i = 0; i < currentPhrase.choices.Length; i++)
            {
                var choice = currentPhrase.choices[i];
                var choiceButton = Instantiate(choiceButtonPrefab, choicesContainer);
                choiceButton.GetComponentInChildren<TextMeshProUGUI>().text = choice.choiceText;
                
                int branchIndex = choice.nextBranchIndex; // Capture for closure
                choiceButton.GetComponent<Button>().onClick.AddListener(() => 
                {
                    SelectChoice(branchIndex);
                });
            }
        }
        else
        {
            // Auto-advance to next phrase
            currentPhraseIndex++;
        }
    }

    private void SelectChoice(int branchIndex)
    {
        currentBranch = currentDialogue.GetBranch(branchIndex);
        currentPhraseIndex = 0;
        DisplayNextPhrase();
    }

    public void EndDialogue()
    {
        isDialogueActive = false;
        dialoguePanel.SetActive(false);
        currentDialogue = null;
    }

    private void Update()
    {
        if (isDialogueActive && Input.GetKeyDown(KeyCode.Space))
        {
            DisplayNextPhrase();
        }
    }
}
}