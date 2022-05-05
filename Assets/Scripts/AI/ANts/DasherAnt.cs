using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DasherAnt : GenericAnt
{
    [Header("DashingSettings")]
    public float dashSpeed = 10;
    public float dashRoteSpeed;
    public float dashAnimSpeed;

    [HideInInspector]public float tempSpeed, tempRoteSpeed, tempAnimSpeed;

    public override void Start()
    {
        base.Start();
        stateMachine.Investigate = new DasherInvestigate(this);
    }
}
