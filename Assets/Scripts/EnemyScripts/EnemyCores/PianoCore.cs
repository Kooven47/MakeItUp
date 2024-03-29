using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PianoCore : EnemyCore
{
    [SerializeField] private GameObject _dropTelegraph, _platformDetector;
    [SerializeField] private float _trackSpeed = 3f;
    [SerializeField] LayerMask _crashDownMask;
    private Rigidbody2D _rigid;
    private Transform _parent;
    private bool _isAiming = false,_isFalling = false;

    private float _minorOffsetTimer = 1f;

    private EnemyDamageContact _enemyDamageContact;

    List<Collider2D> _ignoredColliders = new List<Collider2D>();

    protected override void Start()
    {
        base.Start();
        _dropTelegraph.SetActive(false);
        _parent = transform.parent;
        _rigid = _parent.GetComponent<Rigidbody2D>();
        _enemyDamageContact = _parent.GetComponent<EnemyDamageContact>();
        _enemyDamageContact.skill = _enemySkills[0];
    }

    public override void Interrupt()
    {
        if (_rigid.gravityScale <= 0f)
            _rigid.gravityScale = 1f;
        
        _isAiming = false;
        _isFalling = false;
        _dropTelegraph.SetActive(false);
        _anim.ResetTrigger("release");
        _anim.ResetTrigger("endAttack");
        _enemyDamageContact.EndAttack();

        base.Interrupt();
    }

    protected override void Recovery()
    {
        _anim.SetTrigger("endAttack");
        base.Recovery();
    }

    private void ReturnCollisions()
    {
        if (_ignoredColliders.Count > 0)
        {
            foreach(Collider2D col in _ignoredColliders)
            {
                Physics2D.IgnoreCollision(col,_hurtBox, false);
            }
        }
    }

    private void IsOnOneWayPlatform()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(_platformDetector.transform.position, 0.2f, _crashDownMask);

        if (colliders.Length <= 0)
            return;

        foreach(Collider2D col in colliders)
        {
            if (!Physics2D.GetIgnoreCollision(col,_hurtBox))
            {
                //Debug.Log("Colliding with and ignoring "+col.name);
                Physics2D.IgnoreCollision(col,_hurtBox, true);
                _ignoredColliders.Add(col);
            }
        }
    }

    protected override void SelectAttack()
    {
        if (InDistance(_meleeRange))
        {
            _canAttack = false;
            _attackIndex = 0;
            _knockBackVector = EnumLib.KnockbackVector(_enemySkills[_attackIndex].force);
        }
    }

    protected override IEnumerator ChargeTimers(Vector2 chargeTime, bool isWindUp)
    {
        _rigid.AddForce(new Vector2(0f,500f));
        _rigid.gravityScale = 0f;
        // _rigid.excludeLayers = _crashDownMask;

        yield return new WaitForSeconds(1f);
        _rigid.velocity = Vector2.zero;
        _isAiming = true;
        _dropTelegraph.SetActive(true);
        _dropTelegraph.GetComponent<SpriteRenderer>().color = Color.white;

        yield return new WaitForSeconds(chargeTime.x);
        _dropTelegraph.GetComponent<SpriteRenderer>().color = Color.red;
        _isAiming = false;

        yield return new WaitForSeconds(0.3f);
        _dropTelegraph.SetActive(false);
        _rigid.gravityScale = 1f;
        _rigid.AddForce(new Vector2(0f,-500f));
        _isFalling = true;
        _enemyDamageContact.StartAttack();
        _minorOffsetTimer = 0.5f;
    }

    void FixedUpdate()
    {
        if (_isAiming)
        {
            // Debug.Log("target's x position is ");
            _rigid.position = new Vector2(_rigid.position.x + (_target.position.x-_rigid.position.x)*Time.deltaTime *_trackSpeed,_parent.position.y);
        }

        if (_isFalling)
        {
            if ( _rigid.velocity.y <= 0f && _minorOffsetTimer <= 0f)
            {    
                Debug.Log("Ended falling");
                _isFalling = false;
                _anim.SetTrigger("release");
                ReturnCollisions();
                // _rigid.excludeLayers = _defaultMask;
            }
            else
                IsOnOneWayPlatform();
        }

        if (_minorOffsetTimer != 0f)
        {
            _minorOffsetTimer -= Time.deltaTime;
            if (_minorOffsetTimer <= 0f)
                _minorOffsetTimer = 0f;
        }
    }


    void Update()
    {
        if (_canAttack)
        {
            SelectAttack();
            if (_attackIndex != -1)
            {
                ChargingAttacks(2f,4f);
            }
        }
    }
}