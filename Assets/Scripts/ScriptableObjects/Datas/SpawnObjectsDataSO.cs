using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Data/Spawn Data", fileName = "Spawn Objects Data")]

public class SpawnObjectsDataSO : ScriptableObject
{
    public int AircraftCount;
    public int StationCount;
    public int MetoerCount;

    public int TotalCapacity;

    public void Initialize()
    {
        AircraftCount = 0;
        StationCount = 0;
        MetoerCount = 0;
    }

}
