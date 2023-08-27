using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateShip : MonoBehaviour
{
    [SerializeField]
    private float rotationSpeed = 2f;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);
    }
}
