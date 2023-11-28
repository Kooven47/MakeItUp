using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerColliderLevel1 : EnclosureCollision
{
    [SerializeField] private bool SetCheckpoint;

    protected override void Start()
    {
        base.Start();
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (SetCheckpoint && !_collidedBefore)
            {
                if (CheckpointManager.setCheckPoint == null)
                {
                    Debug.Log("Checkpoint manager wasn't set");
                }
                CheckpointManager.setCheckPoint?.Invoke(4);
                Debug.Log("Checkpoint set");
            }
            
            _collidedBefore = true;
        }
    }
}
