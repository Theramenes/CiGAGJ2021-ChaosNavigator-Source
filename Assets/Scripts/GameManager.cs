using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public int gameState;
    public SpawnManager SpawnManager;
    public MapManager MapManager;

    public float gameTime;
    public float timeScaleRate;
    public int stateTimeClip;

    public float maxTime;

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
        gameTime = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        gameTime += Time.deltaTime;

        MapManager.DoMapScale(gameTime);

        //if (gameTime < maxTime)
        //    MapManager.GetComponent<MapManger>().setTargetScale(1 + timeScaleRate * ((int)gameTime) / stateTimeClip);
    }

    private void Initialize()
    {
        gameTime = 0f;


    }
}
