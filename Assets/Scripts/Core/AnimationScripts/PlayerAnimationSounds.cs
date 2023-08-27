using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationSounds : MonoBehaviour
{

    AudioSource animationSoundPlayer;

    [SerializeField]
    private AudioClip[] animationSounds;


    // Start is called before the first frame update
    void Start()
    {
        animationSoundPlayer = GetComponent<AudioSource>();

    }

    // Update is called once per frame
    void Update()
    {

    }

    // first element in animationSounds is footstep sound - index 0
    private void PlayerFootstepSound()
    {
        animationSoundPlayer.clip = animationSounds[0];
        animationSoundPlayer.Play();
    }

    // second element in animationSounds is shoot sound - index 1
    private void PlayerShootSound()
    {
        animationSoundPlayer.clip = animationSounds[1];
        animationSoundPlayer.Play();
    }

    // third element in animationSounds is dash sound - index 2
    private void PlayerSlash1Sound()
    {
        animationSoundPlayer.clip = animationSounds[2];
        animationSoundPlayer.Play();

    }

    // fourth element in animationSounds is slash sound - index 3
    private void PlayerSlash2Sound()
    {
        animationSoundPlayer.clip = animationSounds[3];
        animationSoundPlayer.Play();
    }

    // fifth element in animationSounds is slash sound - index 4
    private void PlayerSlash3Sound()
    {
        animationSoundPlayer.clip = animationSounds[4];
        animationSoundPlayer.Play();
    }
}
