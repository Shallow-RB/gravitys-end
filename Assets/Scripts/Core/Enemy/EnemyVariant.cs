using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemyVariant
{
    [SerializeField]
    public GameObject enemy;

    [SerializeField]
    public int spawnWeight;
}
