using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpEnumerator 
{

    public IEnumerator Vector3CurveLerp(System.Action<Vector3> variable,Vector3 origin, Vector3 targetVec, AnimationCurve curve, float duration)
    {
        float journey = 0f;

        while (journey <= duration)
        {
            journey = journey + Time.deltaTime;

            float percent = Mathf.Clamp01(journey / duration);
            float curvePercent = curve.Evaluate(percent);

            variable(Vector3.LerpUnclamped(origin, targetVec, curvePercent));
            //transform.position = Vector3.Lerp(origin, target, percent);

            yield return null;
        }
    }

    public IEnumerator FloatCurveLerp(System.Action<float> variable, float origin, float targetFloat, AnimationCurve curve, float duration)
    {
        float journey = 0f;

        while (journey <= duration)
        {
            journey = journey + Time.deltaTime;

            float percent = Mathf.Clamp01(journey / duration);
            float curvePercent = curve.Evaluate(percent);

            variable(Mathf.LerpUnclamped(origin, targetFloat, curvePercent));
            //transform.position = Vector3.Lerp(origin, targetFloat, percent);

            yield return null;
        }
    }

}

