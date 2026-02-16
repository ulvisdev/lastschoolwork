using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 8f;
    public float jumpForce = 12f;
    public Transform groundCheck;
    public float groundCheckDistance = .12f;
    public Vector2 groundCheckOffset = new Vector2(0f, -.5f);
    public LayerMask groundLayer;
    private Rigidbody2D rb;
    private float horizInput;
    private bool isGrounded;
    private SpriteRenderer spriteRenderer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        horizInput = Input.GetAxisRaw("Horizontal");
        Vector2 rayOrigin = groundCheck != null ? (Vector2)groundCheck.position : (Vector2)transform.position + groundCheckOffset;
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, groundCheckDistance, groundLayer);
        isGrounded = hit.collider != null;

        if(Input.GetButtonDown ("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        if(spriteRenderer != null)
        {
            if(horizInput > .1f) spriteRenderer.flipX = false;
            if(horizInput < .1f) spriteRenderer.flipX = true;
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(horizInput * speed, rb.linearVelocity.y);
    }
}
