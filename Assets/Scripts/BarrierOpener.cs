using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierOpener : MonoBehaviour
{
    [SerializeField] private GameObject Enclosure;
    void Start()
    {
        Enclosure.SetActive(true);
    }
        private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Enclosure.SetActive(false);
        }
    }

}
