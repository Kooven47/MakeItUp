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

    [Header("Pathfinding")]
    public Transform target;
    public float activateDistance = 50f;
    public float pathUpdateSeconds = 0.5f;
    public float stuckDelay = 0f; // This is the delay to skip to the next path if the enemy is stuck when attempting a player history jump
    public int maxJumpTrackNum = 3;

    [Header("Physics")]
    public float minDropAngle = 240;
    public float maxDropAngle = 300;
    public float speed = 200f;
    public float nextWaypointDistance = 3f;
    public float jumpNodeHeightRequirement = 0.8f;
    public float jumpModifier = 0.3f;
    public float jumpCheckOffset = 0.1f;
    public float jumpDelay = 0.0f;
    public float groundCheckSize = 0.31f; // 0.31f allowed the groundcheck to match with the spaghetti monster collider. Adjust for other enemies.

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

    public Queue<Vector2> jumpList = new Queue<Vector2>();

    public void Start()
    {
        target = GameObject.FindWithTag("Player").transform;
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        InvokeRepeating("UpdatePath", 0f, pathUpdateSeconds);
    }
    private void FixedUpdate()
    {
        if (TargetInDistance() && followEnabled)
        {
            PathFollow();
        }
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
        if (path == null)
        {
            isFollowingJumpPath = false;
            return;
        }
        // Reached end of path
        if (currentWaypoint >= path.vectorPath.Count && time <= 0)
        {
            if (isFollowingJumpPath)    // This could only be true if we have jumplist count > 0. Which we would set to 0 if flying enemy anyways. Keep this in mind 
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
                if (direction.y > jumpNodeHeightRequirement)
                {
                    rb.AddForce(Vector2.up * speed * jumpModifier);
                }
                time = jumpDelay;
            }
        }

        // Movement
        if (!isGrounded && !IsOnOneWayPlatform)
        {
            if (!flyingEnabled)
                force.y = 0;
        }
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
        if (directionLookEnabled)
        {
            if (rb.velocity.x > 0.05f)
            {
                transform.localScale = new Vector3(-1f * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else if (rb.velocity.x < -0.05f)
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
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
        yield return new WaitForSeconds(0.5f);
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
}
