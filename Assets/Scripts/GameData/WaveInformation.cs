using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Wave
{
    public string[] enemyTypes;
    public int[] enemyQuantities;
}

public class WaveData
{
    public List<Wave> waves;
}
