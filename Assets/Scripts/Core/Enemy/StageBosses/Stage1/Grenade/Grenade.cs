using System.Collections;
using System.Collections.Generic;
using Core.Audio;
using Core.Enemy;
using Core.Enemy.StageBosses;
using UnityEngine;

namespace Core.Enemy.StageBosses.Stage1
{
    public class Grenade : BossAbility
    {
        [Header("Grenade")]
        [SerializeField]
        private GameObject grenade;
        [SerializeField]
        private float throwDuration;
        [SerializeField]
        private float curveHeight;
        [SerializeField]
        private int minDamage;
        [SerializeField]
        private int maxDamage;
        [SerializeField]
        private float grenadeInterval;

        [Header("Decal")]
        [SerializeField]
        private GameObject decal;
        [SerializeField]
        private float decalRadius;

        [Header("Effect")]
        [SerializeField]
        private ParticleSystem destructionEffect;

        private GameObject _boss;
        private Animator _bossAnimator;
        private GameObject _player;

        private void Start()
        {
            _boss = BossManager.Instance.boss;
            _bossAnimator = GameObject.Find("PirateNew").GetComponent<Animator>();

            _player = PlayerManager.Instance.player;
        }

        public override IEnumerator UseBossAbility()
        {
            _bossAnimator.SetTrigger("grenade");
            yield return new WaitForSeconds(0.5f);

            ThrowGrenade();

            yield return new WaitForSeconds(grenadeInterval);
        }

        private void ThrowGrenade()
        {
            GameObject newGrenade = Instantiate(grenade, transform.position, Quaternion.identity);
            GrenadeBehavior newGrenadeBehavior = newGrenade.GetComponentInChildren<GrenadeBehavior>();

            GameObject newDecal = Instantiate(decal);
            newDecal.GetComponent<Decal>().SetRadius(decalRadius);

            newGrenadeBehavior.SetDecal(newDecal);
            newGrenadeBehavior.SetDamage(minDamage, maxDamage);
            newGrenadeBehavior.SetThrowDuration(throwDuration);
            newGrenadeBehavior.SetCurveHeight(curveHeight);
            newGrenadeBehavior.SetDestructionEffect(destructionEffect);

            SoundEffectsManager.instance.PlaySoundEffect(SoundEffect.BOSS_SHOOTS);
        }
    }
}
