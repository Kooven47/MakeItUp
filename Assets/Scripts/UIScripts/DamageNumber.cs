using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DamageNumbers : MonoBehaviour
{
    Animator _anim;
    TMP_Text _damageText,_subText;

    public void Initialize()
    {
        _anim = GetComponent<Animator>();
        _damageText = transform.GetChild(0).GetComponent<TMP_Text>();
        _subText = transform.GetChild(1).GetComponent<TMP_Text>();
    }
    public void SetValue(float val, int effective)
    {
        gameObject.SetActive(true);
        _damageText.SetText(val.ToString());
        if (effective == 1)
        {
            _subText.SetText("EFFECTIVE!");
            _anim.Play("Weak");
        } 
        else if (effective == -1)
        {
            _subText.SetText("RESIST!");
            _anim.Play("Resist");
        }
        else
        {
            _subText.SetText("");
            _anim.Play("Damage");
        }
            
    }

    public void ReturntoPool()
    {
        _subText.SetText("");
        gameObject.SetActive(false);
        DamageNumberPool.returnToPool?.Invoke(GetComponent<DamageNumbers>());
    }
}