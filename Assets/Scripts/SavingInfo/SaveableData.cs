using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//enables the saving of a list to a json file for later access
[System.Serializable]
public class SerializableList<T>
{
    public List<T> list = new List<T>();
}


[System.Serializable]
public class SaveableData
{
    //all data from the game which is needed to be saved



    //centipede data to be saved
    [SerializeField] public Vector3 centipedeHeadPosition;
    [SerializeField] public Quaternion centipedeHeadRotation;

    //length of the list is the number of segments required
    [SerializeField] public SerializableList<Vector3> centipedeSegmentPosition;
    [SerializeField] public SerializableList<Quaternion> centipedeSegmentRotation;
    //centipedeCustomSegmentPosition, centipedeCustomSegmentRotation;
    [SerializeField] public Vector3 centipedeTailPosition;
    [SerializeField] public Quaternion centipedeTailRotation;
    [SerializeField] public SerializableList<float> centipedeSegmentHealth;
    [SerializeField] public SerializableList<int> centipedeSegmentNumAttacking;
    [SerializeField] public float centipedeSpeed; //current movement speed and segment follow speeds

    //either save the surface normal of every segment or update it as soon as everything spawns in(otherwise it mega buggs out)

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
        centipedeSegmentHealth = new SerializableList<float>();
        centipedeSegmentNumAttacking = new SerializableList<int>();
    }
}
