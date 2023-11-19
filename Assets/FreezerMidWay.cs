using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezerMidWay : MonoBehaviour
{
    public static bool midwayTarget = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            midwayTarget = false;
        }
    }
}
