using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour,ISaveGame
{
    [SerializeField]private List<GameObject> _checkpoints = new();
    [SerializeField]private List<EnclosureCollision> _enclosureTriggers = new();

    private GameObject _janitor;

    public static Action<int> setCheckPoint;

    private int _currentCheckPoint = -1;

    private bool _initialized = false;
    // Start is called before the first frame update
    void Awake()
    {
        if (_initialized)
            return;
        _janitor = GameObject.Find("Janitor");
        for(int i = 0; i < transform.childCount; i++)
        {
            _checkpoints.Add(transform.GetChild(i).gameObject);
        }

        setCheckPoint = SetCheckPoint;
        _initialized = true;
    }

    public void SetCheckPoint(int i)
    {
        if (i >= 0)
            _currentCheckPoint = i;
    }

    public void LoadSaveData(SaveData data)
    {
        _currentCheckPoint = data.numObjectivesCompleted;
        Awake();
        int i = _currentCheckPoint >= _checkpoints.Count ? _checkpoints.Count - 1 : _currentCheckPoint;
        _janitor.transform.position = _checkpoints[i].transform.position;
    }

    public void LoadInitialData(SaveData data)
    {
        _currentCheckPoint = 0;
    }

    public void SaveData(ref SaveData data)
    {
        data.numObjectivesCompleted = _currentCheckPoint;
    }

    public void SaveInitialData(ref SaveData data)
    {
        _currentCheckPoint = -1;
    }
}
