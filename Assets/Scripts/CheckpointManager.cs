using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    [SerializeField]private List<GameObject> _checkpoints = new();
    [SerializeField]private List<EnclosureCollision> _enclosureTriggers = new();

    private int _currentCheckPoint = 0;
    // Start is called before the first frame update
    void Awake()
    {
        
    }
}
