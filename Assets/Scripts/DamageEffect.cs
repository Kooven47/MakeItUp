using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageEffect : MonoBehaviour
{
    SpriteRenderer _spriteRender;
    Coroutine _hitEffect, _hitStop;
    // Start is called before the first frame update
    void Start()
    {
        _spriteRender = transform.GetChild(0).GetComponent<SpriteRenderer>();
    }

    private IEnumerator HitTimer(float timer)
    {
        _spriteRender.color = Color.red;
        
        yield return new WaitForSeconds(timer);
        
        _spriteRender.color = Color.white;
    }

    private IEnumerator HitStop(float timer)
    {
        Time.timeScale = 0.1f;
        yield return new WaitForSecondsRealtime(timer);
        Time.timeScale = 1f;
    }

    public void TriggerEffect()
    {
        if (_hitEffect != null)
            StopCoroutine(_hitEffect);
        
        if (_hitStop != null)
            StopCoroutine(_hitStop);
        
        _hitEffect = StartCoroutine(HitTimer(0.25f));
        _hitStop = StartCoroutine(HitStop(0.125f));
    }
}
