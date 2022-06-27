using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//enables the saving of a list to a json file for later access
[System.Serializable]
public class SerializableList<T>
{
    public List<T> list = new List<T>();
}

/// <summary>
/// convert and unconvert the dictionary to a format which can be saved to json
/// </summary>
/// <typeparam name="TKey">the key of the dictionary</typeparam>
/// <typeparam name="TValue">the value of the dictionary</typeparam>
[System.Serializable]
public class SerializableDictionary<TKey, TValue>: ISerializationCallbackReceiver
{
    public Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();

    [SerializeField] SerializableList<TKey> keys = new SerializableList<TKey>();
    [SerializeField] SerializableList<TValue> values = new SerializableList<TValue>();
    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {
        dictionary.Clear();
        for(int i = 0; i < keys.list.Count; i++)
        {
            dictionary.Add(keys.list[i], values.list[i]);
        }
    }

    void ISerializationCallbackReceiver.OnBeforeSerialize()
    {
        keys.list.Clear();
        values.list.Clear();
        foreach (KeyValuePair<TKey, TValue> item in dictionary)
        {
            keys.list.Add(item.Key);
            values.list.Add(item.Value);
        }
    }
}
public enum EWeaponType
{
    empty = -1,
    shield,
    laser,
    launcher,
    gun
}
[System.Serializable]
public class SpiderData
{
    [SerializeField] public float spiderCurAnimTime; //also saving of current spider animation state
    [SerializeField] public string spiderCurAnimClip; //also saving of current spider animation state
    [SerializeField] public float spiderHealth, spiderDeathTimer, spiderAttackTimer, spiderShootTimer, spiderShootAnimTimer, spiderSpawnAntTimer;
    [SerializeField] public bool spiderDying, spiderMoving, spiderAttackPlayer, spiderShooting;
    [SerializeField] public Vector3 spiderPosition;
    [SerializeField] public Quaternion spiderRotation;
}
[System.Serializable]
public class WebData
{
    [SerializeField] public bool bWebIsShot;
    [SerializeField] public float webDespawnTimer;
    [SerializeField] public Vector3 webPosition, webVelocity;
    [SerializeField] public Quaternion webRotation;

}
[System.Serializable]
public class GenericAntData
{
    //basically it works, asside from the animations not being correct or proper

    /// <summary>
    /// issues noted:
    /// antenna rotations not the best when loading in and the attack animation not set correctly
    /// </summary>
    [SerializeField] public Vector3 antPosition;
    [SerializeField] public Quaternion antRotation;
   
    [SerializeField]
    public float health, callBackupWait, shockTimeLeft, shockAnimTimeNormalized,
        investigateStateLostPlayerTime, investigateCallBackupRingStartTime, investigateCallBackupRunTime,
        currAttackTime, currDamageTime, currDeadTime, spawnInWaitTime,
        currAnimNormTime;
    [SerializeField] public string currAnimName;
    [SerializeField] public int currAIState;
    [SerializeField] public bool bCanInvestigate, bCallingBackup, bIsHelper, bInvestigateCallBackupPlayedAudio,
        bAttackDone;

    //the settings for the shockbar
    [SerializeField] public bool bShockBarActiveState;
    [SerializeField] public float shockBarCurrAntimNormTime;
    /// <summary>
    /// //all the variables for the AI states from the behaviour trees need to be saved to lists otherwise will break if two of the same class is saved
    /// </summary>

    [SerializeField] public SerializableList<int> spawnedHelpOrder = new SerializableList<int>();
    [SerializeField] public SerializableList<int> aISelectorChildCounts = new SerializableList<int>(); //in theory if saving and loading from scripts in exactly the same order it should work fine
    [SerializeField] public SerializableList<int> aISequenceChildCounts = new SerializableList<int>(); //in theory if saving and loading from scripts in exactly the same order it should work fine
    [SerializeField] public SerializableList<float> moveTowardsPrevDistToNode = new SerializableList<float>(); //in theory if saving and loading from scripts in exactly the same order it should work fine
    [SerializeField] public SerializableList<Quaternion> moveTowardsForwardOffset = new SerializableList<Quaternion>(); //in theory if saving and loading from scripts in exactly the same order it should work fine

    [SerializeField] public SerializableList<float> moveBackwardsOffset = new SerializableList<float>(); //in theory if saving and loading from scripts in exactly the same order it should work fine
    [SerializeField] public SerializableList<float> moveBackwardsTime = new SerializableList<float>(); //in theory if saving and loading from scripts in exactly the same order it should work fine

    [SerializeField] public SerializableList<float> callBackupRunTime = new SerializableList<float>(); //in theory if saving and loading from scripts in exactly the same order it should work fine
    [SerializeField] public SerializableList<float> callBackupTweenStartTime = new SerializableList<float>(); //in theory if saving and loading from scripts in exactly the same order it should work fine
    [SerializeField] public SerializableList<bool> callBackupPlayedAudio = new SerializableList<bool>(); //in theory if saving and loading from scripts in exactly the same order it should work fine
    [SerializeField] public SerializableList<bool> bIsNodeRunning = new SerializableList<bool>(); //in theory if saving and loading from scripts in exactly the same order it should work fine


    [SerializeField] public int spawnedHelpCurrPos, damageStateCurrPos;
    [SerializeField] public bool[] damageStateOrder;
}
[System.Serializable]
public class HunterAntData : GenericAntData
{
    [SerializeField] public float currShootDelay;
    //weapon held
    [SerializeField] public int heldWeapon;
}
[System.Serializable]
public class FarmerAntData : GenericAntData
{
    [SerializeField] public bool bHoldingLarvae;
}

[System.Serializable]
public class DasherAntData : GenericAntData
{
    [SerializeField] public float tempSpeed, tempRotSpeed, tempAnimSpeed,
        speed, rotSpeed, animSpeed;
}
[System.Serializable]
public class BombAntData : GenericAntData
{
    ////[SerializeField] public bool bHoldingLarvae;
}

/// <summary>
/// within this class, hold values containing all the data(or copies of it) of the game which are required to be saved
/// </summary>
[System.Serializable]
public class SaveableData
{
    //all data from the game which is needed to be saved

    //camera stuff like its pos, rotation and current zoom level

    //centipede data to be saved
    [SerializeField] public Vector3 centipedeHeadPosition, centipedeTailBeginSegmentPosition;
    [SerializeField] public Quaternion centipedeHeadRotation, centipedeTailBeginSegmentRotation;

    //length of the list is the number of segments required
    [SerializeField] public SerializableList<Vector3> centipedeSegmentPosition, centipedeCustomSegmentPositon;
    [SerializeField] public SerializableList<Quaternion> centipedeSegmentRotation, centipedeCustomSegmentRotation;
    [SerializeField] public SerializableList<float> centipedeSegmentHealth, centipedeCustomSegmentHealth;
    [SerializeField] public SerializableList<int> centipedeSegmentNumAttacking, centipedeCustomSegmentNumAttack;
    [SerializeField] public int centipedeTailBeginSegmentNumAttack, centipedeTailBeginSegmentWeaponType;
    [SerializeField] public float centipedeSpeed, centipedeTailBeginSegmentHealth; //current movement speed and segment follow speeds

    //the weapon information about each segment
    [SerializeField] public float centipedeTailBeginSegmentWeaponLastFireTime;
    [SerializeField] public SerializableList<int> centipedeSegmentWeaponType, centipedeCustomSegmentWeaponType;
    [SerializeField] public SerializableList<float> centipedeSegmentWeaponLastFireTime, centipedeCustomSegmentWeaponLastFireTime; //for the shield this is the current time of it

    //all the red apples
    [SerializeField] public SerializableList<Vector3> healthApplePos;

    //all the green apples
    [SerializeField] public SerializableList<Vector3> speedApplePos;

    //all the spiders values
    /// <summary>
    /// //////////////////////////bugs out and not works correctly if the spider loads data when near the player
    /// </summary>
    [SerializeField] public int numSpidersLeft;
    [SerializeField] public SerializableDictionary<int, SpiderData> spiderData;
    /// //////////////////////////////////Issue is that if destroyed and the player loads back in a save while still in the same scene, it will not spawn in a new spider to replace it
    //////////////////as well any audio sources playing time should be saved

    //find all the cobwebs and save them as well
    [SerializeField] public SerializableList<WebData> cobwebData;


    //for the centipede save if slowed down by a cobweb or not as well
    [SerializeField] public float centipedePreSlowedSpeed, centipedeSlowedTimer;
    [SerializeField] public bool bCentipedeSlowed;
    //as already save the speed, if it is a slowed one this will ensure that after xxx time it will stop being slowed


    //all the guard ants
    [SerializeField] public SerializableList<GenericAntData> guardAntData;


    //all the dasher ants
    [SerializeField] public SerializableList<DasherAntData> dasherAntData;



    //all the farmer ants
    [SerializeField] public SerializableList<FarmerAntData> farmerAntData;
    [SerializeField] public bool[] useLarvaeBag;
    [SerializeField] public int farmerAntCurrBagPos;

    //all the ant larvae
    [SerializeField] public SerializableList<Vector3> larvaePos;
    [SerializeField] public SerializableList<Quaternion> larvaeRot;

    //all the hunter ants
    [SerializeField] public SerializableList<HunterAntData> hunterAntData;
    [SerializeField] public int[] hunterAntWeaponBag;
    [SerializeField] public int hunterAntCurrBagPos;

    //all the weapon cards in the game


    //all the bomb ants
    [SerializeField] public SerializableList<BombAntData> bombAntData;


    //the cameras stuff


    //stuff for the centipede input and remembering when something was pressed


    //the UI elements, such as


    //everything else, such as which tutorial screens have been seen yet or not


    //i guess all the bullets in the scene as well as the particles(detached segments?probs not required)


    //stuff for registering if only temporarily 


    //the current time in an audio source as well as the audio clip playing, save them both

    //add a constructor to initilize the default values
    public SaveableData()
    {
    }

    //only need to reset the list values, everything else is also reset when they change
    public void ResetData()
    {
        centipedeSegmentPosition = new SerializableList<Vector3>();
        centipedeSegmentRotation = new SerializableList<Quaternion>(); 
        centipedeCustomSegmentPositon = new SerializableList<Vector3>();
        centipedeCustomSegmentRotation = new SerializableList<Quaternion>();
        centipedeCustomSegmentWeaponType = new SerializableList<int>();
        centipedeCustomSegmentWeaponLastFireTime = new SerializableList<float>();
        centipedeSegmentWeaponType = new SerializableList<int>();
        centipedeSegmentWeaponLastFireTime = new SerializableList<float>();

        centipedeSegmentHealth = new SerializableList<float>();
        centipedeSegmentNumAttacking = new SerializableList<int>();
        centipedeCustomSegmentHealth = new SerializableList<float>();
        centipedeCustomSegmentNumAttack = new SerializableList<int>();

        healthApplePos = new SerializableList<Vector3>();
        speedApplePos = new SerializableList<Vector3>();

        spiderData = new SerializableDictionary<int, SpiderData>();
        cobwebData = new SerializableList<WebData>();

        guardAntData = new SerializableList<GenericAntData>();
        dasherAntData = new SerializableList<DasherAntData>();
        farmerAntData = new SerializableList<FarmerAntData>();
        larvaePos = new SerializableList<Vector3>();
        larvaeRot = new SerializableList<Quaternion>();

        bombAntData = new SerializableList<BombAntData>();
        hunterAntData = new SerializableList<HunterAntData>();
    }

    /// <summary>
    /// based on weapon given, determin the int it correlates with
    /// </summary>
    /// <returns></returns>
    public int WeaponToInt(Weapon weapon)
    {
        switch (weapon)
        {
            case Shield shield:
                return (int)EWeaponType.shield;
            case Laser lazer:
                return (int)EWeaponType.laser;
            case Launcher launcher:
                return (int)EWeaponType.launcher;
            case Gun gun:
                return (int)EWeaponType.gun;
            default: //no weapon
                return (int)EWeaponType.empty;
        }
    }

    /// <summary>
    /// based on an int given, determine the specific type of weapon prefab to spawn in
    /// </summary>
    /// <param name="weapon">the type of weapon to spawn</param>
    /// <returns></returns>
    public Weapon IntToWeapon(int weapon)
    {
        switch (weapon)
        {
            case (int)EWeaponType.shield:
                GameObject obj = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Weapons/Shield.prefab", typeof(GameObject));
                return obj.GetComponent<Weapon>();
            case (int)EWeaponType.laser:
                GameObject obj1 = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Weapons/Laser.prefab", typeof(GameObject));
                return obj1.GetComponent<Weapon>();
            case (int)EWeaponType.launcher:
                GameObject obj2 = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Weapons/Launcher.prefab", typeof(GameObject));
                return obj2.GetComponent<Weapon>();
            case (int)EWeaponType.gun:
                GameObject obj3 = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Weapons/Gun.prefab", typeof(GameObject));
                return obj3.GetComponent<Weapon>();
            default:
                return null;
        }
    }

    public void SaveApple(string tag, ref SerializableList<Vector3> applePos)
    {
        GameObject[] apples = GameObject.FindGameObjectsWithTag(tag);
        foreach (GameObject apple in apples)//get every apple which currently exists in the scene and store its position
        {
            applePos.list.Add(apple.transform.position);
        }
        Debug.Log(applePos.list.Count + "Apples posses: " + tag);
    }

    public void SaveLarvae()
    {
        GameObject[] larvae = GameObject.FindGameObjectsWithTag("Larvae");
        foreach (GameObject item in larvae)//get every larvae which currently exists in the scene and store its position + rotation
        {
            larvaePos.list.Add(item.transform.position);
            larvaeRot.list.Add(item.transform.rotation);
        }
    }

    /// <summary>
    /// every time load a save, get all apples in the scene and move them to the saved positions
    /// if still requiring more apples, spawn some in
    /// if any apples which started in the scene have not been moved to a position where one was not eated, it must have been eater, so destroy it
    /// </summary>
    /// <param name="tag">the tag to search the scene for</param>
    /// <param name="applePos">the list of positions to use</param>
    /// <param name="assetPath">the file path to the prefab</param>
    public void LoadApple(string tag, ref SerializableList<Vector3> applePos, string assetPath, ref SerializableList<Quaternion> appleRot)
    {
        GameObject[] apple = GameObject.FindGameObjectsWithTag(tag);
        int i = 0;
        while(i < applePos.list.Count)//for all apples, whose position has been saved, load it in
        {
            Quaternion rot = appleRot.list.Count > 0 ? appleRot.list[i] : Quaternion.identity; //if some rotation data exists use it, else set default rotation
            if (i < apple.Length)
            {
                apple[i].transform.position = applePos.list[i]; //for all apples currently in the scene, move them to one of the preset, saved positions
                apple[i].transform.rotation = rot;
            }
            else //apple does not exist in the scene when it should, so spawn in a new apple
            {
                GameObject obj = (GameObject)AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject));
                MonoBehaviour.Instantiate(obj, applePos.list[i], rot);
                Debug.Log("Loading in a new Apple");
            }
            i++;
        }
        //Debug.Log(i + "applepos" + applePos.list.Count+"  " + apple.Length);
        //if apples still exist in the scene but are already eaten destroy them
        for (int j = apple.Length - 1;  j >= i; j--)
        {
            MonoBehaviour.Destroy(apple[j]);
        }
    }

    public void LoadAllAnts()
    {
        ///find all the different enemy types in the scene
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");//FindObjectsOfType<MonoBehaviour>();//GameObject.FindGameObjectsWithTag(tag);
        List<GameObject> farmerAnt = new List<GameObject>();
        List<GameObject> guardAnt = new List<GameObject>();
        List<GameObject> dasherAnt = new List<GameObject>();
        List<GameObject> bombAnt = new List<GameObject>();
        List<GameObject> hunterAnt = new List<GameObject>();
        foreach (GameObject item in enemies) //loop through all collected items and add them to the appropriate element
        {
            if (item.GetComponent<GuardAnt>() != null)
                guardAnt.Add(item);
            if (item.GetComponent<FarmerAnt>() != null)
                farmerAnt.Add(item);
            if (item.GetComponent<DasherAnt>() != null)
                dasherAnt.Add(item);
            if (item.GetComponent<BombAnt>() != null)
                bombAnt.Add(item);
            if (item.GetComponent<HunterAnt>() != null)
                hunterAnt.Add(item);
        }

        LoadAnt<GenericAntData>(ref guardAnt, ref guardAntData, "Assets/Prefabs/AntComponents/AntPrefabs/GuardAnt.prefab");
        LoadAnt<DasherAntData>(ref dasherAnt, ref dasherAntData, "Assets/Prefabs/AntComponents/AntPrefabs/DasherAnt.prefab");
        LoadAnt<FarmerAntData>(ref farmerAnt, ref farmerAntData, "Assets/Prefabs/AntComponents/AntPrefabs/FarmerAnt.prefab");
        LoadAnt<HunterAntData>(ref hunterAnt, ref hunterAntData, "Assets/Prefabs/AntComponents/AntPrefabs/HunterAnt.prefab");
    }

    /// <summary>
    /// for each type of ant load in the data related to it
    /// </summary>
    /// <typeparam name="T">the specific ant type refering to</typeparam>
    /// <param name="antObjs">all the gameobjects in the scene for the selected ant type</param>
    /// <param name="antData">the ants data for the selected type</param>
    /// <param name="assetPath">the file location of the ants prefab</param>
    public void LoadAnt<T> (ref List<GameObject> antObjs, ref SerializableList<T> antData, string assetPath) where T : GenericAntData
    {
        int i = 0;
//        Debug.Log(antData.list.Count + "num objs to create " + antObjs.Count);
        while (i < antData.list.Count)//for all ants, whose position has been saved, load it in
        {
            if (i < antObjs.Count)
            {
                antObjs[i].transform.position = antData.list[i].antPosition; //for all ants currently in the scene, move them to one of the preset, saved positions
                antObjs[i].transform.rotation = antData.list[i].antRotation; //for all ants currently in the scene, move them to one of the preset, saved positions
                GenericAnt genericAnt = antObjs[i].GetComponent<GenericAnt>();
                genericAnt.Start(); //reset the ant to its spawned in state
                LoadAntData(i, ref antData, ref genericAnt); //assign the appropriate values
            }
            else //ant does not exist in the scene when it should, so spawn in a new apple
            {
                GameObject obj = (GameObject)AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject));
                GameObject spawnedObj = MonoBehaviour.Instantiate(obj, antData.list[i].antPosition, antData.list[i].antRotation);

                GenericAnt genericAnt = spawnedObj.GetComponent<GenericAnt>();
                genericAnt.Start(); //initilize the ants values
                LoadAntData(i, ref antData, ref genericAnt);//this for some reason bugs out and doesnt set the values if using an ant which is just loaded in
            }
            i++;
        }
        //Debug.Log(i + "ants setup" + antData.list.Count + "  " + ant.Count);
        //if ant still exist in the scene but are already dead destroy them
        for (int j = antObjs.Count - 1; j >= i; j--)
        {
            //Debug.Log("Destroying unrequired ant");
            MonoBehaviour.Destroy(antObjs[j]);
        }
    }

    /// <summary>
    /// physically set the ants data here
    /// </summary>
    /// <typeparam name="T">the type of ant class</typeparam>
    /// <param name="i">the position in the antData list using for the data</param>
    /// <param name="antData">list of data for all ants of the selected type</param>
    /// <param name="genericAnt">the specific ant, either existing or just spawned in setting data for</param>
    void LoadAntData<T>(int i, ref SerializableList<T> antData, ref GenericAnt genericAnt) where T : GenericAntData
    {
        GenericAntData genericAntData = antData.list[i];

        genericAnt.callBackupWait = genericAntData.callBackupWait;

        genericAnt.canInvestigate = genericAntData.bCanInvestigate;
        genericAnt.callingBackup = genericAntData.bCallingBackup;

        ////genericAnt.spawnedHelp;//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        genericAnt.spawnedHelpBag.currPos = genericAntData.spawnedHelpCurrPos;

        genericAnt.isHelper = genericAntData.bIsHelper;
        genericAnt.damageStageChance = genericAntData.damageStateOrder;
        genericAnt.healthBag.currPos = genericAntData.damageStateCurrPos;

        genericAnt.health = genericAntData.health;
        genericAnt.AssignAntennaPositions();

        AIIntToState(genericAntData.currAIState, ref genericAnt.stateMachine);
        genericAnt.stateMachine.loadData(genericAntData);
        genericAnt.anim.Play(genericAntData.currAnimName, 0, genericAntData.currAnimNormTime);

        ////for each ant type load in the info specific to it as well
        if (genericAnt as DasherAnt)
        {
            Debug.Log("Loading in the dasher ant data");
            DasherAnt dasherAnt = genericAnt as DasherAnt;

            DasherAntData dasherAntData = genericAntData as DasherAntData;
            dasherAnt.tempAnimSpeed = dasherAntData.tempAnimSpeed;
            dasherAnt.tempRoteSpeed = dasherAntData.tempRotSpeed;
            dasherAnt.tempSpeed = dasherAntData.tempSpeed;

            dasherAnt.Speed = dasherAntData.speed;
            dasherAnt.animMultiplier = dasherAntData.animSpeed;
            dasherAnt.rotSpeed = dasherAntData.rotSpeed;
        }

        else if (genericAnt as FarmerAnt)
        {
            Debug.Log("Loading in the Farmer ant data");
            FarmerAnt farmerAnt = genericAnt as FarmerAnt;

            FarmerAntData farmerAntData = genericAntData as FarmerAntData;
            if (farmerAntData.bHoldingLarvae)
            {
                if (farmerAnt.Larvae == null)
                    farmerAnt.AddLarvae();
            }
            else if (farmerAnt.Larvae != null)
            {
                MonoBehaviour.Destroy(farmerAnt.Larvae);
                farmerAnt.Larvae = null;
            }
        }
        else if (genericAnt as HunterAnt)
        {
            Debug.Log("Loading in the Farmer ant data");
            HunterAnt hunterAnt = genericAnt as HunterAnt;

            HunterAntData hunterAntData = genericAntData as HunterAntData;
            if (hunterAnt.weaponClass != null) //destroy any weapon which currently exists on the ant
                MonoBehaviour.Destroy(hunterAnt.weaponClass.gameObject);

            //spawn in the appropriate, saved weapon
            if (hunterAntData.heldWeapon == (int)EWeaponType.empty)
                hunterAnt.PickWeapon(null);
            else
            {
                hunterAnt.PickWeapon(IntToWeapon(hunterAntData.heldWeapon).gameObject);
                //for the spawned in weapon assign the appropriate values for its things like shooting and stuff/////////////////////////
            }
        }
    }

    public void LoadCobwebs()
    {
        GameObject prefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Web.prefab", typeof(GameObject));
        foreach (WebData web in cobwebData.list)
        {
            GameObject obj = MonoBehaviour.Instantiate(prefab, web.webPosition, web.webRotation);
            Web objWeb = obj.GetComponent<Web>();

            objWeb.isShot = web.bWebIsShot;
            objWeb.despawnTimer = web.webDespawnTimer;

            obj.transform.position = web.webPosition;
            obj.transform.rotation = web.webRotation;

            obj.GetComponent<Rigidbody>().velocity = web.webVelocity;
        }
    }

    /// <summary>
    /// converts the current state machine state to an integer for saving
    /// </summary>
    /// <param name="stateMachine"></param>
    /// <returns></returns>
    public int AIStateToInt(StateMachine stateMachine)
    {
        if (stateMachine.currState == stateMachine.Movement)
            return 0;
        else if (stateMachine.currState == stateMachine.Shock)
            return 1;
        else if (stateMachine.currState == stateMachine.Investigate)
            return 2;
        else if (stateMachine.currState == stateMachine.Attack)
            return 3;
        else if (stateMachine.currState == stateMachine.Damage)
            return 4;
        else if (stateMachine.currState == stateMachine.Dead)
            return 5;
        else//must be the spawn in state
            return 6;
    }
    /// <summary>
    /// takes the saved state and converts it back to assign it as the starting state for the ant
    /// </summary>
    /// <param name="currState"></param>
    /// <param name="stateMachine"></param>
    public void AIIntToState(int currState, ref StateMachine stateMachine)
    {
        switch (currState)
        {
            case 0:
                stateMachine.changeState(stateMachine.Movement);
                break;
            case 1:
                stateMachine.changeState(stateMachine.Shock);
                break;
            case 2:
                stateMachine.changeState(stateMachine.Investigate);
                break;
            case 3:
                stateMachine.changeState(stateMachine.Attack);
                break;
            case 4:
                stateMachine.changeState(stateMachine.Damage);
                break;
            case 5:
                stateMachine.changeState(stateMachine.Dead);
                break;
            default:
                stateMachine.changeState(stateMachine.SpawnIn);
                break;
        }
    }
}
