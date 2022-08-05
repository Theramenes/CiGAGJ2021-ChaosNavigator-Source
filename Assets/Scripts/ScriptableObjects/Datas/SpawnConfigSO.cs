using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Data/Spawn Config", order = 4)]
public class SpawnConfigSO : ScriptableObject
{
    public float[] SpawnStageTime;

    public float[] SpawnInterval;

    public int currentSpawnStage;

    public void Initialize()
    {
        currentSpawnStage = 0;
    }

    public float GetSpawnInterval()
    {
        return SpawnInterval[currentSpawnStage];
    }
}
