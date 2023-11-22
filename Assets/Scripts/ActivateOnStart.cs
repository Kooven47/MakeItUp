using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateOnStart : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject block = this.gameObject;
        Debug.Log("gameobject name: " + block.transform.name);
        block.SetActive(true);
    }
}
