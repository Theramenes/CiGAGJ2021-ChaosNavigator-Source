using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Data/Map Data", fileName = "Map Data" ,order = 3)]
public class MapDataSO : ScriptableObject
{
    public float InitialMapScale = 1f;
    public float FinalMapScale = 3f;


    public float MapScale;
    public float MapEdgeLength;

    // 摄像机可见范围，用于配合地图范围的变化
    public float CameraOrthographicSize;

    public List<Vector2> MapVertices;
    [SerializeField]
    public List<Area> AircraftSpawnArea;

    [SerializeField]
    public List<Area> StationSpawnArea;


    public void Initialize()
    {
        MapScale = InitialMapScale;
        MapEdgeLength = MapScale * 5f;

        UpdateAircraftSpawnArea();
        UpdateStationSpawnArea();
    }

    public void UpdateAircraftSpawnArea()
    {
        if (AircraftSpawnArea.Count == 0)
        {
            for(int i = 0; i<=3; i++)
            {
                AircraftSpawnArea.Add(new Area());
            }
        }

        //AircraftSpawnArea[0].LDCornerVertice = new Vector3(-0.5f * MapScale, 0, 1.5f * MapScale);
        AircraftSpawnArea[0] = new Area
        {
            LDCornerVertice = new Vector3(-0.5f * MapEdgeLength, 0, 1.5f * MapEdgeLength),
            RTCornerVertice = new Vector3(0.5f * MapEdgeLength, 0, 2f * MapEdgeLength)
        };
        AircraftSpawnArea[1] = new Area
        {
            LDCornerVertice = new Vector3(1.5f * MapEdgeLength, 0, -0.5f * MapEdgeLength),
            RTCornerVertice = new Vector3(2f * MapEdgeLength, 0, 0.5f * MapEdgeLength)
        };
        AircraftSpawnArea[2] = new Area
        {
            LDCornerVertice = new Vector3(-0.5f * MapEdgeLength, 0, -2f * MapEdgeLength),
            RTCornerVertice = new Vector3(0.5f * MapEdgeLength, 0, -1.5f * MapEdgeLength)
        };
        AircraftSpawnArea[3] = new Area
        {
            LDCornerVertice = new Vector3(-2f * MapEdgeLength, 0, -0.5f * MapEdgeLength),
            RTCornerVertice = new Vector3(-1.5f * MapEdgeLength, 0, 0.5f * MapEdgeLength)
        };

        return;
    }

    public void UpdateStationSpawnArea()
    {
        if (StationSpawnArea.Count == 0)
        {
            for (int i = 0; i <= 3; i++)
            {
                StationSpawnArea.Add(new Area());
            }
        }

        //AircraftSpawnArea[0].LDCornerVertice = new Vector3(-0.5f * MapScale, 0, 1.5f * MapScale);
        StationSpawnArea[0] = new Area
        {
            LDCornerVertice = new Vector3(0, 0, 0),
            RTCornerVertice = new Vector3(MapEdgeLength, 0, MapEdgeLength)
        };
        StationSpawnArea[1] = new Area
        {
            LDCornerVertice = new Vector3(0, 0, -MapEdgeLength),
            RTCornerVertice = new Vector3(MapEdgeLength, 0, 0)
        };
        StationSpawnArea[2] = new Area
        {
            LDCornerVertice = new Vector3(-MapEdgeLength, 0, -MapEdgeLength),
            RTCornerVertice = new Vector3(0, 0, 0)
        };
        StationSpawnArea[3] = new Area
        {
            LDCornerVertice = new Vector3(-MapEdgeLength, 0, 0),
            RTCornerVertice = new Vector3(0, 0, MapEdgeLength)
        };


        return;
    }
}

[System.Serializable]
public struct Area
{
    // 区域左下角点
    public Vector3 LDCornerVertice;

    // 区域右上角点
    public Vector3 RTCornerVertice;

    public bool IsPointInArea(Vector3 point)
    {
        return (point.x >= LDCornerVertice.x && point.x <= RTCornerVertice.x) &&
            (point.z >= LDCornerVertice.z && point.z <= RTCornerVertice.z);
    }

    public Vector3 GetRandomPointInArea()
    {
        return new Vector3(Random.Range(LDCornerVertice.x, RTCornerVertice.x),
            0, Random.Range(LDCornerVertice.z, RTCornerVertice.z));
    }
}
