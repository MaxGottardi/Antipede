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

    public Tween(Vector3 StartPos, Vector3 EndPos, float StartTime, float Duration)
    {
        // this.Target = Target;
        this.StartPos = StartPos;
        this.EndPos = EndPos;
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

    public Vector3 UpdatePositionEaseInBounce()
    {
        float timeFraction = 1 - (Time.time - StartTime) / Duration;
        const float n1 = 7.5625f;
        const float d1 = 2.75f;

        if (timeFraction < 1 / d1)
        {
            return Vector3.Lerp(StartPos, EndPos, 1 - (n1 * timeFraction * timeFraction));
        }
        else if (timeFraction < 2 / d1)
        {
            return Vector3.Lerp(StartPos, EndPos, 1 - (n1 * (timeFraction -= 1.5f / d1) * timeFraction + 0.75f));
        }
        else if (timeFraction < 2.5f / d1)
        {
            return Vector3.Lerp(StartPos, EndPos, 1 - (n1 * (timeFraction -= 2.25f / d1) * timeFraction + 0.9375f));
        }
        else
        {
            return Vector3.Lerp(StartPos, EndPos, 1 - (n1 * (timeFraction -= 2.625f / d1) * timeFraction + 0.984375f));
        }
    }
}
