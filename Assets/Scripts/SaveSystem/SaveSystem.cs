using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SaveSystem: MonoBehaviour
{
    [Header("File Storage Config")]
    [SerializeField]private string fileName;

    private SaveData sd;
    public static SaveSystem instance {get; private set;}
    private List<ISaveGame> saveSystemObjects;
    private FileDataHandler fdh;

    [SerializeField]private bool useEncryption;


    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Should not occur");
        }
        instance = this;
    }

    private void Start()
    {
        fdh = new FileDataHandler(Application.persistentDataPath,fileName, useEncryption);
        this.saveSystemObjects = FindAllSaveObjects();
        LoadGame();
    }

    private List<ISaveGame> FindAllSaveObjects()
    {
        IEnumerable<ISaveGame> saveSystemObjects = FindObjectsOfType<MonoBehaviour>().
        OfType<ISaveGame>();

        return new List<ISaveGame>(saveSystemObjects);
    }

    public void NewGame()
    {
        sd = new SaveData();
    }

    public void LoadFromRestart()
    {
        sd = fdh.Load();

        if (this.sd == null)
        {
            Debug.Log("No Data");
            NewGame();
        }

        foreach(ISaveGame ss in saveSystemObjects)
        {
            ss.LoadInitialData(sd);
        }
    }

    public void LoadGame()
    {
        sd = fdh.Load();

        if (this.sd == null)
        {
            Debug.Log("No Data");
            NewGame();
        }

        foreach(ISaveGame ss in saveSystemObjects)
        {
            ss.LoadSaveData(sd);
        }
    }

    public void SaveGame()
    {
        
        foreach(ISaveGame ss in saveSystemObjects)
        {
            ss.SaveData(ref sd);
        }

        fdh.Save(sd);
    }

    public void SaveInitialData()
    {
        foreach(ISaveGame ss in saveSystemObjects)
        {
            ss.SaveInitialData(ref sd);
        }

        fdh.Save(sd);
    }

    private void OnApplicationQuit()
    {
        //SaveGame();
    }
}
