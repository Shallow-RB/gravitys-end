using System.Collections;
using System.Collections.Generic;
using UI.Inventory;
using UnityEngine;

namespace Core
{
    public class LootGeneration : MonoBehaviour
    {
        [SerializeField]
        private List<Item> lootObjects;

        [SerializeField]
        private Vector2 xRange;

        [SerializeField]
        private Vector2 zRange;

        private Vector3 _spawnPointPos;

        public IEnumerator SpawnLoot(GameObject spawnRoom)
        {
            _spawnPointPos = gameObject.transform.position;
            // Spawns amount of loot objects based on the amount of loot objects in the list
            foreach (var item in lootObjects)
            {
                var xOffset = Random.Range(xRange.x, xRange.y);
                var zOffset = Random.Range(zRange.x, zRange.y);

                item.Spawn(new Vector3(_spawnPointPos.x - xOffset, item.prefab.transform.position.y,
                    _spawnPointPos.z - zOffset));
                // Waits 0.1 seconds before spawning the next loot object because of performance
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}
