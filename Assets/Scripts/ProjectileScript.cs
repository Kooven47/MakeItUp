using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;

public class ProjectileScript : MonoBehaviour
{
    [SerializeField] AnimatorOverrideController _animatorOverride;
    [SerializeField]Material _spriteDefault,_wetOutline, _dryOutline;
    private const int MOTION = 0, IMPACT = 1, IDLE = 2;

    protected int projState = 0;
    Rigidbody2D _rigid;
    Animator _anim;
    BoxCollider2D _hitBox;
    SpriteRenderer _spriteRender;
    private float _damage = 0f, timer = 0f;
    private Coroutine _projectileLife;

    private EnumLib.DamageType _damageType = EnumLib.DamageType.Neutral;
    private EnumLib.KnockBackPower _knockBack = EnumLib.KnockBackPower.Sideways;

    public EnumLib.DamageType damageType
    {
        get{return _damageType;}
    }

    public void Initialize()
    {
        _rigid = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        _hitBox = GetComponent<BoxCollider2D>();
        _spriteRender = GetComponent<SpriteRenderer>();

        _anim.runtimeAnimatorController = _animatorOverride;
    }

    public void Fire(Vector2 trajectory, Ability skill)
    {
        _animatorOverride["Motion"] = skill.projectileAnims[MOTION];
        _damage = skill.damage;
        _damageType = skill.attribute;
        _knockBack = skill.force;

        if (_damageType == EnumLib.DamageType.Dry)
        {
            _spriteRender.material = _dryOutline;
        }
        else if (_damageType == EnumLib.DamageType.Wet)
        {
            _spriteRender.material = _wetOutline;
        }
        else
        {
            _spriteRender.material = _spriteDefault;
        }

        _anim.Play("Motion");
        _rigid.AddForce(trajectory * 300f);
        _projectileLife = StartCoroutine(ProjectileLifeSpan(3f));
    }

    private IEnumerator ProjectileLifeSpan(float projTime)
    {
        yield return new WaitForSeconds(projTime);
        _projectileLife = null;
        Dissipate();
    }

    private void OnCollisionEnter2D(Collision2D collider)
    {
        if (collider.collider.CompareTag("Player"))
        {
            Collider2D col = collider.collider;
            Vector2 knockBack = EnumLib.KnockbackVector(_knockBack);
            col.GetComponent<PlayerInterrupt>().Stagger((int) _damageType,knockBack * 0.25f);
            col.GetComponent<PlayerStats>().DamageCalc(_damage,_damageType,false);
            Dissipate();
        }
    }

    public void Dissipate()
    {
        if (_projectileLife != null)
        {
            StopCoroutine(_projectileLife);
            _projectileLife = null;
        }
        ProjectileManager.returnProjectile?.Invoke(gameObject);
    }
}