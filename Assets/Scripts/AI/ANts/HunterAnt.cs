using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>The class for the hunter ant.</summary>
public class HunterAnt : GenericAnt
{
    //implement the shuffle list stuff, makaing it static so all can use it
    public Vector3 weaponPos;
    public Transform weaponParent;

    public static ShuffleBag<GameObject> weaponsBag;
    public GameObject[] weapons;

    public Weapon weaponClass;//the weapon attached to this ant

    [SerializeField] public GameObject shieldCardPrefab;
    [SerializeField] public GameObject launcherCardPrefab;
    [SerializeField] public GameObject laserCardPrefab;
    [SerializeField] public GameObject gunCardPrefab;

    public bool isFleeing = false; //is this ant currently moving away from the player or not
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        stateMachine.Attack = new HunterAttack(this);
        stateMachine.Dead = new HunterDead(this);
        stateMachine.Investigate = new HunterInvestigate(this);
        if (weaponsBag == null)
        {
            weaponsBag = new ShuffleBag<GameObject>();
            weaponsBag.shuffleList = weapons;
        }
        if (weaponClass != null)
            Destroy(weaponClass.gameObject);
        PickWeapon(null);
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }

    public void DropWeapon()
    {
        //old code
        //Instantiate(weaponClass.weaponPickup, transform.position, Quaternion.identity);

        //Changed by David.D
        Vector3 spawnPos = new Vector3(transform.position.x, 1.0f, transform.position.z);

        if (weaponClass.weaponPickup.name == "Shield Pickup")
        {
            Instantiate(shieldCardPrefab, spawnPos, Quaternion.identity);
        }
        if (weaponClass.weaponPickup.name == "Launcher Pickup")
        {
            Instantiate(launcherCardPrefab, spawnPos, Quaternion.identity);
        }
        if (weaponClass.weaponPickup.name == "Laser Pickup")
        {
            Instantiate(laserCardPrefab, spawnPos, Quaternion.identity);
        }
        if (weaponClass.weaponPickup.name == "Gun Pickup")
        {
            Instantiate(gunCardPrefab, spawnPos, Quaternion.identity);
        }
        //Instantiate(shieldCardPrefab, spawnPos, Quaternion.identity);
        Debug.Log(weaponClass.weaponPickup);
    }

    public void PickWeapon(GameObject spawnWeapon)
    {
        GameObject weapon;
        if (spawnWeapon == null)
            weapon = Instantiate(weaponsBag.getNext(), weaponParent, false);
        else
        {
            weapon = Instantiate(spawnWeapon, weaponParent, false);
        }
        weapon.transform.localPosition = weaponPos;
        weapon.transform.localScale *= 2;
        weapon.transform.localRotation = new Quaternion(-0.474147886f, -0.524579644f, -0.474147886f, 0.524579704f);

        weaponClass = weapon.transform.GetComponent<Weapon>();
        weaponClass.isAntGun = true;
        ////weaponClass.LookAt(transform.forward);
    }

    public override void SaveData(SaveableData saveableData)
    {
        if (stateMachine.currState != stateMachine.Dead)
        {
            HunterAntData genericAntData = new HunterAntData();

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

            //data specific to the hunter ants
            genericAntData.heldWeapon = saveableData.WeaponToInt(weaponClass);
            saveableData.hunterAntWeaponBag = new int[weaponsBag.shuffleList.Length];
            for(int i = 0; i < weaponsBag.shuffleList.Length; i++)
            {
                saveableData.hunterAntWeaponBag[i] = saveableData.WeaponToInt(weaponsBag.shuffleList[i].GetComponent<Weapon>());
            }
            saveableData.hunterAntCurrBagPos = weaponsBag.currPos;


            saveableData.hunterAntData.list.Add(genericAntData);
        }
    }
}
