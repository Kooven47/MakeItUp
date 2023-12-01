using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    private Vector3 offset = new Vector3(0f, 0f, -10f);
    private float smoothTime = 0.25f;
    private Vector3 velocity = Vector3.zero;

    // Allows for the function to be called by any other script without needing a reference to the class
    public static Action StartShake;

    [SerializeField] private Transform target;
    [SerializeField] private CinemachineVirtualCamera _cinemaVirtualCam;
    
    CinemachineBasicMultiChannelPerlin multiChannelPerlin;

    Coroutine _shakeTimer;

    void Start()
    {
        multiChannelPerlin = _cinemaVirtualCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        multiChannelPerlin.m_AmplitudeGain = 0f;
        StartShake = CameraShake;
    }

    private IEnumerator ShakeTimer(float time, float intensity)
    {
        float timeTotal = time;
        while (time > 0f)
        {
            yield return new WaitForSecondsRealtime(Time.deltaTime);
            time -= Time.deltaTime;
            multiChannelPerlin.m_AmplitudeGain = Mathf.Lerp(intensity,0f,(1-(time/timeTotal)));
        }
        _shakeTimer = null;
    }

    private void CameraShake()
    {
        Debug.Log("Camera shake called");
        if (_shakeTimer != null)
        {
            StopCoroutine(_shakeTimer);
        }

        // Dictates intensity of shake. Though try 500 in runtime mode. I promise it's fun ;3
        multiChannelPerlin.m_AmplitudeGain = 5f;

        _shakeTimer = StartCoroutine(ShakeTimer(0.1f,5f));
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        Vector3 targetPosition = target.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}
