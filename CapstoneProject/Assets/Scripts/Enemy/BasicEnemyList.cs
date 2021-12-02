using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BasicEnemyList
{
    public List<EnemySO> enemyList;
}

[System.Serializable]
public class WaveList
{
    public List<BasicEnemyList> enemyWaveList;
}
