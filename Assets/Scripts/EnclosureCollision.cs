using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnclosureCollision : MonoBehaviour
{
    public SpawnManager spawnManager;
    [SerializeField]protected bool _collidedBefore = false;
    
    [SerializeField]protected int _objectiveIndex = 0;

    public void SkipObjective()
    {
        _collidedBefore = true;
    }

    public void SetCheckPoint()
    {
        if (CheckpointManager.setCheckPoint == null)
        {
            Debug.Log("Checkpoint manager wasn't set");
        }
        CheckpointManager.setCheckPoint?.Invoke(_objectiveIndex);
    }

    protected virtual void Start()
    {
        spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {

    }
}