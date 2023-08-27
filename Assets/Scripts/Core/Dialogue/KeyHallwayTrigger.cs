using Core.StageGeneration.Stage;
using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;

public class KeyHallwayTrigger : MonoBehaviour
{
    [SerializeField]
    DialogueTrigger keyBossRoomDialogue;

    [SerializeField]
    GameObject lockIcon;

    private bool triggered;

    private void OnTriggerEnter(Collider other)
    {
        if (!triggered && other.gameObject.CompareTag("Player") && ObjectiveSystem.instance.IsObjectiveCompleted(ObjectiveTask.COLLECT_KEY))
        {
            StageHelper.Instance.spawnRoom.bossHallwayDoor.GetComponent<RoomEditor.DoorBlock>().OpenDoor();
            keyBossRoomDialogue.TriggerDialogue();
            lockIcon.SetActive(false);
            triggered = true;
        }  
    }
}
