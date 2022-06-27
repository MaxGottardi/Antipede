using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardAnt : GenericAnt
{
    public static ShuffleBag<GameObject> parentBag;
    [SerializeField] GameObject[] parentParts;

    GameObject heldPart;

    public override void Start()
    {
        base.Start();
        stateMachine.Attack = new GuardAttack(this);
        stateMachine.Investigate = new GuardInvestigate(this);
        stateMachine.Dead = new GuardDead(this);

        if (!isHelper) //only spawn in a part on the inital guards, non on any more which spawn in later
        {
            if (parentBag == null)
            {
                parentBag = new ShuffleBag<GameObject>();
                parentBag.shuffleList = parentParts;
            }
            SpawnPart();
        }
    }

    //spawn in parent components
    void SpawnPart()
    {
        heldPart = parentBag.getNext();
        if(heldPart != null)
        {
            heldPart = Instantiate(heldPart, headTransform);
            heldPart.transform.localRotation = Quaternion.Euler(0.4f, -90, -170);
            heldPart.transform.localScale /= 1.5f;
            heldPart.transform.localPosition = new Vector3(0, 1.3f, -0);
        }
    }

    public void DropParentSeg()
    {
        if (heldPart != null)
        {
            heldPart.transform.parent = null;
            heldPart.transform.localScale *= 2f;
            heldPart.GetComponent<ParentCollectible>().Collect();
        }
    }

    public override void SaveData(ref SaveableData saveableData)
    {
        ////also save if holding a part or not
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
