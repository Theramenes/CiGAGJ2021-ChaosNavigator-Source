using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
