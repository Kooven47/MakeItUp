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
    private float timeInAir;

    private bool _canMove = true, _isSprinting = false;
    [SerializeField] private Vector2 counterJumpForce;
    
    [SerializeField] private float coyoteTime;
    private float coyoteTimeCounter;

    [SerializeField] private float jumpBufferTime;
    private float jumpBufferTimeCounter;

    [SerializeField] private float airDashTime;

    private bool isWallSliding;
    private float wallSlidingSpeed = 0.5f;

    private int maxAirDashes = 1;
    private int airDashesRemaining;
    private bool isAirDashing;
    
    private bool isWallJumping;
    private float wallJumpingDirection;
    private float wallJumpingTime = 0.2f;
    private float wallJumpingTimeCounter;
    private float wallJumpingDuration = 0.4f;
    private Vector2 wallJumpingPower = new Vector2(8f, 8f);
    
    public static EnemyAI[] enemyAIList;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask platformLayer;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private Animator _anim;
    
    [Header("Sound Effects")]
    private const int JUMP = 0,DROP = 1,LAND = 2,WALK = 3,RUN = 4;
    [SerializeField] private AudioClip[] _soundEffects = new AudioClip[5];
    [SerializeField] private AudioSource _audioSrc, _moveAudio;
 
    // public int maxJumpTrackNum = 3; // This is the number of jump history you want to keep track of for the ai to follow 
    // Start is called before the first frame update
    private void Start()
    {
        speed = normalSpeed;
        _moveAudio.clip = _soundEffects[WALK];
        PlayerInterrupt.staggered += SetMobility;
    }

    private void PlaySoundEffect(int index)
    {
        _audioSrc.clip = _soundEffects[index];
        _audioSrc.Play();
    }

    public void SetMobility(bool isMobile)
    {
        _canMove = isMobile;
    }

    // Update is called once per frame
    private void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        
        // Check if Shift key is held down to increase speed
        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed = sprintingSpeed;
            _isSprinting = true;
        }
        else
        {
            speed = normalSpeed;
            _isSprinting = false;
        }
        
        // Handle footsteps
        if ((IsGrounded() || IsOnOneWayPlatform()) && (Time.timeScale != 0))
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
            timeInAir += Time.deltaTime;
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
            if (!isGrounded && timeInAir >= 0.5f)
            {
                PlaySoundEffect(LAND);
            }
            isGrounded = true;
            timeInAir = 0f;
            if (Math.Abs(horizontal) > 0f)
            {
                // walkingSound.enabled = speed == normalSpeed;
                // sprintingSound.enabled = speed == sprintingSpeed;
                _moveAudio.enabled = true;
                if (_isSprinting)
                {
                    _moveAudio.clip = _soundEffects[RUN];
                }
                else
                {
                    _moveAudio.clip = _soundEffects[WALK];
                }

                if (!_moveAudio.isPlaying)
                {
                    _moveAudio.Play();
                }
            }
            else
                _moveAudio.enabled = false;
        }
        else
        {
            isGrounded = false;
            _moveAudio.enabled = false;
        }
        
        // Handle jump
        if (jumpBufferTimeCounter > 0f && !isJumping)
        {
            jumpKeyHeld = true;
            if (coyoteTimeCounter > 0f)
            {
                // This keeps track of the jumps for the a* platforming
                if (enemyAIList != null)
                {
                    foreach (EnemyAI enemy in enemyAIList)
                    {
                        enemy.newJumpPosition(rb.position);
                    }
                }
                if (Time.timeScale != 0)
                    PlaySoundEffect(JUMP);
                rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
                jumpBufferTimeCounter = 0f;
                StartCoroutine(JumpCooldown());
            }
            
            // Make jump last longer on hold
            if (rb.velocity.y > 0f && !isWallJumping)
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
            PlaySoundEffect(DROP);
        }

        WallSlide();
        WallJump();
        AirDash();

        if (!isWallJumping && !isAirDashing)
        {
            Flip();
        }
    }

    private void FixedUpdate()
    {
        if (_canMove)
            Movement();
    }

    private void Movement()
    {
        _anim.SetBool("isMoving",horizontal != 0f);

        if (!isWallJumping && !isAirDashing)
        {
            rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
        }

        if (!IsGrounded() && !IsOnOneWayPlatform() && !isAirDashing)
        {
            if (!jumpKeyHeld && Vector2.Dot(rb.velocity, Vector2.up) > 0)
            {
                rb.AddForce(counterJumpForce * rb.mass);
            }
        }
    }

    private bool IsGrounded()
    {
        bool isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
        _anim.SetBool("onGround",isGrounded);
        return isGrounded;
    }

    private bool IsOnOneWayPlatform()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, platformLayer);
    }
    
    private bool IsWalled()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer);
    }

    private void WallSlide()
    {
        if (IsWalled() && !IsGrounded() && !IsOnOneWayPlatform() && horizontal != 0f)
        {
            isWallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void WallJump()
    {
        if (isWallSliding)
        {
            isWallJumping = false;
            wallJumpingDirection = -transform.localScale.x;
            wallJumpingTimeCounter = wallJumpingTime;

            CancelInvoke(nameof(StopWallJumping));
        }
        else
        {
            wallJumpingTimeCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump") && wallJumpingTimeCounter > 0f)
        {
            isWallJumping = true;
            rb.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
            wallJumpingTimeCounter = 0f;

            if (transform.localScale.x != wallJumpingDirection)
            {
                isFacingRight = !isFacingRight;
                Vector3 localScale = transform.localScale;
                localScale.x *= -1f;
                transform.localScale = localScale;
            }

            Invoke(nameof(StopWallJumping), wallJumpingDuration);
        }
    }
    
    private void StopWallJumping()
    {
        isWallJumping = false;
    }

    private void AirDash()
    {
        if (IsGrounded() || IsOnOneWayPlatform() || IsWalled())
        {
            airDashesRemaining = maxAirDashes;
            isAirDashing = false;
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.LeftShift) && (Time.timeScale != 0))
            {
                if (airDashesRemaining > 0)
                {
                    airDashesRemaining--;
                    isAirDashing = true;
                    StartCoroutine(AirDashTime());
                }
            }
        }
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

    private IEnumerator AirDashTime()
    {
        rb.velocity = Vector2.zero;
        rb.gravityScale = 0f;
        rb.velocity = new Vector2(10 * transform.localScale.x, 0);
        yield return new WaitForSeconds(airDashTime);
        rb.gravityScale = 1.5f;
        isAirDashing = false;
    }

    private void Flip()
    {
        if ((isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f) && (Time.timeScale != 0))
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }
}
