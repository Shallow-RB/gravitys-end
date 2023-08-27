using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceShipExplosion : MonoBehaviour
{
    [SerializeField]
    private Animator flashAnimator;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            flashAnimator.SetTrigger("Flash");
        }    
    }
}
