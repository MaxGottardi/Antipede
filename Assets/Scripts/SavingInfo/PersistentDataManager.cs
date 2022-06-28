using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using System.IO;
//does the handling of the saving and loading of the games data
//only ever have one of these in the scene at one time. only this is required to be added for the entire saving to function correctly
public class PersistentDataManager : MonoBehaviour
{
    public static SaveableData saveableData; //the data which is getting saved
    public static string directoryName; //the name to call the folder saving to
    public static bool bIsNewGame = true;

    List<IDataInterface> dataInterfaces; //stores all objects in the scene which have data requiring to be saved

    string dataDirectory; //for the unity project the persistant data path 

    string dataFileName = "SaveFile"; //the name to call the save file

    [Header("Stuff for Setting the save UI")]
    public GameObject saveButtonPrefab;
    public Transform saveButtonParent;

    private void Awake()
    {
        dataDirectory = Application.persistentDataPath;
        dataDirectory = Path.Combine(dataDirectory, "Saves");
        //the full path C:\Users\[usersname]\AppData\LocalLow\[companyname]\[projectname]
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            if (bIsNewGame)
                NewGame();
            else
                LoadGame();
        }
    }
    public void NewGame()
    {
        saveableData = new SaveableData();

        saveableData.centipedeSegmentPosition = new SerializableList<Vector3>();
        saveableData.centipedeSegmentRotation = new SerializableList<Quaternion>();
        saveableData.centipedeSegmentHealth = new SerializableList<float>();
        saveableData.centipedeSegmentNumAttacking = new SerializableList<int>();
    }

    public void DeleteSaveFile(string saveName)
    {
        if (saveName != null)
        {
            string fullPath = Path.Combine(dataDirectory, saveName, dataFileName);//this accounts for different paths having different path seperators
            try
            {
                if (File.Exists(fullPath))
                {
                    Directory.Delete(Path.GetDirectoryName(fullPath), true); //recursivly delete entire directory
                }
            }
            catch (System.Exception e)
            {

                Debug.Log("failed to load, specified path could not be found: " + fullPath + "/" + e);
            }
        }
    }


    public SaveableData LoadSaveFile(string saveName)
    {
        string fullPath = Path.Combine(dataDirectory, saveName, dataFileName);//this accounts for different paths having different path seperators
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
                return loadedSavedData;
            }
            catch (System.Exception e)
            {

                Debug.Log("failed to load, specified path could not be found: " + fullPath + "/" + e);
            }
        }
        return null; //no save file found
    }
    /// <summary>
    /// loads in all saved data from a file when the game starts up
    /// </summary>
    public void LoadGame()
    {
        //load in the games data

        foreach (IDataInterface dataObj in ObjsDataSaveable())
        {
            dataObj.LoadData(saveableData);
        }
        SerializableList<Quaternion> emptyList = new SerializableList<Quaternion>(); //used for items with no saved rotation
        saveableData.LoadApple(GameObject.FindGameObjectsWithTag("Health"), ref saveableData.healthApplePos, "Assets/Prefabs/RedApple.prefab", ref emptyList);
        saveableData.LoadApple(GameObject.FindGameObjectsWithTag("Speed"), ref saveableData.speedApplePos, "Assets/Prefabs/GreenApple.prefab", ref emptyList);
        saveableData.LoadApple(GameObject.FindGameObjectsWithTag("Larvae"), ref saveableData.larvaePos, "Assets/Prefabs/Larvae.prefab", ref saveableData.larvaeRot);

        Tarantula.numTarantulasLeft = saveableData.numSpidersLeft;
        saveableData.LoadCobwebs();
        saveableData.LoadAllAnts();
        FarmerAnt.larvaeBag.shuffleList = saveableData.useLarvaeBag;
        FarmerAnt.larvaeBag.currPos = saveableData.farmerAntCurrBagPos;

        //assign the appropriate values to the shuffle list
        HunterAnt.weaponsBag.currPos = saveableData.hunterAntCurrBagPos;
        for (int i = 0; i < saveableData.hunterAntWeaponBag.Length; i++)
        {
            HunterAnt.weaponsBag.shuffleList[i] = saveableData.IntToWeapon(saveableData.hunterAntWeaponBag[i]).gameObject;
        }

        saveableData.LoadWeaponCards();

    }

    /// <summary>
    /// saves all saveable data to a file
    /// </summary>
    public void SaveGame()
    {
        saveableData.ResetData();
        foreach (IDataInterface dataObj in ObjsDataSaveable())
        {
            dataObj.SaveData(saveableData);
        }
        saveableData.SaveApple("Health", ref saveableData.healthApplePos);
        saveableData.SaveApple("Speed", ref saveableData.speedApplePos);
        saveableData.numSpidersLeft = Tarantula.numTarantulasLeft;

        saveableData.SaveLarvae();
        saveableData.SaveWeaponCards();

        string fullPath = Path.Combine(dataDirectory, directoryName, dataFileName);//this accounts for different paths having different path seperators
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
            directoryName = "SaveFile";
            Debug.Log("Saving current game state");
            SaveGame();
        }
        if (Input.GetKeyDown(KeyCode.O) && Time.timeScale > 0.5f)
        {
            saveableData = LoadSaveFile("SaveFile");
            Debug.Log("Loading in game from a save");
            LoadGame();
            StartCoroutine(TempPaused(1f));
        }
    }
    IEnumerator TempPaused(float waitTime)
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(waitTime);
        Time.timeScale = 1;
    }

    IEnumerable<IDataInterface> ObjsDataSaveable()
    {
        return FindObjectsOfType<MonoBehaviour>().OfType<IDataInterface>();//FindObjectsOfType<MonoBehaviour>().OfType<IDataInterface>();
    }

    /// <summary>
    /// finds all save files for the game
    /// </summary>
    /// <returns>a dictionary of all save directories</returns>
    public Dictionary<string, SaveableData> LoadAllSaves()
    {
        Dictionary<string, SaveableData> foundSaves = new Dictionary<string, SaveableData>();
        if (Directory.Exists(dataDirectory))
        {
            IEnumerable<DirectoryInfo> dirInfos = new DirectoryInfo(dataDirectory).EnumerateDirectories(); //loops over all directories in the specified location
            foreach (DirectoryInfo item in dirInfos)
            {
                string saveName = item.Name;

                string fullPath = Path.Combine(dataDirectory, saveName, dataFileName);
                if (File.Exists(fullPath)) //checks if the save file exists in the directory, if it doesn't ignore it
                {
                    SaveableData saveableData = LoadSaveFile(saveName);
                    if (saveableData != null)
                    {
                        foundSaves.Add(saveName, saveableData);
                    }
                }
            }
        }

        return foundSaves;
    }

    public void OnEnable()
    {
        ////if any child obj exists, first delete it to reset the list
        //only call while on the main menu scene
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            for (int i = saveButtonParent.childCount - 1; i >= 0; i--)
            {
                Destroy(saveButtonParent.GetChild(i).gameObject);
            }

            Dictionary<string, SaveableData> allSaveFiles = LoadAllSaves();
            foreach (KeyValuePair<string, SaveableData> item in allSaveFiles)
            {
                GameObject button = Instantiate(saveButtonPrefab, saveButtonParent);
                SaveUIData saveUIData = button.GetComponent<SaveUIData>();
                saveUIData.InitilizeData(item.Key, item.Value);
            }
        }
    }
}
