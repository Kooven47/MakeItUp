using System;
using System.Collections;
using UnityEngine;

public class FreezerCore : MonoBehaviour
{
    private EnemyAI _freezerAI;
    private GameObject _player;
    
    private void Start()
    {
        _freezerAI = this.GetComponent<EnemyAI>();
        _player = GameObject.FindWithTag("Player");
    }
    private void Update()
    {
        AdjustFreezerSpeedBasedOnDistance();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Obstacle"))
        {
            var block = other.gameObject;
            block.SetActive(false);
            StartCoroutine(SlowFreezerDown(0.2f, 0.5f));
        }
    }

    private IEnumerator SlowFreezerDown(float time, float ratio)
    {
        _freezerAI.speed *= ratio;
        yield return new WaitForSeconds(time);
        _freezerAI.speed /= ratio;
    }

    private void AdjustFreezerSpeedBasedOnDistance()
    {
        const int minSpeed = 400;
        const int maxSpeed = 800;
        const int minDistance = 5;
        const int maxDistance = 20;
        
        var distance = Vector2.Distance(_player.transform.position, transform.position);
        Debug.Log($"Distance: {distance}");
        
        if (distance > maxDistance) 
        { 
            _freezerAI.speed = maxSpeed; 
        } 
        else if (distance < minDistance) 
        { 
            _freezerAI.speed = minSpeed; 
        } 
        else 
        { 
            var distanceRatio = (distance - minDistance) / (maxDistance - minDistance);
            var diffSpeed = maxSpeed - minSpeed;
            _freezerAI.speed = (distanceRatio * diffSpeed) + minSpeed; 
        }
    }
}
