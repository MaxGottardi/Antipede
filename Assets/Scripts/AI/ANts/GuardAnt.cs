using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardAnt : GenericAnt
{
    public override void Start()
    {
        base.Start();
        stateMachine.Attack = new GuardAttack(this);
        stateMachine.Investigate = new GuardInvestigate(this);
    }

    public override void SaveData(ref SaveableData saveableData)
    {
        if (stateMachine.currState != stateMachine.Dead)
        {
            GenericAntData genericAntData = new GenericAntData();

            //general data for the ants
            genericAntData.antPosition = transform.position;
            genericAntData.antRotation = transform.rotation;

            //current time in animation and current state
            genericAntData.currAnimNormTime = anim.GetCurrentAnimatorStateInfo(0).normalizedTime % 1; //need the modulo as state info normalize time is in the form(num times played).(curr % of way through this playthrough)

            Debug.Log(genericAntData.currAnimNormTime + "Set Normed Time");
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

            saveableData.guardAntData.list.Add(genericAntData);
        }
    }
}
