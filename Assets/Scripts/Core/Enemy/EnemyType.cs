using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemyType
{
    [SerializeField]
    public List<EnemyVariant> enemys;
    [SerializeField]
    public int typeWeight;
}
