using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class MapManger : MonoBehaviour
{

    public GameObject GamePlane;
    public CinemachineVirtualCamera vCamera;

    [Header("Map Data")]
    public MapDataSO InitialMapData;
    public MapDataSO FinalMapData;

    public float curMapScale;
    [SerializeField]
    private float targetMapScale;

    public float targetScale;
    public float lerpStep;

    [SerializeField]
    public float curCameraFiled;

    public float textScale;
    public Material material;

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
        if (curMapScale < targetScale)
        {
            curMapScale += lerpStep * Time.deltaTime;
            GamePlane.transform.localScale = new Vector3(curMapScale, curMapScale, curMapScale);

            //vCamera.m_Lens.OrthographicSize = 0.8f * curCameraFiled * curMapScale;
            vCamera.m_Lens.OrthographicSize = curCameraFiled * curMapScale;
            material.SetFloat("_Scale", textScale / curMapScale);
        }
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

    private void Initialize()
    {
        InitialMapData.Initialize();

        curMapScale = InitialMapData.MapScale;
        curCameraFiled = InitialMapData.CameraOrthographicSize;
        material = GamePlane.GetComponent<MeshRenderer>().material;
        textScale = material.GetFloat("_Scale");

        vCamera.m_Lens.OrthographicSize = curCameraFiled;
        GamePlane.transform.localScale = new Vector3(curMapScale, curMapScale, curMapScale);
    }
}
