using UI.Runtime;
using UI.Tokens;
using UnityEngine;
using UnityEngine.UI;

namespace Controllers.Player
{
    public class PlayerStatsController : MonoBehaviour
    {
        [SerializeField]
        private float health;

        [HideInInspector]
        public int hitCounter;

        private float _currentHealth;
        private Slider _slider;
        private Character _player;
        private ParticleSystem _hitParticle;

        private void Start()
        {
            _currentHealth = health;
            _slider = GameObject.Find("Healthbar").GetComponent<Slider>();
            _slider.maxValue = _currentHealth;

            _player = transform.GetComponent<Character>();
            _hitParticle = _player.hitParticle;
        }

        private void Update()
        {
            _slider.value = _currentHealth;
        }

        public void TakeDamage(int startDamage, int endDamage, float modifier)
        {
            var damage = Random.Range(startDamage, endDamage);
            // Subtract the armor value
            damage -= Mathf.RoundToInt(modifier * damage);
            // Substract the damage reduction
            damage -= Mathf.RoundToInt((TokenManager.instance.healthSection.GetModifier() - 1) * damage);
            damage = Mathf.Clamp(damage, 0, int.MaxValue);

            // Damage character
            _currentHealth -= damage;

            if (!_hitParticle.isPlaying)
            {
                _hitParticle.Play();
            }

            if (_currentHealth <= 0)
                Die();
        }

        public void HealPlayer(float healPlayerAmount)
        {
            if (health - _currentHealth < healPlayerAmount)
                healPlayerAmount = health - _currentHealth;

            if (_currentHealth < health)
                _currentHealth += healPlayerAmount;
        }

        private void Die()
        {
            // Load the death scene
            GameOver.Instance.PlayerGameOver();
        }
    }
}
