using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ProjectileManager : MonoBehaviour
{
    [SerializeField] private int _projectileCopies = 10;
    [SerializeField] private GameObject _projectilePrefab;
    Queue<GameObject> _projectileQueue = new();
    public static Action<Vector2,Vector2,Ability,float> createProjectile;
    public static Action<Vector2,Vector2,Ability> projectileArc;
    
    public static Action<GameObject> returnProjectile;

    void Start()
    {
        GameObject temp;

        for (int j = 0; j < _projectileCopies; j++)
        {
            temp = Instantiate(_projectilePrefab);
            temp.transform.SetParent(transform);
            temp.GetComponent<ProjectileScript>().Initialize();
            temp.gameObject.SetActive(false);
            _projectileQueue.Enqueue(temp);
        }

        createProjectile = SummonProjectile;
        projectileArc = SummonProjectileArc;
        returnProjectile = ReturnProjectile;
    }

    public void ReturnProjectile(GameObject projectile)
    {
        projectile.SetActive(false);
        _projectileQueue.Enqueue(projectile);
    }

    public void SummonProjectileArc(Vector2 position, Vector2 trajectory, Ability _skill)
    {
            GameObject temp = _projectileQueue.Dequeue();
            temp.gameObject.SetActive(true);
            temp.transform.position = position;
            temp.GetComponent<ProjectileScript>().FireArc(trajectory,_skill);
    }

    public void SummonProjectile(Vector2 position, Vector2 trajectory, Ability _skill, float attack)
    {
            GameObject temp = _projectileQueue.Dequeue();
            temp.gameObject.SetActive(true);
            temp.transform.position = position;
            temp.GetComponent<ProjectileScript>().Fire(trajectory,_skill, attack);
    }
}