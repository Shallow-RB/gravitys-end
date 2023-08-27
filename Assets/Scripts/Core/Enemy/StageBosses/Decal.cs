using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decal : MonoBehaviour
{
    private float radius;

    public delegate void DetectPlayerEventHandler(bool playerEntered);
    public static event DetectPlayerEventHandler OnPlayerDetected;

    public void SetRadius(float radius)
    {
        this.radius = radius;
        transform.localScale = new Vector3(radius, transform.localScale.y, radius);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnPlayerDetected?.Invoke(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnPlayerDetected?.Invoke(false);
        }   
    }
}
