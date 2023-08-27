using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;

public class Keycard : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> mapIcons;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ObjectiveSystem.instance.HandleKeycardCollected();
            Destroy(gameObject);

            foreach(GameObject keyIcon in mapIcons)
                keyIcon.SetActive(false);
        }
    }
}
