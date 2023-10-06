using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class PlayerControllerJanitor : MonoBehaviour
{
    private float horizontal;
    private float speed;
    [SerializeField] private float normalSpeed;
    [SerializeField] private float sprintingSpeed;
    [SerializeField] private float jumpingPower;
    private bool isFacingRight = true;
    private bool jumpKeyHeld;
    private bool isJumping;
    private bool isGrounded;
    [SerializeField] private Vector2 counterJumpForce;
    
    [SerializeField] private float coyoteTime;
    private float coyoteTimeCounter;

    [SerializeField] private float jumpBufferTime;
    private float jumpBufferTimeCounter;
    
    private EnemyAI[] enemyAIList;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask platformLayer;
    [SerializeField] private Animator _anim;
    
    [Header("Sound Effects")]
    [SerializeField] private AudioSource jumpSoundEffect;
    [SerializeField] private AudioSource dropThroughSoundEffect;
    [SerializeField] private AudioSource landingSound;
    [SerializeField] private AudioSource walkingSound;
    [SerializeField] private AudioSource sprintingSound;
 
    // public int maxJumpTrackNum = 3; // This is the number of jump history you want to keep track of for the ai to follow 
    // Start is called before the first frame update
    private void Start()
    {
        enemyAIList = FindObjectsOfType<EnemyAI>();
        speed = normalSpeed;
    }

    // Update is called once per frame
    private void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        
        // Check if Shift key is held down to increase speed
        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed = sprintingSpeed;
        }
        else
        {
            speed = normalSpeed;
        }
        
        // Coyote Time
        if (IsGrounded() || IsOnOneWayPlatform())
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }
        
        // Jump Buffer
        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferTimeCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferTimeCounter -= Time.deltaTime;
        }
        
        // Handle footsteps
        if (coyoteTimeCounter > 0f)
        {
            // Just landed
            if (isGrounded == false)
            {
                // TODO: FIX SO ONLY HAPPENS WHEN LANDING FROM GREAT HEIGHT, USE TIME COUNTER
                landingSound.Play();
            }
            isGrounded = true;
            
            if (Math.Abs(horizontal) > 0f)
            {
                walkingSound.enabled = speed == normalSpeed;
                sprintingSound.enabled = speed == sprintingSpeed;
            }
            else
            {
                walkingSound.enabled = false;
                sprintingSound.enabled = false;
            }
        }
        else
        {
            isGrounded = false;
            walkingSound.enabled = false;
            sprintingSound.enabled = false;
        }
        
        if (jumpBufferTimeCounter > 0f && !isJumping)
        {
            jumpKeyHeld = true;
            if (coyoteTimeCounter > 0f)
            {
                // This keeps track of the jumps for the a* platforming
                foreach (EnemyAI enemy in enemyAIList)
                {
                    enemy.newJumpPosition(rb.position);
                }
                
                jumpSoundEffect.Play();
                rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
                jumpBufferTimeCounter = 0f;
                StartCoroutine(JumpCooldown());
            }

            if (rb.velocity.y > 0f)
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
                coyoteTimeCounter = 0f;
            }
        }
        else if (Input.GetButtonUp("Jump"))
        {
            jumpKeyHeld = false;
        }
        
        // One way platforms
        if (Input.GetKeyDown(KeyCode.S) && IsOnOneWayPlatform())
        {
            StartCoroutine(DisablePlatformCollision());
            dropThroughSoundEffect.Play();
        }

        Flip();
    }

    private void FixedUpdate()
    {
        _anim.SetBool("isMoving",horizontal != 0f);
        
        rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
        if (!IsGrounded() && !IsOnOneWayPlatform())
        {
            if (!jumpKeyHeld && Vector2.Dot(rb.velocity, Vector2.up) > 0)
            {
                rb.AddForce(counterJumpForce * rb.mass);
            }
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private bool IsOnOneWayPlatform()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, platformLayer);
    }

    private IEnumerator DisablePlatformCollision()
    {
        var playerCollider = rb.GetComponent<Collider2D>();
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, 0.2f, platformLayer);
        foreach (Collider2D collider in colliders)
        {
            Physics2D.IgnoreCollision(playerCollider, collider, true);
        }
        yield return new WaitForSeconds(0.5f);
        foreach (Collider2D collider in colliders)
        {
            Physics2D.IgnoreCollision(playerCollider, collider, false);
        }
    }
    private IEnumerator JumpCooldown()
    {
        isJumping = true;
        yield return new WaitForSeconds(0.4f);
        isJumping = false;
    }

    private void Flip()
    {
        if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }
}
