using System.Collections;
using Core.Audio;
using UnityEngine;

namespace Core.Enemy.StageBosses.Stage1
{
    public class ClockShot : BossAbility
    {
        [Header("Bullet")]
        [SerializeField]
        private GameObject bullet;
        [SerializeField]
        private float bulletSpeed;
        [SerializeField]
        private int minDamage;
        [SerializeField]
        private int maxDamage;

        [SerializeField]
        private float radius = 3f;

        [Header("Clockshot")]
        [SerializeField]
        private float clockShotInterval;
        private int _amountOfBullets;

        [Header("Bullet styling")]
        [ColorUsageAttribute(true, true)]
        public Color albedo;
        [ColorUsageAttribute(true, true)]
        public Color glow;
        public float glowPower;
        public Gradient trailGradient;

        [Header("Effect styling")]
        public Color standardColor;
        [ColorUsageAttribute(true, true)]
        public Color emission;
        public Color nonEmissive;

        private GameObject _boss;
        private Quaternion _bulletRot;
        private GameObject _player;
        private Animator _bossAnimator;

        private void Start()
        {
            _boss = BossManager.Instance.boss;
            _player = PlayerManager.Instance.player;
            _bossAnimator = GameObject.Find("PirateNew").GetComponent<Animator>();

            _amountOfBullets = 360 / 30;
        }

        public override IEnumerator UseBossAbility()
        {
            _bossAnimator.SetTrigger("clockshot");
            yield return new WaitForSeconds(1f);

            yield return StartCoroutine(Shoot());
        }

        private IEnumerator Shoot()
        {
            var angleIncrement = 360f / _amountOfBullets;

            for (var i = 0; i < _amountOfBullets; i++)
            {
                var rotationAngle = i * angleIncrement;

                var x = radius * Mathf.Cos(rotationAngle * Mathf.Deg2Rad);
                var z = radius * Mathf.Sin(rotationAngle * Mathf.Deg2Rad);

                var bulletPosition = transform.position + new Vector3(x, 0, z);

                var newBullet = Instantiate(bullet, bulletPosition, Quaternion.identity);
                ClockBulletBehaviour newClockBulletBehaviour = newBullet.GetComponentInChildren<ClockBulletBehaviour>();

                newClockBulletBehaviour.SetDamage(minDamage, maxDamage);
                newClockBulletBehaviour.SetSpeed(bulletSpeed);
                newClockBulletBehaviour.SetBulletStyle(albedo, glow, glowPower, trailGradient);
                newClockBulletBehaviour.SetBulletDestructionStyle(standardColor, emission, nonEmissive);

                newBullet.transform.forward = newBullet.transform.position - transform.position;
            }

            SoundEffectsManager.instance.PlaySoundEffect(SoundEffect.BOSS_CLOCK_SHOT);

            yield return new WaitForSeconds(clockShotInterval);
        }
    }
}
