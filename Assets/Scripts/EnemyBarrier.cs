using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBarrier : MonoBehaviour
{
    [SerializeField] private GameObject barrier;
    [SerializeField] private GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        Physics2D.IgnoreCollision(barrier.GetComponent<Collider2D>(), player.GetComponent<Collider2D>());
    }
}
