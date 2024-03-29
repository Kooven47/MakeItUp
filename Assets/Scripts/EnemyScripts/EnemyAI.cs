using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask platformLayer;
    [SerializeField] private EnemyCore _enemyCore;

    [Header("Pathfinding")]
    public Transform target;
    [SerializeField] private String _targetTag = "Player";
    public float activateDistance = 50f;
    public float pathUpdateSeconds = 0.5f;
    public float stuckDelay = 0f; // This is the delay to skip to the next path if the enemy is stuck when attempting a player history jump
    public int maxJumpTrackNum = 3;
    [SerializeField] private float _playerBubbleDistance = 1f;
    [SerializeField] private float _enemyBubbleDistance = 1f;
    [SerializeField] public bool pathfind = true;
    [Header("Physics")]
    public float minDropAngle = 240;
    public float maxDropAngle = 300;
    [SerializeField] private float dropthroughTime = 0.5f;
    public float speed = 200f;
    public float nextWaypointDistance = 3f;
    public float jumpNodeHeightRequirement = 0.8f;
    public float jumpModifier = 0.3f;
    public float jumpCheckOffset = 0.1f;
    public float jumpDelay = 0.0f;
    public float groundCheckSize = 0.31f; // 0.31f allowed the groundcheck to match with the spaghetti monster collider. Adjust for other enemies.

    [Header("Stuck Behavior")]
    [SerializeField] private bool _stuckHelp = false;
    public float stuckForce = 300f; // Force to apply when stuck
    public float stuckVelocityThreshold = 0.1f; // Velocity threshold to consider if stuck
    public float stuckTimeThreshold = 2f; // Time threshold to consider if stuck
    private float stuckTimer = 0f; // Timer to track how long the enemy is stuck

    [Header("Custom Behavior")]
    public bool followEnabled = true;
    public bool jumpEnabled = true;
    public bool directionLookEnabled = true;
    public bool flyingEnabled = false;

    private Path path;
    private int currentWaypoint = 0;
    bool isGrounded = false;
    bool IsOnOneWayPlatform = false;
    Seeker seeker;
    Rigidbody2D rb;
    float time = 0f;
    float timeSincePathStart = 0f;
    private bool isFollowingJumpPath = false;
    private bool _canFlip = true;
    private bool _canMove = true;
    [SerializeField] private int _fallFast = 0; // If 0, no fallFast. else, gravity * fallFast multiplier
    private float _prevGravScale = 1;

    public Queue<Vector2> jumpList = new Queue<Vector2>();

    public void Start()
    {
        target = GameObject.FindWithTag(_targetTag).transform;
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        gameObject.layer = 8; // Enemy layer
        // Check if flying, if so, no collision on platforms
        if (flyingEnabled)
        {
            gameObject.layer = 13; // Flying enemy layer
        }
        _enemyCore = transform.GetChild(0).GetComponent<EnemyCore>();
        _enemyCore.StartArmor += IsAttacking;
        _prevGravScale = rb.gravityScale;
        InvokeRepeating("UpdatePath", 0f, pathUpdateSeconds);

    }
    private void FixedUpdate()
    {
        if (TargetInDistance() && followEnabled)
        {
            PathFollow();
        }
        if (_stuckHelp)
        {
            if (Mathf.Abs(rb.velocity.magnitude) < stuckVelocityThreshold)
            {
                stuckTimer += Time.fixedDeltaTime;
            }
            else
            {
                stuckTimer = 0;
            }
        }
    }

    private void OnDisable()
    {
        _enemyCore.StartArmor -= IsAttacking;
    }

    private void UpdatePath()
    {
        if (!isFollowingJumpPath && followEnabled && TargetInDistance() && seeker.IsDone())
        {
            if (jumpList.Count > 0)
            {
                Vector2 nextPostion = jumpList.Dequeue();
                seeker.StartPath(rb.position, nextPostion, OnPathComplete);
                isFollowingJumpPath = true;
                timeSincePathStart = stuckDelay;
            }
            else
            {
                seeker.StartPath(rb.position, target.position, OnPathComplete);
            }
        }
        else if (isFollowingJumpPath && (timeSincePathStart <= 0))
        {
            if ((Math.Abs(rb.velocity.x) < 0.5) || (Math.Abs(rb.velocity.y) < 0.5))
            {
                isFollowingJumpPath = false;
                jumpList.Clear();
            }
        }
        else if (timeSincePathStart > 0)
        {
            timeSincePathStart -= (Time.deltaTime * 120);
        }
    }
    private void PathFollow()
    {
        if (path == null || pathfind == false)
        {
            isFollowingJumpPath = false;
            return;
        }

        float distanceToPlayer = Vector2.Distance(rb.position, target.position);

        if ((distanceToPlayer < _playerBubbleDistance) && _canMove)
        {
            Vector2 directionToPlayer = ((Vector2)target.position - rb.position).normalized;
            Vector2 moveAwayForce = -directionToPlayer * speed * Time.deltaTime;
            if (flyingEnabled)
                rb.AddForce(moveAwayForce);
            else
                rb.AddForce(new Vector2(moveAwayForce.x, 0));
        }
        else
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

            foreach (GameObject enemy in enemies)
            {
                if (enemy != gameObject)
                {
                    float distanceToEnemy = Vector2.Distance(rb.position, enemy.transform.position);

                    // If the enemy is too close to another enemy, move away
                    if (distanceToEnemy < _enemyBubbleDistance)
                    {
                        Vector2 directionToEnemy = ((Vector2)enemy.transform.position - rb.position).normalized;
                        float distanceFactor = 1.0f - (distanceToEnemy / _enemyBubbleDistance); // Adjust the factor
                        Vector2 moveAwayFromEnemyForce = -directionToEnemy * speed * distanceFactor * Time.deltaTime;

                        // Set the horizontal velocity while keeping the current vertical velocity


                        // Apply the force to the Rigidbody2D
                        if (_canMove)
                        {
                            if (flyingEnabled)
                                rb.AddForce(moveAwayFromEnemyForce);
                            else
                            {
                                moveAwayFromEnemyForce.y = rb.velocity.y;
                                rb.AddForce(new Vector2(moveAwayFromEnemyForce.x, 0));
                            }
                        }
                    }
                }
            }
            // Reached end of path
            if (currentWaypoint >= path.vectorPath.Count && time <= 0)
            {
                if (isFollowingJumpPath && _canMove)    // This could only be true if we have jumplist count > 0. Which we would set to 0 if flying enemy anyways. Keep this in mind 
                    rb.AddForce(Vector2.up * speed * jumpModifier); // Add jump

                isFollowingJumpPath = false;
                time = jumpDelay;
                return;
            }
            else if (currentWaypoint >= path.vectorPath.Count)
            {
                time -= Time.deltaTime;
                return;
            }

            // See if colliding with anything
            // Vector3 startOffset = transform.position - new Vector3(0f, GetComponent<Collider2D>().bounds.extents.y + jumpCheckOffset);
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckSize, groundLayer);

            IsOnOneWayPlatform = Physics2D.OverlapCircle(groundCheck.position, groundCheckSize, platformLayer);

            // Direction Calculation
            Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
            Vector2 force = direction * speed * Time.deltaTime;


            // Jump
            if (jumpEnabled && (isGrounded || IsOnOneWayPlatform) && !flyingEnabled)
            {
                if (time > 0f)
                {
                    time -= Time.deltaTime;
                }
                else if (time <= 0)
                {
                    if ((direction.y > jumpNodeHeightRequirement) && _canMove)
                    {
                        rb.AddForce(Vector2.up * speed * jumpModifier);
                    }
                    time = jumpDelay;
                }
            }
            // fallFast
            if (!isGrounded && (_fallFast > 0))
            {
                rb.gravityScale = _prevGravScale * _fallFast;
            }
            else if (_fallFast > 0)
            {
                rb.gravityScale = _prevGravScale;
            }

            // Movement
            if (!isGrounded && !IsOnOneWayPlatform)
            {
                if (!flyingEnabled)
                    force.y = 0;
            }
            if (_canMove)
                rb.AddForce(force);

            // One way platforms
            if (IsOnOneWayPlatform)
            {
                // Only go down if direction from path to player is downwards-ish, angle can be adjusted
                // Converts radians in arctan's domain to degrees from 0-360
                var directionInDegrees = Math.Atan2(direction.y, direction.x);
                directionInDegrees *= (180 / Math.PI);
                directionInDegrees = (directionInDegrees + 360) % 360;
                if (directionInDegrees >= minDropAngle && directionInDegrees <= maxDropAngle)
                {
                    StartCoroutine(DisablePlatformCollision());
                }
            }

            // Next Waypoint
            float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
            if (distance < nextWaypointDistance)
            {
                currentWaypoint++;
            }

            // Direction Graphics Handling
            if (directionLookEnabled && _canFlip)
            {
                if (rb.velocity.x > 0.5f)
                {
                    transform.localScale = new Vector3(-1f * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                }
                else if (rb.velocity.x < 0.5f)
                {
                    transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                }
            }
            // Check if the enemy is stuck
            if ((stuckTimer > stuckTimeThreshold) && _stuckHelp)
            {
                // Apply force towards the next waypoint
                if (currentWaypoint < path.vectorPath.Count)
                {
                    Vector2 currentPos = new Vector2(rb.position.x, rb.position.y); // Convert to Vector2
                    Vector2 waypointPos = path.vectorPath[currentWaypoint];

                    // Calculate the direction in the x-axis only
                    float directionX = waypointPos.x - currentPos.x;
                    Vector2 directionToWaypoint = new Vector2(directionX, 0).normalized;

                    rb.AddForce(directionToWaypoint * stuckForce);
                }
                stuckTimer = 0; // Reset the timer
            }

        }
    }

    private bool TargetInDistance()
    {
        return Vector2.Distance(transform.position, target.transform.position) < activateDistance;
    }

    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    private IEnumerator DisablePlatformCollision()
    {
        var playerCollider = rb.GetComponent<Collider2D>();
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, groundCheckSize, platformLayer);
        foreach (Collider2D collider in colliders)
        {
            Physics2D.IgnoreCollision(playerCollider, collider, true);
        }
        yield return new WaitForSeconds(dropthroughTime);
        foreach (Collider2D collider in colliders)
        {
            Physics2D.IgnoreCollision(playerCollider, collider, false);
        }
    }
    public void newJumpPosition(Vector2 position)
    {
        if (jumpList.Count < maxJumpTrackNum)
            jumpList.Enqueue(position);
    }

    private void IsAttacking(bool isAttacking)
    {
        Debug.Log("Calling isAttacking");
        _canFlip = !isAttacking;
        _canMove = !isAttacking;
        if (isAttacking)
        {
            if (((Vector2)target.position).x < rb.position.x)
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            else
                transform.localScale = new Vector3(-1f * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }
}