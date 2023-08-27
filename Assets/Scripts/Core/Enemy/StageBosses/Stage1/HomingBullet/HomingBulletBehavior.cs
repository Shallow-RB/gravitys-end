using System.Collections;
using System.Collections.Generic;
using Controllers.Player;
using Core.Enemy;
using UI.Inventory;
using UnityEngine;

namespace Core.Enemy.StageBosses.Stage1
{
    public class HomingBulletBehavior : MonoBehaviour
    {
        private int _minDamage;
        private int _maxDamage;
        private float _speed;
        private float _rotationSpeed;
        private ParticleSystem _destructionEffect;

        private float _timeAlive;
        private float _allowedTimeAlive;

        private GameObject _boss;
        private GameObject _player;
        private Vector3 _startPosition;
        private Vector3 _targetPosition;

        private void Start()
        {
            _boss = BossManager.Instance.boss;
            _player = PlayerManager.Instance.player;

            _startPosition = transform.position;

            _allowedTimeAlive = _timeAlive + Time.time;

            transform.root.LookAt(_player.transform.position);

            _destructionEffect = Instantiate(_destructionEffect);
        }

        private void Update()
        {
            if (_allowedTimeAlive <= Time.time)
            {
                _destructionEffect.Play();
                Destroy(transform.root.gameObject);
            }

            _targetPosition = _player.transform.position;
            // Preserve starting y position
            _targetPosition.y = transform.root.position.y;

            // Calculate the rotation towards the target
            Quaternion rotation = Quaternion.LookRotation(_targetPosition - transform.root.position);

            // Smoothly rotate towards the target with curve modifier
            transform.root.rotation = Quaternion.Slerp(transform.root.rotation, rotation, _rotationSpeed * Time.deltaTime);

            // Move forward in the direction of the current rotation
            transform.root.Translate(transform.root.forward * _speed * Time.deltaTime, Space.World);
            _destructionEffect.gameObject.transform.position = transform.root.position;
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                var armor = _player.GetComponent<EquipmentSystem>()._equippedArmor;
                _player.GetComponent<PlayerStatsController>().TakeDamage(_minDamage, _maxDamage, armor != null ? armor.GetComponent<Item>().GetArmorModifier() : 0);

                Destroy(gameObject);
            }

            if (other.gameObject.CompareTag("Wall") || other.gameObject.CompareTag("Door") || other.gameObject.CompareTag("BulletInteract"))
            {
                _destructionEffect.Play();
                Destroy(gameObject);
            }
        }

        public void SetDamage(int minDamage, int maxDamage)
        {
            _minDamage = minDamage;
            _maxDamage = maxDamage;
        }

        public void SetSpeed(float speed)
        {
            _speed = speed;
        }

        public void SetRotationSpeed(float rotationSpeed)
        {
            _rotationSpeed = rotationSpeed;
        }

        public void SetTimeAlive(float timeAlive)
        {
            _timeAlive = timeAlive;
        }

        public void SetDestructionEffect(ParticleSystem destructionEffect)
        {
            _destructionEffect = destructionEffect;
        }
    }
}
