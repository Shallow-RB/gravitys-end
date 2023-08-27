using System.Collections.Generic;
using UnityEngine;

namespace Core.Chest
{
    public class ChestSpawner : MonoBehaviour
    {
        [Range(0f, 1f)] public float chestSpawnChance = 0.15f;

        [SerializeField] public Transform[] chestSpawnPoints;

        [SerializeField] public GameObject chest;

        [SerializeField] List<LootItem> LootItems;

        public void SpawnChest()
        {
            foreach(Transform spawnpoint in chestSpawnPoints)
            {
                if(Random.value < chestSpawnChance)
                {
                    GameObject spawnedChest = Instantiate(chest, spawnpoint);
                    spawnedChest.GetComponent<Chest>().SetLootObjects(LootItems);
                }
            }
        }
    }
}