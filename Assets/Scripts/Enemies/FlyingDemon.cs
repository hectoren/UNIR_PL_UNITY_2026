using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class FlyingDemon : EnemyBase
{
    [Header("Patrol")]
    [SerializeField] private Transform[] wayPoints;
    [SerializeField] private float patrolSpeed = 3f;
    [SerializeField] private float waypointReachDistance = 0.1f;

    [Header("Chase")]
    [SerializeField] private float chaseSpeed = 4f;

    [Header("Attack")]
    [SerializeField] private float attackDistance = 1.2f;
    [SerializeField] private float attackCooldown = 0.8f;
    [SerializeField] private float attackDamage = 10f;

    [Header("Combat System")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRadius = 0.5f;
    [SerializeField] private LayerMask damageableLayer;

    [Header("Facing")]
    [Tooltip("Actívalo si el sprite queda mirando al revés.")]
    [SerializeField] private bool invertFacing = false;

    private Rigidbody2D rb;
    private Transform player;

    private Vector2 currentDestination;
    private int currentIndex;

    private bool playerDetected;
    private bool isAttacking;

    private float nextAttackTime;
    private float attackUnlockTime;

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    private void Start()
    {
        GameObject playerHitBox = GameObject.FindGameObjectWithTag("PlayerHitBox");
        if (playerHitBox != null)
            player = playerHitBox.transform;

        if (wayPoints != null && wayPoints.Length > 0)
        {
            currentIndex = 0;
            currentDestination = wayPoints[currentIndex].position;
            FocusTarget(currentDestination);
        }
    }

    private void FixedUpdate()
    {
        if (isDead) return;

        if (playerDetected && player != null) FocusTarget(player.position);
        else FocusTarget(currentDestination);

        if (isAttacking && Time.time >= attackUnlockTime)
            isAttacking = false;

        if (isAttacking)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        if (playerDetected && player != null)
            HandleChaseAndAttack();
        else
            HandlePatrol();
    }

    private void HandlePatrol()
    {
        anim.SetBool("isFlying", true);

        if (Vector2.Distance(rb.position, currentDestination) <= waypointReachDistance)
            DefineNewDestination();

        MoveTo(currentDestination, patrolSpeed);
    }

    private void DefineNewDestination()
    {
        currentIndex = (currentIndex + 1) % wayPoints.Length;
        currentDestination = wayPoints[currentIndex].position;
    }

    private void HandleChaseAndAttack()
    {
        float dist = Vector2.Distance(rb.position, player.position);

        if (dist <= attackDistance && Time.time >= nextAttackTime)
        {
            StartAttack();
            return;
        }

        anim.SetBool("isFlying", true);
        MoveTo(player.position, chaseSpeed);
    }

    private void StartAttack()
    {
        isAttacking = true;
        rb.velocity = Vector2.zero;

        anim.SetTrigger("attack");

        nextAttackTime = Time.time + attackCooldown;

        attackUnlockTime = Time.time + Mathf.Min(attackCooldown, 0.35f);
    }

    public void Attack()
    {
        if (isDead || player == null) return;

        Vector2 dir = (player.position - transform.position).normalized;

        Vector2 attackPos = (Vector2)transform.position + dir * Mathf.Abs(attackPoint.localPosition.x);

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            attackPos,
            attackRadius
        );

        foreach (Collider2D hit in hits)
        {
            if (!hit.CompareTag("PlayerHitBox")) continue;

            HealthSystem hs = hit.GetComponentInParent<HealthSystem>();
            if (hs != null)
                hs.ReceivedDamage(attackDamage);
        }
    }


    public void EndAttack()
    {
        isAttacking = false;
    }

    private void MoveTo(Vector2 target, float speed)
    {
        rb.MovePosition(Vector2.MoveTowards(rb.position, target, speed * Time.fixedDeltaTime));
    }

    private void FocusTarget(Vector2 target)
    {
        bool targetOnRight = target.x > transform.position.x;

        if (!invertFacing)
            transform.localScale = targetOnRight ? Vector3.one : new Vector3(-1f, 1f, 1f);
        else
            transform.localScale = targetOnRight ? new Vector3(-1f, 1f, 1f) : Vector3.one;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerHitBox"))
            playerDetected = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("PlayerHitBox"))
            playerDetected = false;
    }

    protected override void HandleDeath()
    {
        base.HandleDeath();
        rb.velocity = Vector2.zero;
        Destroy(gameObject, 1.2f);
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
            Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}

