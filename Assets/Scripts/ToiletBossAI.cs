using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToiletBossAI : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] private Transform toilet;
    [SerializeField] private Transform leftSideTransform;
    [SerializeField] private Transform rightSideTransform;
    private Vector3 leftSide;
    private Vector3 rightSide;
    private Vector3 center;
    
    private float timeSinceLastDash = 0f;
    private float dashCooldown = 1f;
    private float dashSpeed = 50f;
    private bool isDashing = false;
    private int dashesRemaining;
    private int maxDashes = 4;
    
    private bool isMovingRight = true;
    private bool isMovingToCenter = false;

    private float centerCooldown = 5f;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        leftSide = leftSideTransform.position;
        rightSide = rightSideTransform.position;
        center = (rightSide + leftSide) / 2f;
        dashesRemaining = maxDashes;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDashing || isMovingToCenter) return;

        if (dashesRemaining > 0)
        {
            var targetPosition = isMovingRight ? rightSide : leftSide;
            MoveTo(targetPosition);
            Flip();
            timeSinceLastDash += Time.deltaTime;
            if (timeSinceLastDash >= dashCooldown) StartCoroutine(DoDashAttack());
        }
        else
        {
            StartCoroutine(MoveToCenterAndCooldown());
        }
    }

    private void MoveTo(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - toilet.position).normalized;
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

    private IEnumerator DoDashAttack()
    {
        print("Dash triggered!");
        isDashing = true;
        dashesRemaining--;        
        yield return new WaitForSeconds(1.0f);

        isMovingRight = !isMovingRight;
        
        rb.velocity = Vector2.zero;
        isDashing = false;
        timeSinceLastDash = 0f;
    }

    private IEnumerator MoveToCenterAndCooldown()
    {
        isMovingToCenter = true;
        while (Math.Abs(toilet.position.x - center.x) > 0.1f)
        {
            MoveTo(center);
            Flip();
            yield return null;
        }

        rb.velocity = Vector2.zero;
        
        yield return new WaitForSeconds(centerCooldown);
        dashesRemaining = maxDashes;
        isMovingToCenter = false;
        print("Finished waiting at center, dashes refilled!");
    }
}
