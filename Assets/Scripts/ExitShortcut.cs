using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitShortcut : MonoBehaviour
{
    [SerializeField] private GameObject Enclosure;
    [SerializeField] private bool _isHidden = true;
    private bool _collidedBefore = false;

    // Start is called before the first frame update
    void Start()
    {
        if (_isHidden)
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
            _collidedBefore = true;
            EntranceShortcut.takenShortcut = false;
        }
    }
}
