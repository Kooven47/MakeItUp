using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.Serialization;

public class PlayerControllerJanitor : MonoBehaviour
{
    private float horizontal;
    private float speed;

    private float _dashIframe = 0.25f;
    [SerializeField] private float normalSpeed;
    [SerializeField] private float sprintingSpeed;
    [SerializeField] private float jumpingPower;
    private bool isFacingRight = true;
    private bool jumpKeyHeld;
    private bool isJumping;
    private bool isGrounded;
    private float timeInAir;

    private bool _isStunned = false;

    private bool _canMove = true, _isSprinting = true;
    [SerializeField] private Vector2 counterJumpForce;
    
    [SerializeField] private float coyoteTime;
    private float coyoteTimeCounter;

    [SerializeField] private float jumpBufferTime;
    private float jumpBufferTimeCounter;

    [SerializeField] private float airDashTime;

    private bool isWallSliding;
    private float wallSlidingSpeed = 0.5f;

    private bool isWallJumping;
    private float wallJumpingDirection;
    private float wallJumpingTime = 0.2f;
    private float wallJumpingTimeCounter;
    private float wallJumpingDuration = 0.4f;
    private Vector2 wallJumpingPower = new Vector2(8f, 8f);
    
    // For detecting double press for dash
    float delayBetweenPresses = 0.25f;
    bool pressedFirstTime = false;
    float lastPressedTime;
    
    private int maxAirDashes = 1;
    private int airDashesRemaining;
    private bool isDashing;
    [SerializeField] private bool canAirDash;

    private int jumpCount;
    [SerializeField] private int maxAirJumpCount = 1;
    
    public static EnemyAI[] enemyAIList;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private LayerMask platformLayer;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private LayerMask obstacleWallLayer;
    [SerializeField] private Animator _anim;

    [Header("Sound Effects")] private const int JUMP = 0, DROP = 1, LAND = 2, WALK = 3, RUN = 4, FART = 10;
    public AudioClip[] _soundEffects = new AudioClip[5];
    public AudioSource _audioSrc, _moveAudio;
    public static event Action InitializeSound;

    private Rigidbody2D[] _fartClouds = new Rigidbody2D[2];
    private Coroutine _jumpFartTimer = null;
    // public int maxJumpTrackNum = 3; // This is the number of jump history you want to keep track of for the ai to follow 
    // Start is called before the first frame update
    private void Start()
    {
        InitializeSound?.Invoke();
        speed = sprintingSpeed;
        _moveAudio.clip = _soundEffects[RUN];
        PlayerInterrupt.staggered += SetMobility;
        _fartClouds[0] = transform.GetChild(5).GetComponent<Rigidbody2D>();
        _fartClouds[1] = transform.GetChild(6).GetComponent<Rigidbody2D>();
    }
    private void OnDisable()
    {
        PlayerInterrupt.staggered -= SetMobility;
    }
    public void PlaySoundEffect(int index)
    {
        _audioSrc.clip = _soundEffects[index];
        _audioSrc.Play();
    }

    public void SetMobility(bool isMobile)
    {
        _canMove = isMobile;
    }

    public bool SetStunned(bool isStun)
    {
        if (isStun && !_isStunned)
        {
            _isStunned = true;
            return true;
        }
        else if (!isStun)
        {
            _isStunned = false;
        }

        return false;
    }

    // Update is called once per frame
    private void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        _anim.SetBool("onGround", IsGrounded() || IsOnOneWayPlatform());
        // Check for ctrl key to toggle between sprint or walk
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (_isSprinting)
            {
                speed = normalSpeed;
                _isSprinting = false;
            }
            else
            {
                speed = sprintingSpeed;
                _isSprinting = true;
            }
        }
        
        // Handle footsteps
        if ((IsGrounded() || IsOnOneWayPlatform()) && (Time.timeScale != 0))
        {
            coyoteTimeCounter = coyoteTime;
            if (_jumpFartTimer != null)
            {
                StopCoroutine(_jumpFartTimer);
                _jumpFartTimer = null;
                SetFartJump(false);
            }
            jumpCount = 0;
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
            if (Math.Abs(horizontal) > 0f && Math.Abs(rb.velocity.x) > 0f && Time.timeScale != 0f)
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
        if (jumpBufferTimeCounter > 0f && !isJumping && !_isStunned)
        {
            jumpKeyHeld = true;
            if (!isDashing && (coyoteTimeCounter > 0f || jumpCount < maxAirJumpCount))
            {
                // This keeps track of the jumps for the a* platforming
                if (enemyAIList != null)
                {
                    foreach (EnemyAI enemy in enemyAIList)
                    {
                        enemy.newJumpPosition(rb.position);
                    }
                }
                
                rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
                jumpBufferTimeCounter = 0f;
                StartCoroutine(JumpCooldown());
                
                if (Time.timeScale != 0)
                {
                    if (jumpCount == 0 && !isGrounded)
                    {
                        PlaySoundEffect(FART);
                        if (_jumpFartTimer == null)
                            _jumpFartTimer = StartCoroutine(FartCloudJumpTimer());
                    }
                    else PlaySoundEffect(JUMP);
                }
                
                jumpCount++;
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
        if ((Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) && IsOnOneWayPlatform() && !_isStunned)
        {
            StartCoroutine(DisablePlatformCollision());
            PlaySoundEffect(DROP);
        }

        WallSlide();
        WallJump();
        GroundAndAirDash();

        if (!isWallJumping && !isDashing)
        {
            Flip();
        }
    }

    private void FixedUpdate()
    {
        if (_canMove && !_isStunned)
            Movement();
    }

    private void Movement()
    {
        _anim.SetBool("isMoving",horizontal != 0f || (isGrounded && isDashing));

        if (!isWallJumping && !isDashing && !isWallSliding)
        {
            rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
        }

        if (!IsGrounded() && !IsOnOneWayPlatform() && !isDashing)
        {
            if (!jumpKeyHeld && Vector2.Dot(rb.velocity, Vector2.up) > 0)
            {
                rb.AddForce(counterJumpForce * rb.mass);
            }
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer | obstacleLayer);
    }

    private bool IsOnOneWayPlatform()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, platformLayer);
    }
    
    private bool IsWalled()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer | obstacleWallLayer);
    }

    private void WallSlide()
    {
        if (IsWalled() && !IsGrounded() && !IsOnOneWayPlatform() && horizontal != 0f)
        {
            isWallSliding = true;
            if (_jumpFartTimer != null)
            {
                StopCoroutine(_jumpFartTimer);
                _jumpFartTimer = null;
                SetFartJump(false);
            }
            jumpCount = 0;
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
            if (Time.timeScale != 0) PlaySoundEffect(JUMP);
            rb.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
            if (_jumpFartTimer != null)
            {
                StopCoroutine(_jumpFartTimer);
                _jumpFartTimer = null;
                SetFartJump(false);
            }
            jumpCount = 0; // maybe a mistake lol
            StartCoroutine(JumpCooldown(wallJumpingDuration));
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
    
    bool KeyPressedTwice(KeyCode keypress)
    {
        if (Input.GetKeyDown(keypress))
        {
            if (pressedFirstTime)
            {
                bool isDoublePress = Time.time - lastPressedTime <= delayBetweenPresses;
 
                if (isDoublePress)
                {
                    return true;
                }
            }
            else 
            {
                pressedFirstTime = true;
            }
 
            lastPressedTime = Time.time;
        }
                
        if (pressedFirstTime && Time.time - lastPressedTime > delayBetweenPresses)
        {
            pressedFirstTime = false;
            return false;
        }

        return false;
    }
    
    private void GroundAndAirDash()
    {
        if (IsGrounded() || IsOnOneWayPlatform() || IsWalled() && !_isStunned)
        {
            airDashesRemaining = maxAirDashes;

            if (Time.timeScale != 0 && !isDashing && Input.GetKeyDown(KeyCode.LeftShift))
            {
                PlaySoundEffect(FART);
                isDashing = true;
                StartCoroutine(DoDash());
                PlayerStats.dashIFrame(_dashIframe);
            }
        }
        else if (!_isStunned)
        {
            if (canAirDash && airDashesRemaining > 0 && !isDashing && Time.timeScale != 0 && Input.GetKeyDown(KeyCode.LeftShift))
            {
                PlaySoundEffect(FART);
                airDashesRemaining--;
                isDashing = true;
                StartCoroutine(DoDash());
                PlayerStats.dashIFrame(_dashIframe);
            }
        }
    }

    private void SetFartJump(bool setFart)
    {
        _fartClouds[0].gameObject.SetActive(setFart);
        if (setFart)
        {
            _fartClouds[0].transform.localPosition = Vector2.zero;
            _fartClouds[0].AddForce(new Vector2(0f,-300f));
        }
    }

    private IEnumerator FartCloudJumpTimer()
    {
        SetFartJump(true);
        yield return new WaitForSeconds(0.5f);
        SetFartJump(false);
        _jumpFartTimer = null;
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

    private IEnumerator JumpCooldown(float cooldown = 0.4f)
    {
        isJumping = true;
        yield return new WaitForSeconds(cooldown);
        isJumping = false;
    }

    private IEnumerator DoDash()
    {
        rb.velocity = Vector2.zero;
        rb.gravityScale = 0f;
        rb.velocity = new Vector2(10 * transform.localScale.x, 0);

        _fartClouds[1].gameObject.SetActive(true);
        _fartClouds[1].transform.localPosition = new Vector2(0f,0.35f);
        _fartClouds[1].AddForce(new Vector2(transform.localScale.x * -1 * 500f,0f));

        yield return new WaitForSeconds(airDashTime);
        _fartClouds[1].gameObject.SetActive(false);
        rb.gravityScale = 1.5f;
        isDashing = false;
    }

    private void Flip()
    {
        if ((isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f) && Time.timeScale != 0)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }
}
