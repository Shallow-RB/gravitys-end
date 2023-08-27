using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace Core
{
    public class EnemyGeneration : MonoBehaviour
    {
        [SerializeField]
        private List<EnemySpawnpoints> enemySpawnpoints;
        [SerializeField]
        private EnemyTypes enemyTypes;

        IEnumerator EnemyDrop()
        {
            int randomEnemyTypeInt = PickRandomEnemytype();
            
            BannerMaterialSwapper swapper = gameObject.GetComponent<BannerMaterialSwapper>();
            if (swapper != null)
                swapper.UpdateMaterials(randomEnemyTypeInt);

            EnemyType enemyType = enemyTypes.enemyTypes[randomEnemyTypeInt];

            // Iterate through each EnemySpawnpoints object
            foreach (EnemySpawnpoints spawnpoint in enemySpawnpoints)
            {
                // Generate a random number of enemies to spawn between minSpawn and maxSpawn
                int numEnemiesToSpawn = Random.Range(spawnpoint.minSpawn, spawnpoint.maxSpawn + 1);

                // Spawn the enemies
                if (numEnemiesToSpawn == 1)
                    Instantiate(PickRandomEnemy(enemyType.enemys), spawnpoint.spawnpoint.position, spawnpoint.spawnpoint.rotation);
                else if (numEnemiesToSpawn > 1) 
                {
                    for (int i = 0; i < numEnemiesToSpawn; i++)
                    {
                        float randomOffsetX = Random.Range(-1f, 1f);
                        float randomOffsetZ = Random.Range(-1f, 1f);
                        Vector3 spawnPosition = spawnpoint.spawnpoint.position + new Vector3(randomOffsetX, 0f, randomOffsetZ);
                        Instantiate(PickRandomEnemy(enemyType.enemys), spawnPosition, spawnpoint.spawnpoint.rotation);
                        yield return null;
                    }
                }
                
                yield return null;
            }
        }

        private GameObject PickRandomEnemy(List<EnemyVariant> enemyType)
        {
            List<int> randomizer = new();
            for (int i = 0; i < enemyType.Count; i++)
                for (int j = 0; j < enemyType[i].spawnWeight; j++)
                    randomizer.Add(i);

            int randomInt = Random.Range(0, randomizer.Count);
            EnemyVariant randomEnemy = enemyType[randomizer[randomInt]];
            return randomEnemy.enemy;
        }

        private int PickRandomEnemytype()
        {
            List<int> randomizer = new();
            for (int i = 0; i < enemyTypes.enemyTypes.Count; i++)
                for (int j = 0; j < enemyTypes.enemyTypes[i].typeWeight; j++)
                    randomizer.Add(i);

            int randomInt = Random.Range(0, randomizer.Count);
            return randomizer[randomInt];
        }

        public void SpawnEnemy()
        {
            StartCoroutine(EnemyDrop());
        }
    }
}
