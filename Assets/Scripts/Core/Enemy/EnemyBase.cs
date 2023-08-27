using System.Collections;
using Controllers;
using UI;
using UI.Enemy;
using UnityEngine;

namespace Core.Enemy
{
    public class EnemyBase : MonoBehaviour
    {
        public float health;
        public Material hitMaterial;

        public GameObject damageDisplay;

        private Canvas _canvas;
        private HeathDisplay _healthDisplay;

        private float _currentHealth;
        private ParticleSystem _hitParticle;

        public static int enemyKillCounter;

        public delegate void EnemyKilledEventHandler();

        public static event EnemyKilledEventHandler OnEnemyKilled;

        private void Start()
        {
            _canvas = GetComponentInChildren<Canvas>();

            _hitParticle = GetComponent<EnemyController>().hitParticle;
            _currentHealth = health;
            _healthDisplay = gameObject.GetComponent<HeathDisplay>();

            _healthDisplay.UpdateHealthBar(health, _currentHealth);
            _canvas.gameObject.SetActive(false);
        }

        public void TakeDamage(int takeStartDamage, int takeEndDamage, float modifier)
        {
            var damage = Random.Range(takeStartDamage, takeEndDamage);
            damage -= Mathf.RoundToInt(modifier) * damage;
            damage = Mathf.Clamp(damage, 0, int.MaxValue);

            _currentHealth -= damage;

            if (damageDisplay != null)
            {
                if(!_canvas.gameObject.activeSelf)
                    _canvas.gameObject.SetActive(true);

                gameObject.GetComponent<HeathDisplay>().UpdateHealthBar(health, _currentHealth);
                damageDisplay.GetComponent<DamageDisplay>().Show(damage.ToString(), damageDisplay, _canvas, new Color(1f, 0.2f, 0.2f, 1));
            }

            if (!_hitParticle.isPlaying)
            {
                _hitParticle.Play();
            }

            if (_currentHealth <= 0)
            {
                Destroy(gameObject);
                GameStats.Instance.enemiesKilled++;
                OnEnemyKilled?.Invoke();
            }
        }

        public bool EnemyHeal(float healthIncrease)
        {
            if (_currentHealth <= 0)
                return false;

            float roundedHealthIncrease = Mathf.RoundToInt(healthIncrease);
            if (_currentHealth < health)
            {
                if (health - _currentHealth < roundedHealthIncrease)
                    roundedHealthIncrease = health - _currentHealth;

                _currentHealth += roundedHealthIncrease;

                if (damageDisplay != null)
                {
                    gameObject.GetComponent<HeathDisplay>().UpdateHealthBar(health, _currentHealth);
                    damageDisplay.GetComponent<DamageDisplay>().Show(roundedHealthIncrease.ToString(), damageDisplay, _canvas, Color.green);
                    if (_currentHealth == health)
                        _canvas.gameObject.SetActive(false);
                }
            }
            return true;
        }

        private IEnumerator HitFeedback()
        {
            yield return new WaitForSeconds(1f);
        }
    }
}
