using System.Collections;
using Cinemachine;
using UnityEngine;

public class ScrollWheelZoom : MonoBehaviour
{
    private CinemachineVirtualCamera vCam;
    private CinemachineConfiner2D confiner;
    private Collider2D boundingBox;
    
    private float curMinOrthographicSize;
    private float curMaxOrthographicSize;
    public static float minOrthoSize = 1f;
    public static float maxOrthoSize = 4.9f;
    private float zoomSpeed = 1.0f;
    private bool updating = false;
    [SerializeField] private float cameraUpdateTime = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        vCam = GetComponent<CinemachineVirtualCamera>();
        curMinOrthographicSize = minOrthoSize;
        curMaxOrthographicSize = maxOrthoSize;
        // vCam.m_Lens.OrthographicSize = (curMinOrthographicSize + curMaxOrthographicSize) / 2;
        vCam.m_Lens.OrthographicSize = curMaxOrthographicSize;
        
        confiner = GetComponent<CinemachineConfiner2D>();
        boundingBox = confiner.m_BoundingShape2D;
    }

    // Update is called once per frame
    void Update()
    {
        // Just stepped into new barrier
        if (minOrthoSize != curMinOrthographicSize || maxOrthoSize != curMaxOrthographicSize)
        {
            float currentRatio = (vCam.m_Lens.OrthographicSize - curMinOrthographicSize) /
                                 (curMaxOrthographicSize - curMinOrthographicSize);
            float newCalculatedOrthographicSize = currentRatio * (maxOrthoSize - minOrthoSize) + minOrthoSize;
            // Debug.Log("Old orthographic size: " + vCam.m_Lens.OrthographicSize);
            updating = true;
            StartCoroutine(SmoothTowardsNewValue(newCalculatedOrthographicSize, cameraUpdateTime));
            curMinOrthographicSize = minOrthoSize;
            curMaxOrthographicSize = maxOrthoSize;
        }

        if (!updating)
        {
            float scrollWheelInput = Input.GetAxis("Mouse ScrollWheel");
            float newOrthographicSize = vCam.m_Lens.OrthographicSize - scrollWheelInput * zoomSpeed;
            if (Input.GetKeyDown(KeyCode.Equals)) // Use KeyCode.Equals for '+'
                newOrthographicSize -= (zoomSpeed * 0.5f);
            if (Input.GetKeyDown(KeyCode.Minus))
                newOrthographicSize += (zoomSpeed * 0.5f);
            newOrthographicSize = Mathf.Clamp(newOrthographicSize, curMinOrthographicSize, curMaxOrthographicSize);
            // if (newOrthographicSize != vCam.m_Lens.OrthographicSize) Debug.Log("New orthographic size: " + newOrthographicSize);
            vCam.m_Lens.OrthographicSize = newOrthographicSize;
        }

        // Apply the confiner's bounding shape to the camera's orthographic bounds
        confiner.InvalidateCache();
        confiner.m_BoundingShape2D = boundingBox;
    }
    
    IEnumerator SmoothTowardsNewValue(float endValue, float duration)
    {
        float time = 0;
        float startValue = vCam.m_Lens.OrthographicSize;
        while (time < duration)
        {
            vCam.m_Lens.OrthographicSize = Mathf.Lerp(startValue, endValue, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        vCam.m_Lens.OrthographicSize = endValue;
        updating = false;
        // Debug.Log("New orthographic size: " + endValue);
    }
}