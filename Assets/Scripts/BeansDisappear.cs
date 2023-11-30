using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeansDisappear : MonoBehaviour
{
    [SerializeField] private GameObject _beans;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _beans.SetActive(false);
        }
    }
}
