using Core.StageGeneration.Stage;
using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;

public class DissableMissingKeyDialogue : MonoBehaviour
{
    [SerializeField]
    DialogueTrigger closedBossRoomDialogue;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && ObjectiveSystem.instance.IsObjectiveCompleted(ObjectiveTask.COLLECT_KEY))
            closedBossRoomDialogue.hasTriggered = true;
    }
}
