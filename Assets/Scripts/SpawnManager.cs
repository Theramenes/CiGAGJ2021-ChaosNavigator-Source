using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//enum flyObjectType = { "",}

public class SpawnManager : MonoBehaviour
{
    private MapManager MapManager;


    //生成事件列表
    public SpawnEventSetSO spawnEventList;
    //0:ship
    //1:target
    //2:meteor


    private string[] typeStrings = { "Ship", "Target", "Meteor" };

    public int[] totalNum = { 0, 0, 0 };
    private float lastSpawnInterval = 0f;

    [Header("Spawn Config")]
    public SpawnConfigSO SpawnConfig;

    public FloatReference initSpawnInterval;
    [SerializeField]
    private FloatVariableSO gameTimer;

    public IntVariableSO currentScore;
    public IntReference maxTarget;


    [Header("Spawn Objects Data")]
    public SpawnObjectsDataSO SpawnObjectsData;
    //[SerializeField]
    //[SerializeField]

    // 用于Gizmos 记录生成点和方向
    [Header("Gizmos Draw Data")]
    [SerializeField]
    private int maxQueueSize = 10;

    [SerializeField]
    private Queue<Vector3> AircraftSpawnPoint = new Queue<Vector3>();
    [SerializeField]
    private Queue<Vector3> AircraftDirection = new Queue<Vector3>();

    private IEnumerator<Vector3> spawnPointEnumerator;
    private IEnumerator<Vector3> directionEnumerator;


    // 是否开始游戏
    private bool isGameStarted = false;

    private void Awake()
    {
        Initialize();
    }

    private void Update()
    {

        if (!isGameStarted)
            return;

        totalNum[0] = GameObject.Find("---<< AirCrafts >>---").transform.childCount;
        totalNum[1] = GameObject.Find("---<< Stations >>---").transform.childCount;

        lastSpawnInterval += Time.deltaTime;

        if (SpawnConfig.currentSpawnStage + 1 != SpawnConfig.SpawnStageTime.Length)
            UpdateSpawnStageIndex();

        if (lastSpawnInterval > SpawnConfig.GetSpawnInterval() || IsSpawnCondition())
        {
            Debug.Log("Spawn Triggered");
            Spawn();
            lastSpawnInterval = 0f;
        }
    }

    /*重置游戏计时器*/
    //public void InitGameTimer()
    //{
    //    gameTimer = 0f;
    //}

    //碰撞管理
    public void HandleCrash(int objType)
    {
        //得知当前碰撞事件
        if (objType > 0)
        {
            //维护场上生成物数量统计
            totalNum[objType]--;
        }
        //TODO:
        //分数维护

    }

    private void UpdateSpawnStageIndex()
    {
        if (gameTimer.Value > SpawnConfig.SpawnStageTime[SpawnConfig.currentSpawnStage])
            SpawnConfig.currentSpawnStage += 1;

    }

    //private float GetSpawnInterval()
    //{
        
    //    //float[] labels = { 10f, 22f, 34f, 60f };
    //    //float[] intervals = { 5f, 3f, 3f, 2f, 1f,1f };
    //    //int stageIndex = 0;
    //    //for (int i = 0; i < labels.Length; i++)
    //    //{
    //    //    if (gameTimer.Value > labels[i]) stageIndex++;
    //    //    else break;
    //    //}
    //    return intervals[SpawnConfig.currentSpawnStage];
    //}
    private int GetStageIndex()
    {
        float[] labels = { 10f, 22f, 34f, 60f };
        int stageIndex = 0;
        for (int i = 0; i < labels.Length; i++)
        {
            if (gameTimer.Value > labels[i]) stageIndex++;
            else break;
        }
        return stageIndex;
    }

    private bool IsSpawnCondition()
    {
        //此时场上的条件是否满足提前生成？
        if (totalNum[0] == 0 || totalNum[1] == 0)
        {
            return true;
        }
        return false;
    }


    private int GetSpecialSpawn()
    {
        if (totalNum[0] == 0)
        {
            return 1;
        }
        else if (totalNum[1] == 0)
        {
            return 0;
        }
        return -1;
    }

    public List<T> Shuffle<T>(List<T> original)
    {
        System.Random randomNum = new System.Random();
        int index = 0;
        T temp;
        for (int i = 0; i < original.Count; i++)
        {
            index = randomNum.Next(0, original.Count - 1);
            if (index != i)
            {
                temp = original[i];
                original[i] = original[index];
                original[index] = temp;
            }
        }
        return original;
    }

    private void Spawn()
    {
        //抽取一个随机的生成事件进行生成
        int sp = GetSpecialSpawn();
        if (sp != -1)
        {
            Spawn(sp);
            return;
        }

        int totalEvents = 8;
        int[,] cardSet =
        {
            {1,1,1,0,0,0,0,0 },
            {2,2,2,1,1,0,0,0 },
            {3,2,2,1,1,1,1,0 },
            {4,2,2,1,1,1,1,1 },
            {4,1,1,1,2,1,1,3 }
        };
        List<int> cardColle = new List<int>();
        for (int i = 0; i < totalEvents; i++)
        {
            for (int j = cardSet[SpawnConfig.currentSpawnStage, i]; j > 0; j--)
                cardColle.Add(i);
        }
        cardColle = Shuffle<int>(cardColle);
        int index = cardColle[0];

        //int index = Random.Range(0, spawnEventList.Items.Count);
        Spawn(index);
    }
    private void Spawn(int index)
    {
        Debug.Log("Spawn Event " + index);
        //进行index号事件的随机生成
        SpawnEventSO spawnEvent = spawnEventList.Items[index];


        GameObject bH = GameObject.Find("BlackHole");

        Vector3 blackHolePos = bH.transform.position;
        Vector3 spawnPosition;
        int spawnArea;

        if (spawnEvent.shipType == 1 && (SpawnObjectsData.StationCount < maxTarget))
        {
            int bhArea = MapManager.GetPointCurrentArea(blackHolePos);
            spawnArea = Random.Range((bhArea + 1) % 4, (bhArea + 3) % 4);
            spawnPosition = MapManager.GetStationSpawnPoint(spawnArea);

            SpawnStaticObject(spawnEvent.shipType, spawnPosition);
            return;
        }

        int shipGenNum = Random.Range(Mathf.Max(spawnEvent.shipNum - spawnEvent.offset, 0), spawnEvent.shipNum + spawnEvent.offset);
        Vector3 spawnDirectionEuler = new Vector3(0f, Random.Range(0f, 2 * 3.14f), 0f);

        // 飞船生成点
        float mapScale = MapManager.MapData.MapScale;
        //Vector3 spawnPosition = new Vector3(Random.Range(-4f * mapScale, 4f* mapScale), 0f, Random.Range(-4f* mapScale, 4f* mapScale));
        int airCraftSpawnArea = Random.Range(0, 4);
        spawnPosition = MapManager.GetAircraftSpawnPoint(airCraftSpawnArea);

        Debug.Log("Spawn Info : Spawning aircrafts in " + spawnPosition + " in area " + airCraftSpawnArea + "\n");

        // 飞船初始方向生成
        int directArea = Random.Range((airCraftSpawnArea + 1) % 4, (airCraftSpawnArea + 3) % 4);

        Debug.Log("Spawn Info : Spawning aircrafts directing area " + directArea + "\n");

        Vector3 targetPoint = MapManager.GetAircraftSpawnPoint(directArea);
        Vector3 direction = (targetPoint - spawnPosition).normalized;

        Debug.Log("Spawn Info : Spawning aircrafts direction " + direction + "\n");

        //规避黑洞生成
        Vector3 awayFromHole = spawnPosition - blackHolePos;
        awayFromHole = awayFromHole * Mathf.Max(0f, 1.5f * bH.GetComponent<BlackHoleController>().crashRadius - awayFromHole.magnitude);
        spawnPosition += awayFromHole;

        for (int i = 0; i < shipGenNum; i++)
        {

            Vector3 finalSpawnDirectionEuler = spawnDirectionEuler + spawnEvent.initDirections[i].Value;
            //default direction
            Vector3 finalSpawnPosition = spawnPosition + RotateRound(spawnEvent.initPos[i].Value, new Vector3(0f, 0f, 0f), new Vector3(0f, 1f, 0f), spawnDirectionEuler[1]);

            SpawnObject(spawnEvent.shipType.Value, spawnEvent.speed.Value, finalSpawnPosition, direction);
            //SpawnObject(spawnEvent.shipType.Value, spawnEvent.speed.Value, finalSpawnPosition, finalSpawnDirectionEuler);
        }
    }

    public static Vector3 RotateRound(Vector3 position, Vector3 center, Vector3 axis, float angle)
    {
        Vector3 point = Quaternion.AngleAxis(angle, axis) * (position - center);
        Vector3 resultVec3 = center + point;
        return resultVec3;
    }

    public void SpawnStaticObject(int type, Vector3 pos)
    {
        GameObject stationObj = Instantiate(Resources.Load<GameObject>("StationPrefab"));
        stationObj.transform.parent = GameObject.Find("---<< Stations >>---").transform;
        stationObj.transform.position = pos;

        SpaceStation newStation = stationObj.GetComponent<SpaceStation>();
        newStation.currentScore = currentScore;
        newStation.capacity = 5;

        UpdateSpawnObjectData(type, 1);
    }

    private void SpawnObject(int type, float speed, Vector3 pos, Vector3 direction)
    {
        //生成单个飞行物
        if (type < 0 || type >= typeStrings.Length) return;

        if (type == 0)
        {
            GameObject shipObj = Instantiate(Resources.Load<GameObject>("SpaceshipPrefab"));
            shipObj.transform.parent = GameObject.Find("---<< AirCrafts >>---").transform;
            shipObj.transform.position = pos;

            Aircraft newAircraft = shipObj.GetComponent<Aircraft>();
            //newAircraft.direction = new Vector3(Mathf.Cos(direction[1]), 0, Mathf.Sin(direction[1]));
            newAircraft.direction = direction;
            newAircraft.origenalSpeed = (Random.Range(-0.2f, 0.2f) + 1) + speed;  //random speed

            //newAircraft.top = GameObject.Find("Top").transform;
            //newAircraft.down = GameObject.Find("Down").transform;
            //newAircraft.left = GameObject.Find("Left").transform;
            //newAircraft.right = GameObject.Find("Right").transform;

            newAircraft.currentScore = currentScore;

            newAircraft.type = "Ship";
            shipObj.tag = "Ship";

            AddSpawnDataToQueue(pos, direction);
        }
        //else if (type == 1)
        //{
        //    if (totalNum[1] < maxTarget)
        //    {
        //        GameObject stationObj = Instantiate(Resources.Load<GameObject>("StationPrefab"));
        //        stationObj.transform.parent = GameObject.Find("---<< Stations >>---").transform;
        //        stationObj.transform.position = pos;

        //        SpaceStation newStation = stationObj.GetComponent<SpaceStation>();
        //        newStation.currentScore = currentScore;
        //        newStation.capacity = 5;
        //    }
        //}
        else if (type == 2)
        {
            GameObject meteorObj = Instantiate(Resources.Load<GameObject>("MeteorPrefab"));
            meteorObj.transform.parent = GameObject.Find("---<< AirCrafts >>---").transform;
            meteorObj.transform.position = pos;

            Aircraft newAircraft = meteorObj.GetComponent<Aircraft>();
            newAircraft.direction = new Vector3(Mathf.Cos(direction[1]), 0, Mathf.Sin(direction[1]));
            newAircraft.origenalSpeed = (Random.Range(-0.1f, 0.1f) + 1) + speed;  //random speed

            //newAircraft.top = GameObject.Find("Top").transform;
            //newAircraft.down = GameObject.Find("Down").transform;
            //newAircraft.left = GameObject.Find("Left").transform;
            //newAircraft.right = GameObject.Find("Right").transform;

            newAircraft.currentScore = currentScore;

            newAircraft.type = "Meteor";
            meteorObj.tag = "Meteor";
        }

        UpdateSpawnObjectData(type, 1);

        Debug.Log("Generated type "+ typeStrings[type]);
        //totalNum[type]++;
    }

    public void UpdateSpawnObjectData(int type, int offset)
    {
        switch (type)
        {
            case 0:
                SpawnObjectsData.AircraftCount += offset;
                break;
            case 1:
                SpawnObjectsData.StationCount += offset;
                SpawnObjectsData.TotalCapacity += 5*offset;
                break;
            case 2:
                SpawnObjectsData.MetoerCount += offset;
                break;
            default:
                break;
        }

    }


    private void AddSpawnDataToQueue(Vector3 spawnPoint, Vector3 direction)
    {
        if(AircraftSpawnPoint.Count < maxQueueSize)
        {
            AircraftSpawnPoint.Enqueue(spawnPoint);
            AircraftDirection.Enqueue(direction);
            return;
        }

        AircraftSpawnPoint.Dequeue();
        AircraftSpawnPoint.Enqueue(spawnPoint);
        AircraftDirection.Dequeue();
        AircraftDirection.Enqueue(direction);
    }

    public void ActivateSpawnManager()
    {
        isGameStarted = true;

    }

    private void Initialize()
    {
        MapManager = GameObject.Find("MapManager").GetComponent<MapManager>();
        SpawnObjectsData.Initialize();
        SpawnConfig.Initialize();
        isGameStarted = false;
        //InitGameTimer();
    }

    public void StationCountDecrease()
    {
        SpawnObjectsData.StationCount --;
    }

    private void OnDrawGizmos()
    {
        if (AircraftDirection.Count == 0)
            return;

        spawnPointEnumerator = AircraftSpawnPoint.GetEnumerator();
        directionEnumerator = AircraftDirection.GetEnumerator();

        spawnPointEnumerator.Reset();
        directionEnumerator.Reset();

        while (spawnPointEnumerator.MoveNext() && directionEnumerator.MoveNext())
        {
            // spawn point
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(spawnPointEnumerator.Current, 0.1f);

            // direction
            Gizmos.color = Color.green;
            Gizmos.DrawRay(spawnPointEnumerator.Current, directionEnumerator.Current);
        }
    }
}
