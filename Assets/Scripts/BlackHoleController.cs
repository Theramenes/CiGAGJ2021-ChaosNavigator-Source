using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Model:"Blackhole"(https://skfb.ly/PCYW) by rubykamen is licensed under Creative Commons Attribution (http://creativecommons.org/licenses/by/4.0/).


public class BlackHoleController : MonoBehaviour
{
    // Start is called before the first frame update
    private MapManager MapManager;

    //init position
    public Vector3 spawnPosition = new Vector3(0f, 0f, 0f);
    public FloatReference crashRadius;
    public FloatReference gravityMaxAcceleration, gravityRadius;
    public FloatReference controlFilter;
    public FloatReference gravAcceIncresePerShip;
    public FloatReference crashRadiusRate;
    public FloatReference gravityRadiusRate;

    public float maxSpeed;
    public GameObject innerRing;
    public GameObject outerRing;

    public GameObject blackHoleVisual;

    public IntReference absorbNum;

    //which player? which input?
    public int playerNum = 1;

    // ??Ϸ?Ƿ???ʼ
    private bool isGameStart;

    private void Awake()
    {
        Initialize();   
    }

    void Start()
    {
        gameObject.GetComponent<SphereCollider>().radius = gravityRadius;
        this.transform.position = spawnPosition;

        //innerRing.transform.localScale = new Vector3(crashRadius, crashRadius, crashRadius);
        //outerRing.transform.localScale = new Vector3(gravityRadius, gravityRadius, gravityRadius);
        absorbNum.Variable.SetDefaultValue();
        float innerRingRadius = GetCrashRadius();
        innerRing.transform.localScale = new Vector3(innerRingRadius,
            innerRingRadius,
            innerRingRadius);
        float outerRingRadius = GetDistanceOfAccel(0.2f);
        outerRing.transform.localScale = new Vector3(outerRingRadius,
            outerRingRadius,
            outerRingRadius);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isGameStart)
            return;

        maxSpeed = 5 * GetCrashRadius();

        if (Input.GetKey(KeyCode.LeftShift))
            maxSpeed = 2 * maxSpeed;

        //Motion Handler. Only moves on axis X,Z
        float translationHorizontal = Input.GetAxis("Player" + playerNum + "Horizontal") * maxSpeed * Time.deltaTime; //Z
        float translationVertical = Input.GetAxis("Player" + playerNum + "Vertical") * maxSpeed * Time.deltaTime; //X

        //speed filter
        //if (Mathf.Abs(translationHorizontal) < controlFilter && Mathf.Abs(translationVertical) < controlFilter)

        Vector3 posDelta = new Vector3(translationHorizontal, 0f, translationVertical);
        Vector3 pos = this.transform.position;
        //Vector3 transformPosDelta = Camera.main.transform.TransformDirection(posDelta);
        Vector3 transformPosDelta = Quaternion.Euler(0,Camera.main.transform.eulerAngles.y,0) * posDelta;
        transformPosDelta.y = 0.0f;


        //Vector3 posDestination = pos + posDelta;
        Vector3 posDestination = pos + transformPosDelta;

        Vector4 edge = GameObject.Find("MapManager").GetComponent<MapManager>().getEdge();
        float mapEdge = GameObject.Find("MapManager").GetComponent<MapManager>().MapData.MapEdgeLength;
        

        float deltaX = transformPosDelta.x;
        float deltaZ = transformPosDelta.z;
        
        if(!MapManager.IsBlackHoleInMoveArea(posDestination))
        {
            if (posDestination.x + GetCrashRadius() >= mapEdge && deltaX > 0)
                deltaX = 0f;
            if (posDestination.x - GetCrashRadius() <= -mapEdge && deltaX < 0)
                deltaX = 0f;
            if (posDestination.z + GetCrashRadius() >= mapEdge && deltaZ > 0)
                deltaZ = 0f;
            if (posDestination.z - GetCrashRadius() <= -mapEdge && deltaZ < 0)
                deltaZ = 0f;
        }
        //if (posDestination.x + GetCrashRadius() >= edge.w && deltaX > 0)
        //{
        //    deltaX = 0f;
        //}
        //if (posDestination.x - GetCrashRadius() <= edge.z && deltaX < 0)
        //{
        //    deltaX = 0f;
        //}
        //if (posDestination.z + GetCrashRadius() >= edge.x && deltaZ > 0)
        //{
        //    deltaZ = 0f;

        //}
        //if (posDestination.z - GetCrashRadius() <= edge.y && deltaZ < 0)
        //{
        //    deltaZ = 0f;
        //}

        this.transform.position = pos + new Vector3(deltaX, 0f, deltaZ);



        float innerRingRadius = GetCrashRadius();
        innerRing.transform.localScale = new Vector3(innerRingRadius,
            innerRingRadius,
            innerRingRadius);
        //outerRing.transform.localScale = new Vector3(gravityRadius + gravityRadiusRate * absorbNum * gravAcceIncresePerShip,
        //    gravityRadius + gravityRadiusRate * absorbNum * gravAcceIncresePerShip,
        //    gravityRadius + gravityRadiusRate * absorbNum * gravAcceIncresePerShip);
        float outerRingRadius = GetDistanceOfAccel(0.2f);
        outerRing.transform.localScale = new Vector3(outerRingRadius,
            outerRingRadius,
            outerRingRadius);
    }

    private void FixedUpdate()
    {
        //Cast Gravity by collider
        Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, GetDistanceOfAccel(0.2f));
        foreach (var hitCollider in hitColliders)
        {
            Vector3 objPos = hitCollider.gameObject.transform.position;
            Vector3 directionVector = this.transform.position - objPos;
            float distance = directionVector.magnitude;
            if (distance < GetCrashRadius())
            {
                //ײ???ڶ?
                hitCollider.SendMessage("OnCollideWithBlackHole", this, SendMessageOptions.DontRequireReceiver);
            }
            else
            {
                //Ӧ?ü??ٶ?
                directionVector = directionVector / distance;
                float accel = GetAccelOfDistance(distance);

                hitCollider.SendMessage("ApplyAcceleration", directionVector * accel, SendMessageOptions.DontRequireReceiver);
            }

        }
    }

    private float GetGravityCorrectionFactor(float distance)
    {
        //???????ٶ?Ҫ????????ϵ??
        if (distance > gravityRadius) return 0;
        return 1;
        //return (gravityRadius - distance) / gravityRadius;
    }

    public void absorbShip(int weight)
    {
        absorbNum.Variable.ApplyChange(weight);



        float innerRingRadius = GetCrashRadius();
        innerRing.transform.localScale = new Vector3(innerRingRadius,
            innerRingRadius,
            innerRingRadius);

        float blackHoleSize = 1.5f;
        blackHoleVisual.transform.localPosition = new Vector3(0, 0.5f * innerRingRadius, 0);
        for (int i = 0; i < blackHoleVisual.transform.childCount; i++)
        {
            blackHoleVisual.transform.GetChild(i).transform.localScale = new Vector3(innerRingRadius* blackHoleSize,
                innerRingRadius * blackHoleSize,
                innerRingRadius * blackHoleSize);
        }

        //outerRing.transform.localScale = new Vector3(gravityRadius + gravityRadiusRate * absorbNum * gravAcceIncresePerShip,
        //    gravityRadius + gravityRadiusRate * absorbNum * gravAcceIncresePerShip,
        //    gravityRadius + gravityRadiusRate * absorbNum * gravAcceIncresePerShip);
        float outerRingRadius = GetDistanceOfAccel(0.2f);
        outerRing.transform.localScale = new Vector3(outerRingRadius,
            outerRingRadius,
            outerRingRadius);
    }

    private float GetDistanceOfAccel(float accel)
    {
        return (gravityMaxAcceleration.Value + absorbNum * gravAcceIncresePerShip - accel) / 4.9f;
    }

    private float GetAccelOfDistance(float distance)
    {
        float ret = gravityMaxAcceleration.Value + absorbNum * gravAcceIncresePerShip - 4.9f * distance;
        return (ret > 0f ? ret : 0f);
    }

    private float GetCrashRadius()
    {
        return crashRadius + crashRadiusRate * absorbNum * gravAcceIncresePerShip;
    }

    public bool IsGameEndCondition(float mapEdgeLength)
    {
        return 1.5f * mapEdgeLength < outerRing.transform.localScale.x ? true : false;
    }

    public void ActivateBHController()
    {
        isGameStart = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, crashRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, gravityRadius);
    }

    private void Initialize()
    {
        MapManager = GameObject.Find("MapManager").GetComponent<MapManager>();
        isGameStart = false;
    }

    //private

}
