using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tween
{
    //  public Transform Target { get; private set; }
    public Vector3 StartPos { get; private set; }
    public Vector3 EndPos { get; private set; }
    public Quaternion StartRot { get; private set; }
    public Quaternion EndRot { get; private set; }
    public float StartTime { get; private set; }
    public float Duration { get; private set; }

    public Tween(Vector3 StartPos, Vector3 EndPos, Quaternion StartRot, Quaternion EndRot, float StartTime, float Duration)
    {
        // this.Target = Target;
        this.StartPos = StartPos;
        this.EndPos = EndPos;
        this.StartRot = StartRot;
        this.EndRot = EndRot;
        this.StartTime = StartTime;
        this.Duration = Duration;
    }

    public Vector3 UpdatePosition()
    {
        float timeFraction = (Time.time - StartTime) / Duration;
       // Debug.Log(timeFraction);
        return Vector3.Lerp(StartPos, EndPos, timeFraction);
    }
    public Quaternion UpdateRotation()
    {
        float timeFraction = (Time.time - StartTime) / Duration;
        return Quaternion.Lerp(StartRot, EndRot, timeFraction);
    }

    public Vector3 UpdatePositionEaseOutCirc()
    {
        float timeFraction = (Time.time - StartTime) / Duration;
        timeFraction = Mathf.Clamp(timeFraction, 0.0f, 1.0f);
        timeFraction = Mathf.Sqrt(1 - timeFraction * timeFraction);
        Debug.Log(timeFraction);
        return Vector3.Lerp(StartPos, EndPos, 1 - timeFraction);
    }
    public Vector3 UpdatePositionEaseInCubic()
    {
        float timeFraction = (Time.time - StartTime) / Duration;
        timeFraction = Mathf.Clamp(timeFraction, 0.0f, 1.0f);
        timeFraction = 3 * Mathf.Pow(timeFraction, 3);
        Debug.Log(timeFraction);
        return Vector3.Lerp(StartPos, EndPos, timeFraction);
        //5f * NATURAL_LOG_OF_2 * end * Mathf.Pow(2f, 1f - 10f * value);
    }

    public Vector3 UpdatePositionEaseOutExp()
    {
        float timeFraction = (Time.time - StartTime) / Duration;
        timeFraction = Mathf.Clamp(timeFraction, 0.0f, 1.0f);
        timeFraction = (-Mathf.Pow(2, -10 * timeFraction) + 1);
        return Vector3.Lerp(StartPos, EndPos, timeFraction);
        //5f * NATURAL_LOG_OF_2 * end * Mathf.Pow(2f, 1f - 10f * value);
    }
}
