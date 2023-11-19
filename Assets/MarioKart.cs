using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarioKart : MonoBehaviour
{
    [SerializeField] int starForce = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("HELP");
            other.attachedRigidbody.AddForce(new Vector2(-starForce, 0));
        }
    }
   }
