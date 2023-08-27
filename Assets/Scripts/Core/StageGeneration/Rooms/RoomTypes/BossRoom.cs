using Core.Enemy;
using UnityEngine;

namespace Core.StageGeneration.Rooms.RoomTypes
{
    public class BossRoom : Room
    {
        [SerializeField]
        private GameObject roomBossSpawnPoint;

        private bool _playerEnterBossFight;

        private GameObject _roomBoss;

        private void Start()
        {
            _roomBoss = BossManager.Instance.boss;
        }

        private void Update()
        {
            if (_playerEnterBossFight && _roomBoss != null) _roomBoss.GetComponent<Boss>().SetStartFight(true);
        }

        public bool GetPlayerEnterBossFight()
        {
            return _playerEnterBossFight;
        }

        public void SetPlayerEnterBossFight(bool bossFight)
        {
            _playerEnterBossFight = bossFight;
        }

        public GameObject GetRoomBoss()
        {
            return _roomBoss;
        }
    }
}
