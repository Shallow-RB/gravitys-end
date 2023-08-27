using System.Collections;
using System.Collections.Generic;
using Controllers.Player;
using UI.Inventory;
using UnityEngine;

namespace Core.Enemy.StageBosses.Stage1
{
    public class GrenadeBehavior : MonoBehaviour
    {
        private float _throwDuration;
        private float _curveHeight;
        private int _minDamage;
        private int _maxDamage;
        private ParticleSystem _destructionEffect;

        private float _throwStartTime = 0f;

        private GameObject _boss;
        private GameObject _player;
        private Vector3 _startPosition;
        private Quaternion _startRotation;

        private Vector3 _targetPosition;
        private Quaternion _targetRotation;

        private GameObject _decal;

        private bool playerIndecal = false;

        private void Start()
        {
            _boss = BossManager.Instance.boss;
            _player = PlayerManager.Instance.player;

            _startPosition = transform.position;
            _startRotation = transform.rotation;
            _targetPosition = _player.transform.position;
            _throwStartTime = Time.time;

            _destructionEffect = Instantiate(_destructionEffect);

            PlaceDecal();

            StartCoroutine(ThrowGrenade());
        }

        private void OnEnable()
        {
            Decal.OnPlayerDetected += UpdatePlayerIndecal;
        }

        private void OnDisable()
        {
            Decal.OnPlayerDetected -= UpdatePlayerIndecal;
        }

        private IEnumerator ThrowGrenade()
        {
            while (Time.time - _throwStartTime < _throwDuration)
            {
                float normalizedTime = (Time.time - _throwStartTime) / _throwDuration;

                Vector3 currentPos = CalculateThrowPosition(normalizedTime);
                transform.root.position = currentPos;

                Quaternion currentRot = CalculateThrowRotation(normalizedTime);
                transform.root.rotation = currentRot;

                _destructionEffect.gameObject.transform.position = transform.root.position;

                yield return null;
            }

            if (playerIndecal)
            {
                var armor = _player.GetComponent<EquipmentSystem>()._equippedArmor;
                _player.GetComponent<PlayerStatsController>().TakeDamage(_minDamage, _maxDamage, armor != null ? armor.GetComponent<Item>().GetArmorModifier() : 0);
            }

            _destructionEffect.Play();

            Destroy(transform.root.gameObject);
            Destroy(_decal);
        }

        private Vector3 CalculateThrowPosition(float normalizedTime)
        {
            Vector3 currentPos = Vector3.Lerp(_startPosition, _targetPosition, normalizedTime);
            currentPos.y += Mathf.Sin(normalizedTime * Mathf.PI) * _curveHeight;
            return currentPos;
        }

        private Quaternion CalculateThrowRotation(float normalizedTime)
        {
            return Quaternion.Slerp(_startRotation, _targetRotation, normalizedTime);
        }

        private void PlaceDecal()
        {
            Vector3 decalPos = new Vector3(_targetPosition.x, 0.3f, _targetPosition.z);

            _decal.transform.position = decalPos;
        }

        private void UpdatePlayerIndecal(bool playerEntered)
        {
            playerIndecal = playerEntered;
        }

        public void SetDecal(GameObject decal)
        {
            _decal = decal;
        }

        public void SetDamage(int minDamage, int maxDamage)
        {
            _minDamage = minDamage;
            _maxDamage = maxDamage;
        }

        public void SetThrowDuration(float throwDuration)
        {
            _throwDuration = throwDuration;
        }

        public void SetCurveHeight(float curveHeight)
        {
            _curveHeight = curveHeight;
        }

        public void SetDestructionEffect(ParticleSystem destructionEffect)
        {
            _destructionEffect = destructionEffect;
        }
    }
}
