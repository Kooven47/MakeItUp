using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntranceShortcut : MonoBehaviour
{
    [SerializeField] private GameObject Enclosure;
    [SerializeField] private Transform target;
    [SerializeField] private bool _isHidden = true;
    public static bool takenShortcut = false;
    public static bool inShortcut = false;
    private bool _collidedBefore = false;

    // Start is called before the first frame update
    void Start()
    {
        if (_isHidden)
            Enclosure.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Enclosure.SetActive(true);
            _collidedBefore = true;
            takenShortcut = true;
            inShortcut = true;
            GameObject.Find("Freezer Variant(Clone)").transform.position = target.position;
        }
    }
}
