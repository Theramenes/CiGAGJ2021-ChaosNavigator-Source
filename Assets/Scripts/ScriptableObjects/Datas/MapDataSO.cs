using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Data/Map Data", fileName = "Map Data" ,order = 3)]
public class MapDataSO : ScriptableObject
{
    public float InitialMapScale = 1f;

    public float MapScale;
    public float MapEdgeLength;

    // 摄像机可见范围，用于配合地图范围的变化
    [SerializeField]
    private float InitialCameraOrthographicSize = 4f;
    public float CameraOrthographicSize;


    [Header("")]
    public List<Vector2> MapVertices;

    [Header("Map Area Data")]
    [SerializeField]
    public Area BlackHoleMoveArea;

    [SerializeField]
    public List<Area> AircraftSpawnArea;

    [SerializeField]
    public List<Area> StationSpawnArea;

    [Header("Map Scaling Data")]
    public List<MapScalingData> MapScalingDataList;
    [SerializeField]
    private int mapScalingStage;

    [Header("Map Scaling Animation Data")]
    public AnimationCurve CameraFieldCurve;
    public float CameraFieldAnimatDuration;
    public AnimationCurve MapScalingCurve;
    public float MapScalingAnimatDuration;


    public void Initialize()
    {
        MapScale = InitialMapScale;
        CameraOrthographicSize = InitialCameraOrthographicSize;

        MapEdgeLength = MapScale * 5f;
        mapScalingStage = 0;

        UpdateAircraftSpawnArea();
        UpdateStationSpawnArea();
    }

    public void MapScaling(float gameTime)
    {
        if (mapScalingStage == MapScalingDataList.Count - 1)
            return;

        if(MapScalingDataList[mapScalingStage+1].gameTime < gameTime)
        {
            mapScalingStage++;
            UpdateMapData();
            UpdateBlackHoleMoveArea();
            UpdateAircraftSpawnArea();
            UpdateStationSpawnArea();
        }

        //if (mapScalingStage < MapScalingDataList.Count)
        //{

        //}
    }

    public bool CanDoMapScaling(float gameTime)
    {
        if (mapScalingStage == MapScalingDataList.Count - 1)
            return false;

        return MapScalingDataList[mapScalingStage + 1].gameTime <= gameTime;
    
    }

    public Vector3 GetAircraftSpawnPoint(int area)
    {
        return AircraftSpawnArea[area].GetRandomPointInArea();
    }



    public void UpdateMapData()
    {
        MapScale = MapScalingDataList[mapScalingStage].mapScale;
        MapEdgeLength = MapScale * 5f;
        CameraOrthographicSize = InitialCameraOrthographicSize * MapScale;
    }

    public void UpdateBlackHoleMoveArea()
    {
        BlackHoleMoveArea.LDCornerVertice = new Vector3(-MapEdgeLength, 0f, -MapEdgeLength);
        BlackHoleMoveArea.RTCornerVertice = new Vector3(MapEdgeLength, 0f, MapEdgeLength);
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


    [System.Serializable]
    public struct MapScalingData
    {
        public float gameTime;
        public float mapScale;

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

    public Vector3 GetCenterPointInArea()
    {
        return new Vector3((LDCornerVertice.x + RTCornerVertice.x) / 2f, 0f, (LDCornerVertice.z + RTCornerVertice.z) / 2f);
    }

    // Area size 指的是边长
    public Vector3 GetAreaSize()
    {
        return new Vector3(Mathf.Abs(LDCornerVertice.x - RTCornerVertice.x), 0f, Mathf.Abs(LDCornerVertice.z - RTCornerVertice.z));
    }
}

