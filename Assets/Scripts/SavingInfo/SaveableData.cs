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
    [SerializeField] public Vector3 antPosition;
    [SerializeField] public Quaternion antRotation;

    [SerializeField] public float health;

    //stuff for the current state and lengths of time in each one which uses and animation
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
    [SerializeField] public int numSpidersLeft;
    [SerializeField] public SerializableDictionary<int, SpiderData> spiderData;
    /// //////////////////////////////////Issue is that if destroyed and the player loads back in a save while still in the same scene, it will not spawn in a new spider to replace it

    //find all the cobwebs and save them as well
    [SerializeField] public SerializableList<WebData> cobwebData;


    //for the centipede save if slowed down by a cobweb or not as well
    [SerializeField] public float centipedePreSlowedSpeed, centipedeSlowedTimer;
    [SerializeField] public bool bCentipedeSlowed;
    //as already save the speed, if it is a slowed one this will ensure that after xxx time it will stop being slowed


    //all the guard ants
    [SerializeField] SerializableList<GenericAnt> guardAntData;


    //all the dasher ants


    //all the hunter ants


    //all the weapon cards in the game


    //all the farmer ants


    //all the ant larvae

    //all the bomb ants


    //the cameras stuff


    //stuff for the centipede input and remembering when something was pressed


    //the UI elements, such as


    //everything else, such as which tutorial screens have been seen yet or not


    //i guess all the bullets in the scene as well as the particles(detached segments?probs not required)


    //stuff for registering if only temporarily 


    //the weapon which is attached to each segment, save a reference to it

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

    /// <summary>
    /// every time load a save, get all apples in the scene and move them to the saved positions
    /// if still requiring more apples, spawn some in
    /// if any apples which started in the scene have not been moved to a position where one was not eated, it must have been eater, so destroy it
    /// </summary>
    /// <param name="tag">the tag to search the scene for</param>
    /// <param name="applePos">the list of positions to use</param>
    /// <param name="assetPath">the file path to the prefab</param>
    public void LoadApple(string tag, ref SerializableList<Vector3> applePos, string assetPath)
    {
        GameObject[] apple = GameObject.FindGameObjectsWithTag(tag);
        int i = 0;
        while(i < applePos.list.Count)//for all apples, whose position has been saved, load it in
        {
            if (i < apple.Length)
                apple[i].transform.position = applePos.list[i]; //for all apples currently in the scene, move them to one of the preset, saved positions
            else //apple does not exist in the scene when it should, so spawn in a new apple
            {
                GameObject obj = (GameObject)AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject));
                MonoBehaviour.Instantiate(obj, applePos.list[i], Quaternion.identity);
                Debug.Log("Loading in a new Apple");
            }
            i++;
        }
        Debug.Log(i + "applepos" + applePos.list.Count+"  " + apple.Length);
        //if apples still exist in the scene but are already eaten destroy them
        for (int j = apple.Length - 1;  j >= i; j--)
        {
            Debug.Log("Destroy Apple");
            MonoBehaviour.Destroy(apple[j]);
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
}
