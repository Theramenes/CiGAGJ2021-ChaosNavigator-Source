using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class MapManager : MonoBehaviour
{

    public GameObject GamePlane;
    public CinemachineVirtualCamera vCamera;

    [Header("Map Data")]
    public MapDataSO MapData;

    public float curMapScale;
    [SerializeField]
    private float targetMapScale;

    public float targetScale;
    public float lerpStep;

    [SerializeField]
    public float curCameraFiled;

    public float textScale;
    public Material material;

    private LerpEnumerator lerpTool;

    private void Awake()
    {
        Initialize();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //if (curMapScale < targetScale)
        //{
        //    curMapScale += lerpStep * Time.deltaTime;
        //    GamePlane.transform.localScale = new Vector3(curMapScale, curMapScale, curMapScale);

        //    //vCamera.m_Lens.OrthographicSize = 0.8f * curCameraFiled * curMapScale;
        //    vCamera.m_Lens.OrthographicSize = curCameraFiled * curMapScale;
        //    material.SetFloat("_Scale", textScale / curMapScale);
        //}
    }

    public void setTargetScale(float scale)
    {
        targetScale = scale;
    }

    public Vector4 getEdge()
    {
        return new Vector4(5 * curMapScale, -5 * curMapScale, -5 * curMapScale, 5 * curMapScale);
    }

    public void GetMapScalingRate()
    {

    }

    public void DoMapScale(float gameTime)
    {
        if (!MapData.CanDoMapScaling(gameTime))
            return;

        MapData.MapScaling(gameTime);

        material.SetFloat("_Scale", textScale / MapData.MapScale);

        StartCoroutine(lerpTool.FloatCurveLerp((var)=> vCamera.m_Lens.OrthographicSize = var,
            vCamera.m_Lens.OrthographicSize,
            MapData.CameraOrthographicSize,
            MapData.CameraFieldCurve, MapData.CameraFieldAnimatDuration)); 

        StartCoroutine(lerpTool.Vector3CurveLerp((var)=> GamePlane.transform.localScale = var,
            GamePlane.transform.localScale, 
            new Vector3(MapData.MapScale, MapData.MapScale, MapData.MapScale),
            MapData.MapScalingCurve, MapData.MapScalingAnimatDuration));

    }

    public bool IsBlackHoleInMoveArea(Vector3 pos)
    {
        return MapData.BlackHoleMoveArea.IsPointInArea(pos);
    }

    public int GetPointCurrentArea(Vector3 pos)
    {
        int area = 4;

        for(int i = 0; i<MapData.StationSpawnArea.Count; i++)
        {
            if (MapData.StationSpawnArea[i].IsPointInArea(pos))
                area = i;
        }
        return area;
    }

    public Vector3 GetAircraftSpawnPoint(int area)
    {
        return MapData.AircraftSpawnArea[area].GetRandomPointInArea();
    }

    public Vector3 GetStationSpawnPoint(int area)
    {
        return MapData.StationSpawnArea[area].GetRandomPointInArea();
    }

    public float GetMapEdgeRadius()
    {
        return 2 * MapData.MapEdgeLength;
    }

    private void Initialize()
    {
        MapData.Initialize();
        lerpTool = new LerpEnumerator();

        curMapScale = MapData.MapScale;
        curCameraFiled = MapData.CameraOrthographicSize;
        material = GamePlane.GetComponent<MeshRenderer>().material;
        textScale = material.GetFloat("_Scale");

        vCamera.m_Lens.OrthographicSize = curCameraFiled;
        GamePlane.transform.localScale = new Vector3(curMapScale, curMapScale, curMapScale);
    }

    //»æÖÆ gizmos
    private void OnDrawGizmos()
    {
        // Draw AircraftSpawn Area
        Gizmos.color = Color.red;
        foreach(var area in MapData.AircraftSpawnArea)
        {
            Gizmos.DrawWireCube(area.GetCenterPointInArea(), area.GetAreaSize());
        }

        // Draw StationSpawn Area
        Gizmos.color = Color.yellow;
        foreach(var area in MapData.StationSpawnArea)
        {
            Gizmos.DrawWireCube(area.GetCenterPointInArea(), area.GetAreaSize());
        }

    }
}
