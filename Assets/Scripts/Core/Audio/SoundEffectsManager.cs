using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Core.Audio
{

    public class SoundEffectsManager : MonoBehaviour
    {
        public static SoundEffectsManager instance;

        // The audiosource where the soundeffects will be playing from
        private AudioSource soundEffectSource;

        [SerializeField] private Slider soundEffectsSlider;

        // Sound effects 
        private Dictionary<SoundEffect, AudioClip> soundEffects;

        [SerializeField] private AudioClip objectiveCompleted;
        [SerializeField] private AudioClip enemyShoots;
        [SerializeField] private AudioClip playerGunShotLow;
        [SerializeField] private AudioClip playerGunShotMid;
        [SerializeField] private AudioClip playerGunShotHeavy;
        [SerializeField] private AudioClip swordAttack;
        [SerializeField] private AudioClip punchingAir;
        [SerializeField] private AudioClip chestOpening;
        [SerializeField] private AudioClip gunPickup;
        [SerializeField] private AudioClip walkingSound;
        [SerializeField] private AudioClip bossShoots;
        [SerializeField] private AudioClip bossDies;
        [SerializeField] private AudioClip dashingSound;
        [SerializeField] private AudioClip bossClockShot;
        [SerializeField] private AudioClip bossYells;
        [SerializeField] private AudioClip playerTakesDamage;
        [SerializeField] private AudioClip armorPickup;
        [SerializeField] private AudioClip clockTicking;


        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            soundEffectSource = GetComponent<AudioSource>();

            // Check if PlayerPrefs has a stored value for SoundEffectsVolume
            if (!PlayerPrefs.HasKey("SoundEffectsVolume"))
            {
                SetSoundEffectsVolume(0.5f);
            }
            SetSoundEffectsVolume(PlayerPrefs.GetFloat("SoundEffectsVolume"));
        }

        private void Start()
        {
            // Initialize the soundEffects dictionary and assign the AudioClips
            soundEffects = new Dictionary<SoundEffect, AudioClip>();

            // Adding the sound effects to the dictionary
            soundEffects.Add(SoundEffect.OBJECTIVE_COMPLETED, objectiveCompleted);
            soundEffects.Add(SoundEffect.ENEMY_SHOOTS, enemyShoots);
            soundEffects.Add(SoundEffect.PLAYER_GUN_SHOT_LOW, playerGunShotLow);
            soundEffects.Add(SoundEffect.PLAYER_GUN_SHOT_MID, playerGunShotMid);
            soundEffects.Add(SoundEffect.PLAYER_GUN_SHOT_HEAVY, playerGunShotHeavy);
            soundEffects.Add(SoundEffect.SWORD_ATTACk, swordAttack);
            soundEffects.Add(SoundEffect.PUNCHING_AIR, punchingAir);
            soundEffects.Add(SoundEffect.CHEST_OPENING, chestOpening);
            soundEffects.Add(SoundEffect.GUN_PICKUP, gunPickup);
            soundEffects.Add(SoundEffect.BOSS_SHOOTS, bossShoots);
            soundEffects.Add(SoundEffect.BOSS_DIES, bossDies);
            soundEffects.Add(SoundEffect.DASH, dashingSound);
            soundEffects.Add(SoundEffect.BOSS_CLOCK_SHOT, bossClockShot);
            soundEffects.Add(SoundEffect.BOSS_YELLS, bossYells);
            soundEffects.Add(SoundEffect.PLAYER_TAKE_DAMAGE, playerTakesDamage);
            soundEffects.Add(SoundEffect.ARMOR_PICKUP, armorPickup);
            soundEffects.Add(SoundEffect.CLOCK_TICKING, clockTicking);
        }

        public void PlaySoundEffect(SoundEffect soundEffectType)
        {
            if (soundEffects.ContainsKey(soundEffectType))
            {
                AudioClip soundEffect = soundEffects[soundEffectType];
                soundEffectSource.PlayOneShot(soundEffect);
            }
            else
            {
                Debug.LogError("Sound effect '" + soundEffectType + "' not found!");
            }
        }

        public void StopSoundEffect(SoundEffect soundEffectType)
        {
            soundEffectSource.Stop();
        }

        public void SetSoundEffectsVolume(float volume)
        {
            soundEffectSource.volume = volume;
            soundEffectsSlider.value = volume;
            PlayerPrefs.SetFloat("SoundEffectsVolume", volume);
            PlayerPrefs.Save();
        }
    }

    public enum SoundEffect
    {
        OBJECTIVE_COMPLETED,
        ENEMY_SHOOTS,
        PLAYER_GUN_SHOT_LOW,
        PLAYER_GUN_SHOT_MID,
        PLAYER_GUN_SHOT_HEAVY,
        SWORD_ATTACk,
        PUNCHING_AIR,
        CHEST_OPENING,
        GUN_PICKUP,
        BOSS_SHOOTS,
        BOSS_CLOCK_SHOT,
        BOSS_DIES,
        DASH,
        BOSS_YELLS,
        PLAYER_TAKE_DAMAGE,
        ARMOR_PICKUP,
        CLOCK_TICKING
    }
}
