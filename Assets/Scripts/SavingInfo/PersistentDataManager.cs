using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;
//does the handling of the saving and loading of the games data
//only ever have one of these in the scene at one time. only this is required to be added for the entire saving to function correctly
public class PersistentDataManager : MonoBehaviour
{
    SaveableData saveableData;

    List<IDataInterface> dataInterfaces; //stores all objects in the scene which have data requiring to be saved

    string dataDirectory; //for the unity project the persistant data path 

    string dataFileName = "SaveFile1.json";

    private void Awake()
    {
        dataDirectory = Application.persistentDataPath;
    }

    private void Start()
    {
        NewGame();
    }
    public void NewGame()
    {
        this.saveableData = new SaveableData();

        saveableData.centipedeSegmentPosition = new SerializableList<Vector3>();
        saveableData.centipedeSegmentRotation = new SerializableList<Quaternion>();
        saveableData.centipedeSegmentHealth = new SerializableList<float>();
        saveableData.centipedeSegmentNumAttacking = new SerializableList<int>();
    }

    /// <summary>
    /// loads in all saved data from a file
    /// </summary>
    public void LoadGame()
    {
        //load in the games data
        string fullPath = Path.Combine(dataDirectory, dataFileName);//this accounts for different paths having different path seperators
        if (File.Exists(fullPath))
        {
            try
            {
                string dataToLoad;
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))//get the specified file and open it 
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd(); //load the data into the specified string
                    }

                }
                SaveableData loadedSavedData = JsonUtility.FromJson<SaveableData>(dataToLoad);//deserilize and load the data back in to the file
                saveableData = loadedSavedData;
            }
            catch (System.Exception e)
            {

                Debug.Log("failed to load, specified path could not be found: " + fullPath + "/" + e);
            }
        }
        foreach (IDataInterface dataObj in ObjsDataSaveable())
        {
            dataObj.LoadData(saveableData);
        }

        saveableData.LoadApple("Health", ref saveableData.healthApplePos, "Assets/Prefabs/RedApple.prefab");
        saveableData.LoadApple("Speed", ref saveableData.speedApplePos, "Assets/Prefabs/GreenApple.prefab");
    }

    /// <summary>
    /// saves all saveable data to a file
    /// </summary>
    public void SaveGame()
    {
        saveableData.ResetData();
        foreach (IDataInterface dataObj in ObjsDataSaveable())
        {
            dataObj.SaveData(ref saveableData);
        }
        saveableData.SaveApple("Health", ref saveableData.healthApplePos);
        saveableData.SaveApple("Speed", ref saveableData.speedApplePos);


        string fullPath = Path.Combine(dataDirectory, dataFileName);//this accounts for different paths having different path seperators
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));//if the specified directory does not exist create it

            //serilize the data from a C# script into a JSON file
            string dataToSave = JsonUtility.ToJson(saveableData, true);
            using (FileStream stream = new FileStream(fullPath, FileMode.Create))//physically make a new file in the specified location
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToSave); //save the specified data to the file
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.Log("Error saving file to specified path: " + fullPath + "/" + e);
        }
    }

    public void Update()
    {
        //test functionality, change to buttons later
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("Saving current game state");
            SaveGame();
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            Debug.Log("Loading in game from a save");
            LoadGame();
        }
    }

    IEnumerable<IDataInterface> ObjsDataSaveable()
    {
        return FindObjectsOfType<MonoBehaviour>().OfType<IDataInterface>();//FindObjectsOfType<MonoBehaviour>().OfType<IDataInterface>();
    }
}
