using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToiletBossAI : BossCore
{   
    private Transform toiletTransform;
    private Transform leftSideTransform;
    private BoxCollider2D leftSideCollider;
    private Transform centerTransform;
    private BoxCollider2D centerCollider;
    private Transform rightSideTransform;
    private BoxCollider2D rightSideCollider;

    private Vector3 leftSide;
    private Vector3 rightSide;
    private Vector3 center;
    
    private float dashCooldown = 0.5f;
    private float phaseOneDashSpeed = 25f;
    private float phaseTwoDashSpeed = 50f;
    private float currentDashSpeed;
    private float phaseOneDamage = 15f;
    private float phaseTwoDamage = 25f;
    private float currentDamage;
    private bool isDashing = false;
    private int dashesRemaining;
    private int maxDashes = 3;
    
    private bool timeToPee = false;
    private bool onPhaseTwo = false;
    private bool _isReady = true;
    
    private float centerCooldown = 8f;
    private float playerHitCooldown = 1.5f;

    private bool justSpawned = true;
    private enum BossState
    {
        DashToLeft,
        DashToRight,
        DashToCenter
    }
    
    private BossState currentState = BossState.DashToLeft;
    private BossState stateBeforeHit;
    private BossState stateBeforeCenter;
    private Coroutine bossCoroutine;
    
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        
        _rb = GetComponent<Rigidbody2D>();
        
        toiletTransform = transform;
        
        var leftObject = GameObject.Find("LeftSide");
        leftSideTransform = leftObject.transform;
        leftSide = leftSideTransform.position;
        leftSideCollider = leftObject.GetComponent<BoxCollider2D>();
        
        var centerObject = GameObject.Find("center");
        centerTransform = centerObject.transform;
        center = centerTransform.position;
        centerCollider = centerObject.GetComponent<BoxCollider2D>();
        
        var rightObject = GameObject.Find("RightSide");
        rightSideTransform = rightObject.transform;
        rightSide = rightSideTransform.position;
        rightSideCollider = rightObject.GetComponent<BoxCollider2D>();
        
        dashesRemaining = maxDashes;
        currentDashSpeed = phaseOneDashSpeed;
        currentDamage = phaseOneDamage;
        
        _bossStats = GetComponent<EnemyStats>();
        
        bossCoroutine = StartCoroutine(StartBoss());
    }

    void Update()
    {
        if (_bossStats.healthRatio <= 0.5f)
        {
            onPhaseTwo = true;
            currentDashSpeed = phaseTwoDashSpeed;
            currentDamage = phaseTwoDamage;
        }
    }

    public override void Fire()
    {
        ProjectileManager.projectileArc?.Invoke(transform.position, new Vector2(-1f,1f), _moveSet[0]);
        ProjectileManager.projectileArc?.Invoke(transform.position, new Vector2(-0.5f,1f), _moveSet[0]);
        ProjectileManager.projectileArc?.Invoke(transform.position, new Vector2(1f,1f), _moveSet[0]);
        ProjectileManager.projectileArc?.Invoke(transform.position, new Vector2(0.5f,1f), _moveSet[0]);
    }

    IEnumerator StartBoss()
    {
        if (justSpawned)
        {
            yield return new WaitForSeconds(2f);
            justSpawned = false;
        }
        
        while (true)
        {
            // Fuck switch statements, all my homies hate switch statements
            if (currentState == BossState.DashToLeft)
            {
                SetUpAttack(1);
                yield return new WaitForSeconds(dashCooldown/2f);
                _anim.SetTrigger("release");
                MoveTo(leftSide, leftSideCollider);
                yield return new WaitWhile(() => isDashing);
                _anim.SetTrigger("recover");
                yield return new WaitForSeconds(dashCooldown);
                dashesRemaining--;
                if (dashesRemaining > 0)
                {
                    currentState = BossState.DashToRight;
                }
                else
                {
                    currentState = BossState.DashToCenter;
                    stateBeforeCenter = BossState.DashToLeft;
                }
            }
            else if (currentState == BossState.DashToRight)
            {
                SetUpAttack(1);
                yield return new WaitForSeconds(dashCooldown/2f);
                _anim.SetTrigger("release");
                MoveTo(rightSide, rightSideCollider);
                yield return new WaitWhile(() => isDashing);
                _anim.SetTrigger("recover");
                yield return new WaitForSeconds(dashCooldown);
                dashesRemaining--;
                if (dashesRemaining > 0)
                {
                    currentState = BossState.DashToLeft;
                }
                else
                {
                    currentState = BossState.DashToCenter;
                    stateBeforeCenter = BossState.DashToRight;
                }
            }
            else if (currentState == BossState.DashToCenter)
            {
                SetUpAttack(1);
                yield return new WaitForSeconds(dashCooldown/2f);
                _anim.SetTrigger("release");
                MoveTo(center, centerCollider);
                yield return new WaitWhile(() => isDashing);
                _anim.SetTrigger("recover");
                if (onPhaseTwo)
                {
                    // if (timeToPee)
                    // {
                        SetUpAttack(0);
                        yield return new WaitForSeconds(centerCooldown / 2);
                        _anim.SetTrigger("release");
                        yield return new WaitForSeconds(centerCooldown / 2);
                    // }
                    // else
                    // {
                        // yield return new WaitForSeconds(centerCooldown);
                    // }
                    // timeToPee = !timeToPee;
                }
                else
                {
                    _anim.Play("Asleep");
                    yield return new WaitForSeconds(centerCooldown);
                    _anim.SetTrigger("wakeUp");
                }
                dashesRemaining = maxDashes;
                currentState = stateBeforeCenter == BossState.DashToLeft ? BossState.DashToRight : BossState.DashToLeft;
            }
        }
    }

    IEnumerator HitPlayerCooldown()
    {
        yield return new WaitForSeconds(playerHitCooldown);
        if (stateBeforeHit == BossState.DashToLeft)
        {
            currentState = BossState.DashToRight;
        }
        else if (stateBeforeHit == BossState.DashToRight)
        {
            currentState = BossState.DashToLeft;
        }
        else if (stateBeforeHit == BossState.DashToCenter)
        {
            currentState = BossState.DashToCenter;
        }
        bossCoroutine = StartCoroutine(StartBoss());
    }
    
    void OnTriggerEnter2D(Collider2D col)
    {
        PlayerStats playerStat;
        Vector2 knockBackVector = EnumLib.KnockbackVector(EnumLib.KnockBackPower.SideLaunch);
    
        if (col.CompareTag("Player") && isDashing)
        {
            // Physics2D.IgnoreCollision(col, _toiletCollider, true);
            // StartCoroutine(ReenableCollision(col));
            Debug.Log("Hit the janitor!");
            Debug.Log("isDashing: " + isDashing + ", current state: " + currentState);
            playerStat = col.GetComponent<PlayerStats>();
            if (!playerStat.iFrame)
            {
                Debug.Log("Vroom vroom you fool! Toilet gonna give it to ya");
                Vector2 direction = (col.transform.position - toiletTransform.position).normalized;
                col.GetComponent<PlayerInterrupt>().Stagger(1,knockBackVector * direction.x * 0.5f);
                playerStat.DamageCalc(currentDamage, EnumLib.DamageType.Wet,false);
                CameraFollow.StartShake?.Invoke();
            }
            else
            {
                Debug.Log("Under Iframes");
            }
    
            _rb.velocity = Vector2.zero;
            isDashing = false;
            stateBeforeHit = currentState;
            StopCoroutine(bossCoroutine);
            StartCoroutine(HitPlayerCooldown());
        }
    }
    
    void OnTriggerStay2D(Collider2D col)
    {
        PlayerStats playerStat;
        Vector2 knockBackVector = EnumLib.KnockbackVector(EnumLib.KnockBackPower.SideLaunch);
    
        if (col.CompareTag("Player") && isDashing)
        {
            // Physics2D.IgnoreCollision(col, _toiletCollider, true);
            // StartCoroutine(ReenableCollision(col));
            Debug.Log("Hit the janitor!");
            Debug.Log("isDashing: " + isDashing + ", current state: " + currentState);
            playerStat = col.GetComponent<PlayerStats>();
            if (!playerStat.iFrame)
            {
                Debug.Log("Vroom vroom you fool! Toilet gonna give it to ya");
                Vector2 direction = (col.transform.position - toiletTransform.position).normalized;
                col.GetComponent<PlayerInterrupt>().Stagger(1,knockBackVector * direction.x);
                playerStat.DamageCalc(currentDamage, EnumLib.DamageType.Wet,false);
                col.gameObject.GetComponent<DamageEffect>().TriggerEffect((int)EnumLib.DamageType.Wet);
                CameraFollow.StartShake?.Invoke();
            }
            else
            {
                Debug.Log("Under Iframes");
            }
    
            _rb.velocity = Vector2.zero;
            isDashing = false;
            stateBeforeHit = currentState;
            StopCoroutine(bossCoroutine);
            StartCoroutine(HitPlayerCooldown());
        }
    }
    
    void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            Physics2D.IgnoreCollision(col, _toiletCollider, false);
        }
    }

    private IEnumerator ReenableCollision(Collider2D col)
    {
        yield return new WaitForSeconds(1f);
        Physics2D.IgnoreCollision(col, _toiletCollider, false);
    }

    private void MoveTo(Vector3 targetPosition, BoxCollider2D targetCollider)
    {
        isDashing = true;
        Vector3 direction = (targetPosition - toiletTransform.position).normalized;
        _rb.velocity = new Vector2(direction.x * currentDashSpeed, _rb.velocity.y);
        Flip();
        StartCoroutine(CheckIfDashingComplete(targetCollider));
    }
    
    private IEnumerator CheckIfDashingComplete(BoxCollider2D targetCollider)
    {
        while (!_toiletCollider.IsTouching(targetCollider))
        {
            yield return null;
        }
        
        print("dashing complete!");
        _rb.velocity = Vector2.zero;
        isDashing = false;
    }

    protected override void Flip()
    {
        if (_rb.velocity.x > 0f)
        {
            toiletTransform.localScale = new Vector3(Mathf.Abs(toiletTransform.localScale.x), toiletTransform.localScale.y, toiletTransform.localScale.z);
        }
        else if (_rb.velocity.x < 0f)
        {
            toiletTransform.localScale = new Vector3(-1f * Mathf.Abs(toiletTransform.localScale.x), toiletTransform.localScale.y, toiletTransform.localScale.z);
        }
    }
}
