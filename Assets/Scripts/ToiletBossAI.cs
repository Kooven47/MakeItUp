using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToiletBossAI : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] private BoxCollider2D toiletCollider;
    [SerializeField] private Transform toiletTransform;
    [SerializeField] private Transform leftSideTransform;
    [SerializeField] private BoxCollider2D leftSideCollider;
    [SerializeField] private Transform centerTransform;
    [SerializeField] private BoxCollider2D centerCollider;
    [SerializeField] private Transform rightSideTransform;
    [SerializeField] private BoxCollider2D rightSideCollider;

    [SerializeField] private List<EnemyAbility> _moveSet = new List<EnemyAbility>();
    private EnemyStats toiletStats;
    private Vector3 leftSide;
    private Vector3 rightSide;
    private Vector3 center;
    
    private float dashCooldown = 0.5f;
    private float phaseOneDashSpeed = 25f;
    private float phaseTwoDashSpeed = 50f;
    private float currentDashSpeed;
    private float phaseOneDamage = 10f;
    private float phaseTwoDamage = 20f;
    private float currentDamage;
    private bool isDashing = false;
    private int dashesRemaining;
    private int maxDashes = 3;
    
    private bool timeToPee = false;
    private bool onPhaseTwo = false;
    
    private float centerCooldown = 5f;
    private float playerHitCooldown = 3f;
    private enum BossState
    {
        DashToLeft,
        DashToRight,
        DashToCenter
    }
    
    private BossState currentState = BossState.DashToLeft;
    private BossState stateBeforeHit;
    private Coroutine bossCoroutine;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        leftSide = leftSideTransform.position;
        rightSide = rightSideTransform.position;
        center = centerTransform.position;
        dashesRemaining = maxDashes;
        currentDashSpeed = phaseOneDashSpeed;
        currentDamage = phaseOneDamage;
        toiletStats = GetComponent<EnemyStats>();
        bossCoroutine = StartCoroutine(StartBoss());
    }

    void Update()
    {
        if (toiletStats.healthRatio <= 1f)
        {
            onPhaseTwo = true;
            currentDashSpeed = phaseTwoDashSpeed;
            currentDamage = phaseTwoDamage;
        }
    }

    IEnumerator StartBoss()
    {
        while (true)
        {
            // Fuck switch statements, all my homies hate switch statements
            if (currentState == BossState.DashToLeft)
            {
                MoveTo(leftSide, leftSideCollider);
                yield return new WaitWhile(() => isDashing);
                yield return new WaitForSeconds(dashCooldown);
                dashesRemaining--;
                currentState = dashesRemaining > 0 ? BossState.DashToRight : BossState.DashToCenter;
            }
            else if (currentState == BossState.DashToRight)
            {
                MoveTo(rightSide, rightSideCollider);
                yield return new WaitWhile(() => isDashing);
                yield return new WaitForSeconds(dashCooldown);
                dashesRemaining--;
                currentState = dashesRemaining > 0 ? BossState.DashToLeft : BossState.DashToCenter;
            }
            else if (currentState == BossState.DashToCenter)
            {
                MoveTo(center, centerCollider);
                yield return new WaitWhile(() => isDashing);
                if (onPhaseTwo)
                {
                    if (timeToPee)
                    {
                        Debug.Log("Time to pee!");
                        ProjectileManager.projectileArc?.Invoke(transform.position,new Vector2(-1f,1f),_moveSet[0]);
                        ProjectileManager.projectileArc?.Invoke(transform.position,new Vector2(-0.5f,1f),_moveSet[0]);
                        ProjectileManager.projectileArc?.Invoke(transform.position,new Vector2(1f,1f),_moveSet[0]);
                        ProjectileManager.projectileArc?.Invoke(transform.position,new Vector2(0.5f,1f),_moveSet[0]);
                        yield return new WaitForSeconds(centerCooldown * 2);
                    }
                    else
                    {
                        yield return new WaitForSeconds(centerCooldown);
                    }

                    timeToPee = !timeToPee;
                }
                else
                {
                    yield return new WaitForSeconds(centerCooldown);
                }
                dashesRemaining = maxDashes;
                currentState = BossState.DashToLeft;
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
            Physics2D.IgnoreCollision(col, toiletCollider, true);
            StartCoroutine(ReenableCollision(col));
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
    
            rb.velocity = Vector2.zero;
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
            Physics2D.IgnoreCollision(col, toiletCollider, false);
        }
    }

    private IEnumerator ReenableCollision(Collider2D col)
    {
        yield return new WaitForSeconds(1f);
        Physics2D.IgnoreCollision(col, toiletCollider, false);
    }

    private void MoveTo(Vector3 targetPosition, BoxCollider2D targetCollider)
    {
        isDashing = true;
        Vector3 direction = (targetPosition - toiletTransform.position).normalized;
        rb.velocity = new Vector2(direction.x * currentDashSpeed, rb.velocity.y);
        Flip();
        StartCoroutine(CheckIfDashingComplete(targetCollider));
    }
    
    private IEnumerator CheckIfDashingComplete(BoxCollider2D targetCollider)
    {
        while (!toiletCollider.IsTouching(targetCollider))
        {
            yield return null;
        }
        
        print("dashing complete!");
        rb.velocity = Vector2.zero;
        isDashing = false;
    }

    private void Flip()
    {
        if (rb.velocity.x > 0f)
        {
            toiletTransform.localScale = new Vector3(Mathf.Abs(toiletTransform.localScale.x), toiletTransform.localScale.y, toiletTransform.localScale.z);
        }
        else if (rb.velocity.x < 0f)
        {
            toiletTransform.localScale = new Vector3(-1f * Mathf.Abs(toiletTransform.localScale.x), toiletTransform.localScale.y, toiletTransform.localScale.z);
        }
    }
}
