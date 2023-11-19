using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezerTarget : MonoBehaviour
{
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private Transform _shortcutTarget;
    [SerializeField] private Transform _midwayTarget;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_playerTransform != null)
        {
            if (FreezerMidWay.midwayTarget == true && EntranceShortcut.takenShortcut == false)
            {
                transform.position = _midwayTarget.position;

                transform.rotation = _midwayTarget.rotation;
            }
            else if (EntranceShortcut.inShortcut == true)
            {
                transform.position = _shortcutTarget.position;

                transform.rotation = _shortcutTarget.rotation;
            }
            else
            {
                // Set the position of the FreezerTarget to be the same as the player's position
                transform.position = _playerTransform.position;

                // Set the rotation of the FreezerTarget to be the same as the player's rotation
                transform.rotation = _playerTransform.rotation;
            }
            
        }
    }
}
