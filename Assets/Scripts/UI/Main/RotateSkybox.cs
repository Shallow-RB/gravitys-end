using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateSkybox : MonoBehaviour
{
    public float rotationSpeed = 1f;
    public float maxRotationAngle = 30f;

    private Vector3 targetRotation;

    private void Start()
    {
        // Set a random target rotation
        targetRotation = new Vector3(
            Random.Range(-maxRotationAngle, maxRotationAngle),
            Random.Range(-maxRotationAngle, maxRotationAngle),
            Random.Range(-maxRotationAngle, maxRotationAngle)
        );
    }

    private void Update()
    {
        // Rotate towards the target rotation gradually
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            Quaternion.Euler(targetRotation),
            rotationSpeed * Time.deltaTime
        );

        // If the camera nearly reaches the target rotation, set a new target rotation
        if (Quaternion.Angle(transform.rotation, Quaternion.Euler(targetRotation)) < 1f)
        {
            targetRotation = new Vector3(
                Random.Range(-maxRotationAngle, maxRotationAngle),
                Random.Range(-maxRotationAngle, maxRotationAngle),
                Random.Range(-maxRotationAngle, maxRotationAngle)
            );
        }
    }
}
