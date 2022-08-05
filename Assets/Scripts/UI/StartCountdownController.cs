using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartCountdownController : MonoBehaviour
{
    [Header("Countdown Config")]
    public int CountdownSec;

    public FloatReference CountdownAnimDuration;
    private float timeRemain;
    private int nextTime;
    private bool isCounting;

    public GameEvent GameStart;

    [Header("Countdown UI Config")]
    public Text timeText;

    public GameObject text;
    public GameObject CountdownPanel;

    public FloatReference lerpOffset;
    public AnimationCurve animationCurve;
    private LerpEnumerator lerpTool;

    private void Awake()
    {
        Initialize();
    }

    // Start is called before the first frame update
    void Start()
    {
        DisplayCountdown(CountdownSec);
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isCounting)
            TimeUpdate();

    }


    private void TimeUpdate()
    {
        timeRemain -= 1 * Time.deltaTime;

        if(timeRemain <= 0f)
        {
            EndCountdown();
            return;
        }

        if (Mathf.Floor(timeRemain) < nextTime)
        {
            DisplayCountdown(nextTime);
            nextTime -= 1;
        }
    }

    private void Initialize()
    {
        timeRemain = CountdownSec;
        nextTime = CountdownSec - 1;
        lerpTool = new LerpEnumerator();
        isCounting = false;
    }

    private void EndCountdown()
    {
        isCounting = false;
        GameStart.Raise();
        text.SetActive(false);
        CountdownPanel.SetActive(false);
    }

    public void ActivateStarCountdown()
    {
        text.SetActive(true);
        CountdownPanel.SetActive(true);

        isCounting = true;
    }

    public void DisplayCountdown(int time)
    {
        timeText.text =  time.ToString();

        Vector3 textTargetPos = timeText.transform.position;
        Vector3 textStartPos = textTargetPos + new Vector3(0f,200f,0f);
        timeText.transform.position = textStartPos;

        StartCoroutine(lerpTool.Vector3CurveLerp((var) => timeText.transform.position = var,
            textStartPos,textTargetPos, animationCurve,CountdownAnimDuration.Value)
            );
    }
}
