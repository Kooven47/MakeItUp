using UnityEngine;

public class SetCameraOrthoSize : MonoBehaviour
{
    [SerializeField] private float minOrthoSize;
    [SerializeField] private float maxOrthoSize;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ScrollWheelZoom.minOrthoSize = minOrthoSize;
            ScrollWheelZoom.maxOrthoSize = maxOrthoSize;
        }
    }
    
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ScrollWheelZoom.minOrthoSize = minOrthoSize;
            ScrollWheelZoom.maxOrthoSize = maxOrthoSize;
        }
    }
}
