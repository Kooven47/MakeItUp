using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckpointManager : MonoBehaviour,ISaveGame
{
    [SerializeField]private List<GameObject> _checkpoints = new();

    private GameObject _janitor;

    public static Action<int> setCheckPoint;
    public static Action resetCheckPoint;

    private int _currentCheckPoint = 0;

    public int CurrentCheckPoint
    {
        get => _currentCheckPoint;
    }

    private bool _initialized = false;
    // Start is called before the first frame update
    void Awake()
    {
        if (_initialized)
            return;
        _janitor = GameObject.Find("Janitor");
        for (int i = 0; i < transform.childCount; i++)
        {
            _checkpoints.Add(transform.GetChild(i).gameObject);
        }
    
        setCheckPoint = SetCheckPoint;
        resetCheckPoint = ResetCheckPoint;
        _initialized = true;
    }

    public void SetCheckPoint(int i)
    {
        if (i >= _currentCheckPoint)
        {
            _currentCheckPoint = i;
            SaveSystem.instance.SaveGame();
        }
    }

    public void ResetCheckPoint()
    {
        _currentCheckPoint = 0;
        SaveSystem.instance.SaveGame();
    }

    public void LoadSaveData(SaveData data)
    {
        // SceneManager.LoadScene(data.currentLevel);
        _currentCheckPoint = data.numObjectivesCompleted;
        Awake();
        Debug.Log("Current checkpoint asked exceeds checkpoint count: "+(_currentCheckPoint >= _checkpoints.Count));
        Debug.Log("Data says "+data.numObjectivesCompleted);
        int i = _currentCheckPoint >= _checkpoints.Count ? _checkpoints.Count - 1 : _currentCheckPoint;
        Debug.Log("Starting at checkpoint "+i);
        _janitor.transform.position = _checkpoints[i].transform.position;
    }

    public void LoadInitialData(SaveData data)
    {
        // _currentCheckPoint = 0;
    }

    public void SaveData(ref SaveData data)
    {
        data.numObjectivesCompleted = _currentCheckPoint;
        // data.currentLevel = SceneManager.GetActiveScene().buildIndex;
    }

    public void SaveInitialData(ref SaveData data)
    {
        _currentCheckPoint = -1;
    }
}
