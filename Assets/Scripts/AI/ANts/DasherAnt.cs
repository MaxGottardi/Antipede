using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DasherAnt : GenericAnt
{
    [Header("DashingSettings")]
    public float dashSpeed = 10;
    public float dashRoteSpeed;
    public float dashAnimSpeed;

    [HideInInspector] public float tempSpeed, tempRoteSpeed, tempAnimSpeed;

    public override void Start()
    {
        base.Start();
        tempSpeed = Speed;
        tempRoteSpeed = rotSpeed;
        tempAnimSpeed = animMultiplier;

        stateMachine.Investigate = new DasherInvestigate(this);
    }

    public override void SaveData(ref SaveableData saveableData)
    {
        if (stateMachine.currState != stateMachine.Dead)
        {
            Debug.Log("Saving Dasher Data");
            DasherAntData genericAntData = new DasherAntData();

            //general data for the ants
            genericAntData.antPosition = transform.position;
            genericAntData.antRotation = transform.rotation;

            //current time in animation and current state
            genericAntData.currAnimNormTime = anim.GetCurrentAnimatorStateInfo(0).normalizedTime % 1; //need the modulo as state info normalize time is in the form(num times played).(curr % of way through this playthrough)

            //Debug.Log(genericAntData.currAnimNormTime + "Set Normed Time");
            genericAntData.currAnimName = SaveAntStateName();

            genericAntData.callBackupWait = callBackupWait;
            genericAntData.currAIState = saveableData.AIStateToInt(stateMachine);
            genericAntData.bCanInvestigate = canInvestigate;
            genericAntData.bCallingBackup = callingBackup;
            for (int i = 0; i < spawnedHelp.Length; i++)
            {
                genericAntData.spawnedHelpOrder.list.Add((int)antType);
            }
            genericAntData.spawnedHelpCurrPos = spawnedHelpBag.currPos;
            genericAntData.bIsHelper = isHelper;
            genericAntData.damageStateOrder = damageStageChance;
            genericAntData.damageStateCurrPos = healthBag.currPos;
            genericAntData.health = health;

            stateMachine.saveData(genericAntData);

            //data specific to the dasher ants
            genericAntData.tempAnimSpeed = tempAnimSpeed;
            genericAntData.tempRotSpeed = tempRoteSpeed;
            genericAntData.tempSpeed = tempSpeed;

            genericAntData.animSpeed = animMultiplier;
            genericAntData.rotSpeed = rotSpeed;
            genericAntData.speed = Speed;

            saveableData.dasherAntData.list.Add(genericAntData);        //set the anim when load in
        }
    }
}
