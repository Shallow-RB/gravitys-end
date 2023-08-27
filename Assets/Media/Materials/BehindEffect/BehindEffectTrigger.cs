using RoomEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehindEffectTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Wall"))
            other.transform.GetComponent<WallSeeThrough>().DisableTiles();
        if (other.transform.CompareTag("BehindEffectTag"))
            foreach(Transform doorPart in other.transform)
                doorPart.GetComponent<MeshRenderer>().material.renderQueue = 3002;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag("Wall"))
            other.transform.GetComponent<WallSeeThrough>().EnableTiles();
        if (other.transform.CompareTag("BehindEffectTag"))
            foreach (Transform doorPart in other.transform)
                doorPart.GetComponent<MeshRenderer>().material.renderQueue = 3000;
    }
}
