using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageEffect : MonoBehaviour
{
    SpriteRenderer _spriteRender;
    Coroutine _hitEffect, _hitStop;

    [SerializeField]int _numHitFX = 5;

    [SerializeField]GameObject _dustFXPrefab,_splashFXPrefab;

    GameObject _vfxParent;


    Queue<GameObject> _dustFX = new Queue<GameObject>(), _splashFX = new Queue<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        _spriteRender = transform.GetChild(0).GetComponent<SpriteRenderer>();
        _vfxParent = transform.Find("VFX").gameObject;

        GameObject temp;

        for (int i = 0; i < _numHitFX; i++)
        {
            temp = Instantiate(_dustFXPrefab);
            temp.transform.SetParent(_vfxParent.transform,false);
            _dustFX.Enqueue(temp);

            temp = Instantiate(_splashFXPrefab);
            temp.transform.SetParent(_vfxParent.transform,false);
            _splashFX.Enqueue(temp);
        }

    }

    private void TriggerImpact(int type)
    {
        GameObject temp;
        if (type == 1)
        {
            temp = _dustFX.Dequeue();
            temp.SetActive(true);
            // temp.transform.localPosition = Vector2.zero + new Vector2(Random.Range(0f,1f),Random.Range(0f,1f));
            _dustFX.Enqueue(temp);
        }
        else
        {
            temp = _splashFX.Dequeue();
            temp.SetActive(true);
            // temp.transform.localPosition = Vector2.zero + new Vector2(Random.Range(0f,1f),Random.Range(0f,1f));
            _splashFX.Enqueue(temp);
        }

        temp.transform.localPosition = Vector2.zero;
    }

    private IEnumerator HitTimer(float timer)
    {
        _spriteRender.color = Color.red;
        
        yield return new WaitForSeconds(timer);
        
        _spriteRender.color = Color.white;
    }

    private IEnumerator HitStop(float timer)
    {
        if (PlayerStats.playerIsDead) yield break;
        Time.timeScale = 0.1f;
        yield return new WaitForSecondsRealtime(timer);
        Time.timeScale = 1f;
    }

    public void TriggerEffect(int damageType)
    {
        if (_hitEffect != null)
            StopCoroutine(_hitEffect);
        
        if (_hitStop != null)
            StopCoroutine(_hitStop);

        TriggerImpact(damageType);
        
        _hitEffect = StartCoroutine(HitTimer(0.25f));
        _hitStop = StartCoroutine(HitStop(0.125f));
    }
}
