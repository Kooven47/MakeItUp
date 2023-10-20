using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallEnclosureCollision : MonoBehaviour
{
    [SerializeField] private GameObject Enclosure;
    // Start is called before the first frame update
    void Start()
    {
        Enclosure.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Enclosure.SetActive(true);
        }
    }
}
