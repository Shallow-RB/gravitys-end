using Controllers.Player;
using TMPro;
using UnityEngine;

namespace Core.Player
{
    public class DashTimer : MonoBehaviour
    {
        [SerializeField]
        private GameObject available;

        [SerializeField]
        private GameObject cooldown;

        private float _cooldown;

        private GameObject _player;

        private void Start()
        {
            cooldown.SetActive(false);
        }

        private void Update()
        {
            if (_player == null)
            {
                _player = PlayerManager.Instance.player;
                _cooldown = _player.GetComponent<Dashing>().GetDashTimer();
            }
            else
            {
                var dashAvailable = _player.GetComponent<Dashing>().GetDashAvailable();

                if (dashAvailable)
                {
                    available.SetActive(true);
                    cooldown.SetActive(false);
                }
                else
                {
                    available.SetActive(false);
                    cooldown.SetActive(true);
                }
            }
        }
    }
}
