using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public int gameState;
    
    [Header("Core Game Components")]
    public SpawnManager SpawnManager;
    public MapManager MapManager;
    public BlackHoleController BHController;
    public StartCountdownController countdownController;

    public FloatVariableSO gameTime;
    public float timeScaleRate;
    public int stateTimeClip;

    public float maxTime;

    private bool isGameStart;

    [Header("Events")]
    public GameEvent GameEnd;

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
        countdownController.ActivateStarCountdown();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isGameStart)
            return;

        gameTime.ApplyChange(Time.deltaTime);
        MapManager.DoMapScale(gameTime.Value);
        CheckEndGame();

        //if (gameTime < maxTime)
        //    MapManager.GetComponent<MapManger>().setTargetScale(1 + timeScaleRate * ((int)gameTime) / stateTimeClip);
    }

    private void CheckEndGame()
    {
        if (BHController.IsGameEndCondition(MapManager.GetMapEdgeLength()))
            GameEnd.Raise();
    }

    public void StartGameSession()
    {
        isGameStart = true;
        BHController.ActivateBHController();
        SpawnManager.ActivateSpawnManager();
    }

    private void Initialize()
    {
        gameTime.SetDefaultValue();
        isGameStart = false;
    }
}
