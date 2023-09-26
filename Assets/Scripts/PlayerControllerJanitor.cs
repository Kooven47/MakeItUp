using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerJanitor : MonoBehaviour
{
    private float horizontal;
    private float speed;
    public float normalSpeed;
    public float sprintingSpeed;
    public float jumpingPower;
    private bool isFacingRight = true;
    private bool jumpKeyHeld;
    private bool isJumping;
    public Vector2 counterJumpForce;
    public Queue<Vector2> jumpList = new Queue<Vector2>();

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Animator _anim;
    public int maxJumpTrackNum = 3; // This is the number of jump history you want to keep track of for the ai to follow 
    // Start is called before the first frame update
    private void Start()
    {
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


        if (Input.GetButtonDown("Jump"))
        {
            jumpKeyHeld = true;
            if (IsGrounded())
            {
                // This keeps track of the jumps for the a* platforming
                if (jumpList.Count < maxJumpTrackNum)
                    jumpList.Enqueue(rb.position);  

                rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
            }

            if (rb.velocity.y > 0f)
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
            }
        }
        else if (Input.GetButtonUp("Jump"))
        {
            jumpKeyHeld = false;
        }


        Flip();
    }

    private void FixedUpdate()
    {
        _anim.SetBool("isMoving",horizontal != 0f);
        
        rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
        if (!IsGrounded())
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

    public Queue<Vector2> GetJumpList()
    {
        return jumpList;
    }
}
