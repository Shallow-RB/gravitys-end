using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoomEditor {
	public class DoorBlock : MonoBehaviour
	{
		public GameObject WallBlock;
		public GameObject DoorModelObject;

		[SerializeField]
		private GameObject wallBlockObject;

		public void CloseDoor() {
			gameObject.SetActive(true);

			// Closes the door model
			DoorModelObject.GetComponent<DoorModel>().Close();

			// Creates a wall block
			wallBlockObject = Instantiate(WallBlock, this.transform.position, this.transform.rotation, this.transform.parent);
			wallBlockObject.transform.localScale = this.transform.localScale;
		}

		public void OpenDoor() {
			// Closes the door model
			DoorModelObject.GetComponent<DoorModel>().Open();

			// destroyes the wall block
			Destroy(wallBlockObject);

			gameObject.SetActive(false);
		}
	}
}
