using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UI;
using Core;

public class DialogueManager : MonoBehaviour
{
    // Singleton for DialogueManager
    public static DialogueManager instance;

    //Make sure the the name of the character portrait is the same as the character name
    public TextMeshProUGUI characterNameText;
    public List<GameObject> characterPortraits;
    public TextMeshProUGUI dialogueText;

    public float textSpeed;

    public Animator animator;

    private Queue<string> sentences;

    private bool _dialogueActive;

    public bool dialogueActive
    {
        get => _dialogueActive;
        set
        {
            _dialogueActive = value;
            Timer.instance.timerIsRunning = !value;
            OnDialgueActive?.Invoke(dialogueActive);
        }
    }

    public delegate void DialogueActive(bool mapOpened);
    public static event DialogueActive OnDialgueActive;

    private bool textIsTyping;
    private bool paused;
    private string currentSentence;
    private InputManager _inputManager;

    private void Start()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        _inputManager = new InputManager();

        sentences = new Queue<string>();

        if (gameObject.GetComponentInChildren<DialogueTrigger>() != null)
            gameObject.GetComponentInChildren<DialogueTrigger>().TriggerDialogue();
    }

    private void Update()
    {
        if (dialogueActive)
        {
            _inputManager.UI.Enable();
            if (_inputManager.UI.DisplayNextSentence.triggered)
                OnDisplayNextSentence();
        }
    }

    public void StartDialogue(Dialogue dialogue)
    {
        dialogueActive = true;
        animator.SetBool("IsOpen", true);

        characterNameText.text = dialogue.name;

        foreach (GameObject character in characterPortraits)
        {
            if (character.name == dialogue.name)
            {
                character.SetActive(true);
            }
            else character.SetActive(false);
        }

        sentences.Clear();

        foreach (string senctence in dialogue.sentences)
        {
            sentences.Enqueue(senctence);
        }

        OnDisplayNextSentence();
    }

    public void OnDisplayNextSentence()
    {
        if (textIsTyping)
        {
            StopAllCoroutines();
            dialogueText.text = currentSentence;
            textIsTyping = false;
            return;
        }

        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        currentSentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSenctence());
    }

    public IEnumerator TypeSenctence()
    {
        textIsTyping = true;
        dialogueText.text = "";

        foreach (char letter in currentSentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(textSpeed);
        }

        textIsTyping = false;
    }

    public void EndDialogue()
    {
        dialogueActive = false;
        animator.SetBool("IsOpen", false);
    }

    public void SkipDialogue()
    {
        sentences.Clear();
        dialogueText.text = "";
        EndDialogue();
    }

    public void ToggleDialoguePaused()
    {
        paused = !paused;
        if (!paused)
        {
            animator.SetBool("IsOpen", true);
            if (textIsTyping)
                OnDisplayNextSentence();
        }  
    }
}
