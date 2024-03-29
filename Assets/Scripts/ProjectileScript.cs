using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;

public class ProjectileScript : MonoBehaviour
{
    [SerializeField] AnimatorOverrideController _animatorOverride;
    [SerializeField]Material _spriteDefault,_wetOutline, _dryOutline;
    protected const int MOTION = 0, IMPACT = 1, IDLE = 2;

    protected int _projState = 0, _bounces = 3, _maxBounce = 3;
    protected Rigidbody2D _rigid;
    protected Animator _anim;
    protected BoxCollider2D _hitBox;
    protected SpriteRenderer _spriteRender;
    protected float _damage = 0f, timer = 0f;
    protected Coroutine _projectileLife;

    protected Vector2 _lastVelocity = Vector2.zero;

    protected EnumLib.DamageType _damageType = EnumLib.DamageType.Neutral;
    protected EnumLib.KnockBackPower _knockBack = EnumLib.KnockBackPower.Sideways;

    private float _attack = 0f;

    private bool _isHoming = false;
    private float _movementSpeed = 5f;
    private float _angleChangingSpeed = 1f;

    private Transform _target;

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
        _animatorOverride = Instantiate(_animatorOverride);

        _anim.runtimeAnimatorController = _animatorOverride;
    }

    public void FireArc(Vector2 trajectory, Ability skill)
    {
        InitializeProjectile(skill);
        _anim.Play("Motion");
        _rigid.AddForce(trajectory * 300f);
        _projectileLife = StartCoroutine(ProjectileLifeSpan(6f));
        _rigid.gravityScale = 1f;
    }

    public void InitializeProjectile(Ability skill)
    {
        _animatorOverride["Motion"] = skill.projectileAnims[MOTION];
        _animatorOverride["Idle"] = skill.projectileAnims[IDLE];
        _projState = MOTION;
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
    }

    public void FireHoming(Transform target, Ability skill, float attack)
    {
        Debug.Log("Fired homing projectile!");
        _target = target;
        InitializeProjectile(skill);
        _anim.Play("Motion");
        _projectileLife = StartCoroutine(ProjectileLifeSpan(5f));
        _rigid.gravityScale = 0f;
        _attack = attack;
        _isHoming = true;
        _bounces = 0;
    }

    public void Fire(Vector2 trajectory, Ability skill, float attack)
    {
        Debug.Log("Initiatlizing projectile with "+skill.name);
        InitializeProjectile(skill);
        _bounces = _maxBounce;
        _anim.Play("Motion");
        _rigid.AddForce(trajectory * 300f);
        _projectileLife = StartCoroutine(ProjectileLifeSpan(3f));
        _rigid.gravityScale = 0f;
        _attack = attack;
    }

    protected IEnumerator ProjectileLifeSpan(float projTime)
    {
        yield return new WaitForSeconds(projTime);
        _projectileLife = null;
        Dissipate();
    }

    protected void OnCollisionEnter2D(Collision2D collider)
    {
        if (collider.collider.CompareTag("Player"))
        {
            Collider2D col = collider.collider;
            PlayerStats _playerStat = col.GetComponent<PlayerStats>();
            if (!_playerStat.iFrame)
            {
                Vector2 knockBack = EnumLib.KnockbackVector(_knockBack);
                col.GetComponent<PlayerInterrupt>().Stagger((int) _damageType,Vector2.zero);
                _playerStat.DamageCalc(_damage + _attack,_damageType,false); 
                col.gameObject.GetComponent<DamageEffect>().TriggerEffect((int) _damageType);
            }
            Dissipate();
        }
        else if (collider.collider.CompareTag("Ground"))
        {
            if (_damageType == EnumLib.DamageType.Dry)
            {
                if (_bounces == 0)
                {
                    _bounces = _maxBounce;
                    Dissipate();
                    return;
                }
                
                Debug.Log("Bounce!");
                _bounces--;
                float speed = _lastVelocity.magnitude;
                Vector2 direction = Vector2.Reflect(_lastVelocity.normalized,collider.contacts[0].normal);
                _rigid.velocity = direction * Mathf.Max(speed,0f); 
            }
            else if (_damageType == EnumLib.DamageType.Wet && _projState != IDLE)
            {
                _isHoming = false;
                Linger();
            }
            
        }
    }

    public void Dissipate()
    {
        if (_projectileLife != null)
        {
            StopCoroutine(_projectileLife);
            _projectileLife = null;
        }
        _isHoming = false;
        ProjectileManager.returnProjectile?.Invoke(gameObject);
    }

    public void Linger()
    {
        if (_projectileLife != null)
        {
            Debug.Log("Lingering");
            _projState = IDLE;
            StopCoroutine(_projectileLife);
            _projectileLife = StartCoroutine(ProjectileLifeSpan(2f));
            _rigid.velocity = Vector2.zero;
            _anim.Play("Idle");
        }
    }

    void FixedUpdate()
    {
        if(_isHoming)
        {
            _rigid.position = Vector2.MoveTowards(_rigid.position,_target.position, _movementSpeed * Time.deltaTime);
        }
    }

    void Update()
    {
        _lastVelocity = _rigid.velocity;
    }
}