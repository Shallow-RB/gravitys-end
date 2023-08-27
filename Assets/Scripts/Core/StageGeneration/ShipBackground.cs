using Core.StageGeneration.Stage;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipBackground : MonoBehaviour
{
    void Start()
    {
        Vector2Int scale = gameObject.GetComponent<StageGenerator>().GetXZ();
        transform.localScale = new Vector3(scale.x, 1, scale.y);
    }
}
