using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerManager : MonoBehaviour
{
    [Header("Time")]
    public FloatVariableSO longestTime;
    public FloatVariableSO time;

    [Header("Text UI")]
    public Text timeText;

    private float msec;
    private float sec;
    private float min;

    private void Start()
    {

    }


    public void StopTimer()
    {
        StopCoroutine("Timer");
    }

    private void UpdateTimeRecord()
    {
        longestTime.SetValue((time.Value > longestTime.Value) ? 
            time.Value : longestTime.Value); 
    }

    public void ActivateTimer()
    {

        StartCoroutine("Timer");
    }

    IEnumerator Timer()
    {
        while (true)
        {
            msec = (int)((time.Value - (int)time.Value) * 100);
            sec = (int)(time.Value % 60);
            min = (int)(time.Value / 60 % 60);

            timeText.text = string.Format("{0:00}:{1:00}:{2:00}",
                min, sec, msec);

            yield return null;
        }
    }

}
