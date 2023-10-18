using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ProjectileManager : MonoBehaviour
{
    [SerializeField] private int _projectileCopies = 10;
    [SerializeField] private GameObject _projectilePrefab;
    Queue<GameObject> _projectileQueue = new();

    void Start()
    {
        GameObject temp;

        for (int j = 0; j < _projectileCopies; j++)
        {
            temp = Instantiate(_projectilePrefab);
            temp.transform.SetParent(transform);
            _projectileQueue.Enqueue(temp);
        }
    }

    public void ReturnProjectile(GameObject projectile)
    {

    }

    public void SummonProjectile(EnumLib.ProjectileType projectileKey, Vector2 position, Vector2 trajectory, Ability _skill)
    {
            GameObject temp = _projectileQueue.Dequeue();
            temp.transform.position = position;
            temp.GetComponent<ProjectileScript>().Fire(trajectory,_skill);
    }
}