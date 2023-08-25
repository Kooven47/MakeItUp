using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovemenet : MonoBehaviour
{
    Rigidbody2D rb;
    float moveForce = 2;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        rb.AddForce(Input.GetAxis("Horizontal") * moveForce * transform.right, ForceMode2D.Force);
    }
}
