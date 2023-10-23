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
    private Vector3 leftSide;
    private Vector3 rightSide;
    private Vector3 center;
    
    private float timeSinceLastDash = 0f;
    private float dashCooldown = 0.1f;
    private float dashSpeed = 50f;
    private bool isDashing = false;
    private int dashesRemaining;
    private int maxDashes = 3;
    
    private bool isMovingRight = true;
    private bool isMovingToCenter = false;

    private bool timeToPee = false;
    private bool onPhaseTwo = false;
    
    private float centerCooldown = 5f;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        leftSide = leftSideTransform.position;
        rightSide = rightSideTransform.position;
        center = centerTransform.position;
        dashesRemaining = maxDashes;
    }

    // Update is called once per frame
    void Update()
    {
        // onPhaseTwo = _curHP <= _maxHP / 2;
        if (isDashing || isMovingToCenter) return;

        if (dashesRemaining > 0)
        {
            var targetPosition = isMovingRight ? rightSide : leftSide;
            var targetCollider = isMovingRight ? rightSideCollider : leftSideCollider;
            MoveTo(targetPosition);
            Flip();
            if (toiletCollider.IsTouching(targetCollider)) timeSinceLastDash += Time.deltaTime;
            if (timeSinceLastDash >= dashCooldown) DoDashAttack();
        }
        else
        {
            StartCoroutine(MoveToCenterAndCooldown());
        }
    }

    private void MoveTo(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - toiletTransform.position).normalized;
        rb.velocity = new Vector2(direction.x * dashSpeed, rb.velocity.y);
    }

    private void Flip()
    {
        if (rb.velocity.x > 0f)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (rb.velocity.x < 0f)
        {
            transform.localScale = new Vector3(-1f * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    private void DoDashAttack()
    {
        print("Dash ended!");
        isDashing = true;
        dashesRemaining--;        

        isMovingRight = !isMovingRight;
        
        rb.velocity = Vector2.zero;
        isDashing = false;
        timeSinceLastDash = 0f;
    }

    private IEnumerator MoveToCenterAndCooldown()
    {
        isMovingToCenter = true;
        while (!toiletCollider.IsTouching(centerCollider))
        {
            MoveTo(center);
            Flip();
            yield return null;
        }

        rb.velocity = Vector2.zero;
        
        // Phase 2
        if (onPhaseTwo)
        {
            if (timeToPee)
            {
                // Continuous arch for 2 seconds to side of player, as soon as it starts touching ground, spawns slippery toxic ground, will be different layer with different sprite
            }
            else
            {
                yield return new WaitForSeconds(centerCooldown);
            }

            timeToPee = !timeToPee;
        }
        // Phase 1
        else
        {
            yield return new WaitForSeconds(centerCooldown);
        }
        
        dashesRemaining = maxDashes;
        isMovingToCenter = false;
        print("Finished waiting at center, dashes refilled!");
    }
}
