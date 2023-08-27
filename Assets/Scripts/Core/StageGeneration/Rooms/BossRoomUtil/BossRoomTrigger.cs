using System.Linq;
using Cinemachine;
using Core.Audio;
using Core.StageGeneration.Rooms.RoomTypes;
using Core.StageGeneration.Stage;
using UI;
using UnityEngine;

namespace Core.StageGeneration.Rooms.BossRoomUtil
{
    public class BossRoomTrigger : MonoBehaviour
    {
        [SerializeField]
        private Canvas bossFightCanvas;
        [SerializeField]
        private GameObject bossProtection;

        private GameObject bossRoom;
        private GameObject boss;
        private BossHallway bossHallway;

        private CinemachineVirtualCamera topDownCamera;

        private bool triggered;
        private bool fightStarted;

        private void Start()
        {
            bossRoom = transform.root.gameObject;

            bossFightCanvas.enabled = false;

            bossHallway = FindObjectOfType<BossHallway>();
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player") && !triggered)
            {
                triggered = true;

                gameObject.GetComponent<DialogueTrigger>().TriggerDialogue();

                topDownCamera = GameObject.Find("Cinemachine Camera").GetComponent<CinemachineVirtualCamera>();
                topDownCamera.m_Lens.OrthographicSize = 9f;

                ReplaceDoor(bossRoom.GetComponent<BossRoom>().GetDoors()[0]);
                bossHallway.endDoor.GetComponent<DoorModel>().Close();

                bossFightCanvas.enabled = true;

                // Play the boss music
                BackgroundMusicManager.SwitchToBossBackgroundMusic();
            }
        }

        private void ReplaceDoor(GameObject door)
        {
            RoomEditor.DoorBlock lDoorBlock = door.GetComponent<RoomEditor.DoorBlock>();
            if (lDoorBlock != null)
                lDoorBlock.CloseDoor();
        }

        private void Update()
        {
            if(!fightStarted && triggered && !DialogueManager.instance.dialogueActive)
            {
                bossRoom.GetComponent<BossRoom>().SetPlayerEnterBossFight(true);
                bossProtection.SetActive(false);
            }
        }
    }
}
