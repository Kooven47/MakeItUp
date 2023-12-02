using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarioKartBarrier : MonoBehaviour
{
    [SerializeField] GameObject barrier;
    void Start()
    {
        barrier.SetActive(false);
    }

    protected void OnTriggerEnter2D(Collider2D other)
    {
        barrier.SetActive(true);
    }
 }
