using System.Collections;
using System.Collections.Generic;
using Controllers.Enemy;
using Core.Audio;
using Core.Enemy.StageBosses;
using TMPro;
using UI;
using UI.Enemy;
using UI.Runtime;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Enemy
{
    public class Boss : MonoBehaviour
    {
        [SerializeField]
        private float health;

        [SerializeField]
        private Slider healthBar;

        [SerializeField]
        private TextMeshProUGUI bossNameBar;

        [SerializeField]
        private List<BossAbilityStage> bossAbilityStages;

        public GameObject damageDisplay;

        [SerializeField]
        private ParticleSystem hitParticle;

        private Canvas _canvas;

        private BossAbilityStage _currentBossStage;
        private BossAbilitySequence _currentBossSequence;
        private BossAbility _currentBossAbility;

        private int _currentStageIndex = 0;
        private int _currentSequenceIndex = 0;

        private float _currentHealth;
        private bool _startAbilitiesSequence;
        private bool _startFight;

        private BossController _bossController;

        private void Start()
        {
            _currentHealth = health;

            healthBar.maxValue = health;
            healthBar.value = healthBar.maxValue;

            bossNameBar.text = name;

            _currentBossStage = bossAbilityStages[_currentStageIndex];

            _canvas = GetComponentInChildren<Canvas>();
            _bossController = GetComponent<BossController>();
        }

        private void Update()
        {
            if (!_startFight || _startAbilitiesSequence) return;

            StartCoroutine(LoopBossSequence());
            _startAbilitiesSequence = true;
        }

        private void LoopBossStages()
        {
            // Check if it wont go out of bounds
            if (_currentStageIndex + 1 < bossAbilityStages.Count)
            {
                // If the current health is or lower than the next health stage activation
                // Set the next stage as current stage
                if (_currentHealth <= bossAbilityStages[_currentStageIndex + 1].GetHealhStageActivation())
                {
                    _currentBossStage = bossAbilityStages[++_currentStageIndex];
                    _currentSequenceIndex = 0;
                }
            }
        }

        private IEnumerator LoopBossSequence()
        {
            while (true)
            {
                // Loop boss stages
                LoopBossStages();

                // Set the current sequence
                _currentBossSequence = _currentBossStage.GetBossAbilitySequences()[_currentSequenceIndex];

                // Set the current ability of the sequence
                _currentBossAbility = _currentBossSequence.GetBossAbility();

                //Use the current ability of the sequence
                yield return StartCoroutine(_currentBossAbility.UseBossAbility());

                // Increment the number of times used for the current sequence
                _currentBossSequence.IncrementAmountOfTimesUsed();

                // Check if we've used the current sequence enough times
                if (_currentBossSequence.GetAmountOfTimesUsed() != _currentBossSequence.GetAmountOfTimes())
                    continue;

                // Reset the times used for the current sequence
                _currentBossSequence.SetAmountOfTimesUsed(0);

                // Move to the next sequence
                _currentSequenceIndex++;
                if (_currentSequenceIndex >= _currentBossStage.GetBossAbilitySequences().Count)
                {
                    // We've reached the end of the sequences, so cycle back to the first sequence
                    _currentSequenceIndex = 0;
                }
            }
        }

        public void TakeDamage(int takeStartDamage, int takeEndDamage, float modifier)
        {
            var damage = Random.Range(takeStartDamage, takeEndDamage);
            damage -= Mathf.RoundToInt(modifier / 100) * damage;
            damage = Mathf.Clamp(damage, 0, int.MaxValue);

            if (damageDisplay != null)
                damageDisplay.GetComponent<DamageDisplay>().Show(damage.ToString(), damageDisplay, _canvas, Color.red);

            _currentHealth -= damage;
            SoundEffectsManager.instance.PlaySoundEffect(SoundEffect.BOSS_YELLS);

            healthBar.value = _currentHealth;

            if (!hitParticle.isPlaying)
            {
                hitParticle.Play();
            }

            if (_currentHealth <= 0)
            {
                ObjectiveSystem.instance.HandleBossKilled();
                Destroy(gameObject);

                SoundEffectsManager.instance.PlaySoundEffect(SoundEffect.BOSS_DIES);
                GameOver.Instance.PlayerEndGame();
            }
        }

        public bool GetStartFight()
        {
            return _startFight;
        }

        public void SetStartFight(bool startFight)
        {
            _startFight = startFight;
        }

        public BossAbility GetCurrentAbility()
        {
            return _currentBossAbility;
        }

        public List<BossAbilityStage> GetBossAbilityStages()
        {
            return bossAbilityStages;
        }

        public float GetHealth()
        {
            return _currentHealth;
        }
    }
}
