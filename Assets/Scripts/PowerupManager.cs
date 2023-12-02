using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupManager : MonoBehaviour
{
    [SerializeField]private GameObject[] _healingItems = new GameObject[1];
    [SerializeField]private int _maxHealingItems = 3;

    Queue<GameObject> _healingItemQ = new();
    public static Action<int,Vector2> spawnHealingItem;
    public static Action<GameObject> returnToHealingPool;

    // Start is called before the first frame update
    void Start()
    {

        if (_healingItems.Length == 0 || _healingItems[0] == null)
        {
            Debug.LogError("Need to initialize one healing prefab");
            return;
        }

        spawnHealingItem = SpawnHealingItem;
        returnToHealingPool = ReturnToHealingPool;

        GameObject temp;

        for (int i = 0; i < _maxHealingItems; i++)
        {
            temp = Instantiate(_healingItems[0]);
            temp.transform.SetParent(transform);
            temp.SetActive(false);
            _healingItemQ.Enqueue(temp);
        }
    }

    void ReturnToHealingPool(GameObject g)
    {
        g.SetActive(false);
        _healingItemQ.Enqueue(g);
    }

    void SpawnHealingItem(int potency,Vector2 position)
    {
        if (potency >= _healingItems.Length || _healingItemQ.Count == 0)
            return;
        
        GameObject temp = _healingItemQ.Dequeue();
        temp.SetActive(true);

        temp.transform.position = position;
    }
}
