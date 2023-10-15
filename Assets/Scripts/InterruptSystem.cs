using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterruptSystem : MonoBehaviour
{
    protected Animator _anim;
    protected Rigidbody2D _rb;

    public enum ArmorType{Neutral, SuperArmor};

    protected ArmorType _poise = ArmorType.Neutral;

    protected Coroutine _staggerTimer;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        _anim = transform.GetChild(0).GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
    }

    protected virtual IEnumerator StaggerTime(float staggerTime)
    {
        yield return new WaitForSeconds(staggerTime);
    }

    public virtual void Stagger(int damageType, Vector2 knockVector)
    {
        if (_poise == ArmorType.SuperArmor)
            return;
        
    }

    
}
