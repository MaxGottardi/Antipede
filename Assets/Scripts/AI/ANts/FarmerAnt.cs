using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class FarmerAnt : GenericAnt
{
    [Header("Larvae")]
    [SerializeField] public GameObject Larvae;
    Collider larvaeCollider;

    public bool[] useLarvae;
    public static ShuffleBag<bool> larvaeBag;

    // Start is called before the first frame update
    public override void Start()
    {
        if (larvaeBag == null)
        {
            larvaeBag = new ShuffleBag<bool>();
            larvaeBag.shuffleList = useLarvae;
        }
        ShouldSpawnLarvae();

        base.Start();
    }

    void ShouldSpawnLarvae()
    {
        if (larvaeBag.getNext() && Larvae)
        {
            AddLarvae();
        }
        else
            Larvae = null;
    }

    public void AddLarvae()
    {
        GameObject larvaeObj = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Larvae.prefab", typeof(GameObject));

        GameObject spawnedLarvae = Instantiate(larvaeObj, headTransform);
        spawnedLarvae.tag = "Untagged"; //no tag so when doing a find obj by tag these will not show up 
        spawnedLarvae.transform.localRotation = Quaternion.Euler(0.4f, -90, -170);
        spawnedLarvae.transform.localPosition = new Vector3(0, 1.3f, -0);
        Larvae = spawnedLarvae;
        larvaeCollider = Larvae.GetComponent<Collider>();
        larvaeCollider.enabled = false;
    }

    public override void ReduceHealth(int amount)
    {
        if (Larvae)
        {
            larvaeCollider.enabled = true;
            Larvae.transform.parent = null;
            Larvae.tag = "Larvae";
            Vector3 larvaePos = transform.position;
            Larvae.transform.position = larvaePos;
            Larvae.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
            Larvae = null;
        }
        base.ReduceHealth(amount);
    }

    public override void SaveData(ref SaveableData saveableData)
    {
        if (stateMachine.currState != stateMachine.Dead)
        {
            Debug.Log("Saving Dasher Data");
            FarmerAntData genericAntData = new FarmerAntData();

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

            //data specific to the farmer ants
            saveableData.useLarvaeBag = larvaeBag.shuffleList;
            saveableData.farmerAntCurrBagPos = larvaeBag.currPos;
            genericAntData.bHoldingLarvae = Larvae == null ? false : true;

            saveableData.farmerAntData.list.Add(genericAntData);
        }
    }
}
