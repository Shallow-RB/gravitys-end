using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemySpawnpoints
{
    [SerializeField]
    public Transform spawnpoint;
    [SerializeField]
    public int minSpawn;
    [SerializeField]
    public int maxSpawn;
}
