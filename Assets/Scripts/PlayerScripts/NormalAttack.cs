using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalAttack : MonoBehaviour
{
    [SerializeField]private Animator _anim;
    bool _attackBuffer = false, _inAttack = false;
    [SerializeField]int _chain = 0;
    // Start is called before the first frame update
    void Start()
    {
        _anim = GetComponent<Animator>();
        
    }

    void Recover()
    {
        if (_attackBuffer)
        {
            _attackBuffer = false;
            Attack();
        }
        else
        {
            Debug.Log("End Chain");
            _inAttack = false;
            _chain = 0;
            _anim.SetInteger("chain",_chain);

        }
    }

    void Attack()
    {
        _chain++;
        if (_chain > 3)
            _chain = 1;
        
        _anim.SetInteger("chain",_chain);
        _inAttack = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp("z"))
        {
            if (!_inAttack)
            {
                Attack();
            }
            else
            {
                _attackBuffer = true;
            }
                
        }
    }
}
