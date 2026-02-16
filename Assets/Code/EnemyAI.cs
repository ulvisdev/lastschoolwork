using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyAI : MonoBehaviour, IDamageable
{
    [Header("Stats")]
    [SerializeField] private float maxHealth = 50f;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private float damageAmount = 10f;

    [Header("Patrol Bounds")]
    [SerializeField] private Transform leftBound;
    [SerializeField] private Transform rightBound;

    private float currentHealth;
    private float attackTimer = 0f;
    private Transform player;
    private bool isMovingRight = true;
    private Rigidbody2D rb;

    [System.Obsolete]
    private void Awake()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();

        // Find player in scene
        PlayerHealth playerHealth = FindObjectOfType<PlayerHealth>();
        if (playerHealth != null)
            player = playerHealth.transform;
    }

    private void Update()
    {
        if (currentHealth <= 0) return;

        attackTimer -= Time.deltaTime;

        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            if (distanceToPlayer <= detectionRange)
            {
                ChasePlayer(distanceToPlayer);
            }
            else
            {
                Patrol();
            }
        }
        else
        {
            Patrol();
        }
    }

    private void ChasePlayer(float distanceToPlayer)
    {
        // Attack if in range
        if (distanceToPlayer <= attackRange)
        {
            IDamageable playerDamageable = player.GetComponent<IDamageable>();
            if (playerDamageable != null && attackTimer <= 0f)
            {
                playerDamageable.ApplyDamage(damageAmount);
                attackTimer = attackCooldown;
            }

            // Stop moving while attacking
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }
        else
        {
            // Move toward player
            float direction = player.position.x > transform.position.x ? 1f : -1f;
            Move(direction);
        }
    }

    private void Patrol()
    {
        if (leftBound != null && rightBound != null)
        {
            if (isMovingRight && transform.position.x >= rightBound.position.x)
                isMovingRight = false;
            else if (!isMovingRight && transform.position.x <= leftBound.position.x)
                isMovingRight = true;
        }

        Move(isMovingRight ? 1f : -1f);
    }

    private void Move(float direction)
    {
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
        }

        // Flip sprite
        if (direction != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(direction), 1f, 1f);
        }
    }

    // IDamageable implementation
    public bool ApplyDamage(float damage)
    {
        if (currentHealth <= 0f) return false;

        currentHealth -= damage;

        if (currentHealth <= 0f)
        {
            Die();
            return true;
        }

        return true;
    }

    private void Die()
    {
        rb.linearVelocity = Vector2.zero;
        gameObject.SetActive(false);
    }
    }