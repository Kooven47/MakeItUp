using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnclosureCollision : MonoBehaviour
{
    public SpawnManager spawnManager;
    protected bool _collidedBefore = false;

    public void SkipObjective()
    {
        _collidedBefore = true;
    }

    protected virtual void Start()
    {
        spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {

    }
}