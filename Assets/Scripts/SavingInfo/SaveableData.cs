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
public enum EWeaponType
{
    empty = -1,
    shield,
    laser,
    launcher,
    gun
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

    //all the spiders


    //all the dasher ants


    //all the hunter ants


    //all the weapon cards in the game


    //all the farmer ants


    //all the ant larvae


    //all the guard ants


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
}
