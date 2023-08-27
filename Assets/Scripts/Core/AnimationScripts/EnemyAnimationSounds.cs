using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationSounds : MonoBehaviour
{
    AudioSource animationSoundEnemy;
    // Start is called before the first frame update
    void Start()
    {
        animationSoundEnemy = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void EnemyFootstepSound() 
    {
        animationSoundEnemy.Play();
    }
}
