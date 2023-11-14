using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezerTarget : MonoBehaviour
{
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private Transform _shortcutTarget;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_playerTransform != null)
        {
            if (EntranceShortcut.takenShortcut == false) 
            {
                // Set the position of the FreezerTarget to be the same as the player's position
                transform.position = _playerTransform.position;

                // Set the rotation of the FreezerTarget to be the same as the player's rotation
                transform.rotation = _playerTransform.rotation;
            }
            else
            {
                transform.position = _shortcutTarget.position;

                transform.rotation = _shortcutTarget.rotation;
            }
        }
    }
}
