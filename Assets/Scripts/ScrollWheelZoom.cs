using Cinemachine;
using UnityEngine;

public class ScrollWheelZoom : MonoBehaviour
{
    private CinemachineVirtualCamera vCam;
    private CinemachineConfiner2D confiner;
    private Collider2D boundingBox;

    [SerializeField] private float defaultOrthographicSize;
    [SerializeField] private float minOrthographicSize;
    [SerializeField] private float maxOrthographicSize;

    private float zoomSpeed = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        vCam = GetComponent<CinemachineVirtualCamera>();
        vCam.m_Lens.OrthographicSize = defaultOrthographicSize;

        confiner = GetComponent<CinemachineConfiner2D>();
        boundingBox = confiner.m_BoundingShape2D;
    }

    // Update is called once per frame
    void Update()
    {
        float scrollWheelInput = Input.GetAxis("Mouse ScrollWheel");
        float newOrthographicSize = vCam.m_Lens.OrthographicSize - scrollWheelInput * zoomSpeed;
        // Had to do -0.1 b/c it breaks at 5 for some reason???
        newOrthographicSize = Mathf.Clamp(newOrthographicSize, minOrthographicSize, maxOrthographicSize - 0.1f);
        if (newOrthographicSize != vCam.m_Lens.OrthographicSize) Debug.Log("New orthographic size: " + newOrthographicSize);
        vCam.m_Lens.OrthographicSize = newOrthographicSize;

        // Apply the confiner's bounding shape to the camera's orthographic bounds
        confiner.InvalidateCache();
        confiner.m_BoundingShape2D = boundingBox;
    }
}