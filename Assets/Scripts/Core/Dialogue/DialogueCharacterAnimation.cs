using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;

public class DialogueCharacterAnimation : MonoBehaviour
{
    [SerializeField]
    private GameObject inWorldCharacter;

    [SerializeField]
    private GameObject characterPrefab;

    [SerializeField]
    private GameObject characterAltPrefab;

    [SerializeField]
    private DialogueTrigger dialogueTrigger;

    [SerializeField]
    private bool ActiveOnStart;

    [SerializeField]
    private GameObject keyIcon;

    private bool triggered;
    private bool swapped;
    private bool finished;

    private void Awake()
    {
        if(!ActiveOnStart)
        {
            inWorldCharacter.SetActive(false);
            dialogueTrigger.hasTriggered = true;
            finished = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!triggered && !finished && other.gameObject.CompareTag("Player"))
        {
            GameObject obj = Instantiate(characterAltPrefab, inWorldCharacter.transform.position, inWorldCharacter.transform.rotation, inWorldCharacter.transform.parent);
            Destroy(inWorldCharacter);
            inWorldCharacter = obj;
            triggered = true;
            if (keyIcon != null)
                keyIcon.SetActive(false);
        }
    }

    void Update()
    {
        if(triggered)
            if(!DialogueManager.instance.dialogueActive)
            {
                GameObject obj = Instantiate(characterPrefab, inWorldCharacter.transform.position, inWorldCharacter.transform.rotation, inWorldCharacter.transform.parent);
                Destroy(inWorldCharacter);
                inWorldCharacter = obj;
                finished = true;
                triggered = false;
            }

        if (!swapped && ObjectiveSystem.instance.IsObjectiveCompleted(ObjectiveTask.KILL_Enemies))
        {
            inWorldCharacter.SetActive(!inWorldCharacter.activeSelf);
            dialogueTrigger.hasTriggered = !inWorldCharacter.activeSelf;
            finished = !inWorldCharacter.activeSelf;
            swapped = true;
        }
    }
}
