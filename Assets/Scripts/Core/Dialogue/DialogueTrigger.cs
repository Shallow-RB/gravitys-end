using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField]
    private bool enableTriggerEnter;
    [SerializeField]
    private bool enableTriggerExit;

    [SerializeField]
    private Dialogue dialogue;

    [HideInInspector]
    public bool hasTriggered;

    public void TriggerDialogue()
    {
        DialogueManager.instance.StartDialogue(dialogue);  
    }
    private void OnTriggerEnter(Collider other)
    {
        if (enableTriggerEnter && !hasTriggered && other.gameObject.CompareTag("Player"))
        {
            DialogueManager.instance.StartDialogue(dialogue);
            hasTriggered = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (enableTriggerExit && !hasTriggered && other.gameObject.CompareTag("Player"))
        {
            DialogueManager.instance.StartDialogue(dialogue);
            hasTriggered = true;
        }
    }
}
