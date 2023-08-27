using System.Collections;
using Core.Audio;
using UnityEngine;

namespace Core.Enemy.StageBosses.Stage1
{
    public class HomingShot : BossAbility
    {
        [Header("Bullet")]
        [SerializeField]
        private GameObject bullet;
        [SerializeField]
        private float bulletSpeed;
        [SerializeField]
        private float bulletRotationSpeed;
        [SerializeField]
        private int minDamage;
        [SerializeField]
        private int maxDamage;
        [SerializeField]
        private float bulletInterval;
        [SerializeField]
        private float timeAlive;

        [Header("Spray logic")]
        [SerializeField]
        private float sprayAmountBullets;

        [Header("Effect")]
        [SerializeField]
        private ParticleSystem destructionEffect;

        [SerializeField]
        private float sprayInterval;

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
            yield return StartCoroutine(Spray());
        }

        private IEnumerator Spray()
        {
            for (var i = 0; i < sprayAmountBullets; i++)
            {
                _bossAnimator.SetBool("shoot", true);
                Shoot();
                yield return new WaitForSeconds(bulletInterval);
            }
            _bossAnimator.SetBool("shoot", false);
            yield return new WaitForSeconds(sprayInterval);
        }

        private void Shoot()
        {
            GameObject newBullet = Instantiate(bullet, transform.position, Quaternion.identity);
            HomingBulletBehavior newHomingBulletBehavior = newBullet.GetComponentInChildren<HomingBulletBehavior>();

            newHomingBulletBehavior.SetDamage(minDamage, maxDamage);
            newHomingBulletBehavior.SetSpeed(bulletSpeed);
            newHomingBulletBehavior.SetRotationSpeed(bulletRotationSpeed);
            newHomingBulletBehavior.SetTimeAlive(timeAlive);
            newHomingBulletBehavior.SetDestructionEffect(destructionEffect);

            SoundEffectsManager.instance.PlaySoundEffect(SoundEffect.BOSS_SHOOTS);
        }
    }
}
