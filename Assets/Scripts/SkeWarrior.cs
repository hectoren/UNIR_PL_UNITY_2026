using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class SkeWarrior : EnemyBase
{
    [Header("Patrol")]
    [SerializeField] private Transform[] wayPoints;
    [SerializeField] private float speedPatrol = 2f;

    [Header("Chase")]
    [SerializeField] private float chaseSpeedMultiplier = 2.5f;

    [Header("Combat")]
    [SerializeField] private float attackDamage = 10f;

    [Header("Hurt / Death")]
    [SerializeField] private float hurtStunTime = 0.3f;
    [SerializeField] private float deathDestroyDelay = 1.2f;

    [Header("Damage Tuning")]
    [SerializeField] private float damageCooldown = 0.5f;
    private float nextDamageTime;

    [Header("Combat System")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRadius = 0.5f;
    [SerializeField] private LayerMask damageableLayer;
    private bool isAttacking;//


    private Animator animator;

    private int currentIndex;
    private Vector3 currentDestination;

    private Rigidbody2D rb;
    private Coroutine patrolRoutine;

    private bool isStunned;
    private bool isChasing;

    private Transform playerTarget;
    private Vector3 originalScale;

    protected override void Awake()
    {
        base.Awake();

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        rb.gravityScale = 1f;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        originalScale = transform.localScale;

        currentIndex = 0;
        currentDestination = wayPoints[currentIndex].position;

        patrolRoutine = StartCoroutine(Patrol());
    }

    IEnumerator Patrol()
    {
        while (true)
        {
            if (IsDead)
                yield break;

            if (isStunned)
            {
                rb.velocity = Vector2.zero;
                yield return null;
                continue;
            }

            Vector2 direction;
            float speed;

            // PERSECUCIÓN + ATAQUE
            if (isChasing && playerTarget != null)
            {
                float distanceToPlayer =
                    Mathf.Abs(playerTarget.position.x - transform.position.x);

                if (isAttacking)//
                {//
                    rb.velocity = Vector2.zero;//
                    yield return null;//
                    continue;//
                }

                // SI ESTÁ LO SUFICIENTEMENTE CERCA ATACA
                if (distanceToPlayer < 1.2f)
                {
                    rb.velocity = Vector2.zero;
                    isAttacking = true;//
                    anim.SetTrigger("attack_1");

                    yield return null;
                    continue;
                }

                // SI NO, SIGUE PERSIGUIENDO
                direction = new Vector2(
                    playerTarget.position.x - transform.position.x,
                    0f
                ).normalized;

                speed = speedPatrol * chaseSpeedMultiplier;
            }
            else
            {
                // REACTIVAR PERSECUCIÓN SI EL PLAYER REAPARECE
                if (playerTarget != null &&
                    Mathf.Abs(playerTarget.position.x - transform.position.x) < 8f)
                {
                    isChasing = true;
                    continue;
                }

                // PATRULLA NORMAL
                direction = new Vector2(
                    currentDestination.x - transform.position.x,
                    0f
                ).normalized;

                speed = speedPatrol;
            }

            rb.velocity = new Vector2(direction.x * speed, rb.velocity.y);
            FocusDirection(direction.x);

            if (!isChasing &&
                Mathf.Abs(transform.position.x - currentDestination.x) <= 0.05f)
            {
                rb.velocity = Vector2.zero;
                DefineNewDestination();
            }

            yield return null;
        }
    }


    private void DefineNewDestination()
    {
        currentIndex++;
        if (currentIndex >= wayPoints.Length)
            currentIndex = 0;

        currentDestination = wayPoints[currentIndex].position;
    }

    private void FocusDirection(float dirX)
    {
        if (dirX > 0f)
        {
            transform.localScale = new Vector3(
                Mathf.Abs(originalScale.x),
                originalScale.y,
                originalScale.z
            );
        }
        else if (dirX < 0f)
        {
            transform.localScale = new Vector3(
                -Mathf.Abs(originalScale.x),
                originalScale.y,
                originalScale.z
            );
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (IsDead) return;

        if (other.CompareTag("DetectionPlayer"))
        {
            playerTarget = other.transform.root;
            isChasing = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (IsDead) return;

        if (other.CompareTag("DetectionPlayer"))
        {
            playerTarget = null;
            isChasing = false;
            currentDestination = wayPoints[currentIndex].position;
        }
    }

    public void TryDealDamage(Collider2D other)
    {
        if (IsDead) return;
        if (Time.time < nextDamageTime) return;

        if (other.CompareTag("PlayerHitBox"))
        {
            HealthSystem hs = other.GetComponent<HealthSystem>();
            if (hs != null)
            {
                hs.ReceivedDamage(attackDamage);
                
                nextDamageTime = Time.time + damageCooldown;
            }
        }
    }

    protected override void HandleDamaged()
    {
        base.HandleDamaged();

        if (IsDead || isStunned) return;
        StartCoroutine(HurtStun());
    }

    private IEnumerator HurtStun()
    {
        isStunned = true;
        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(hurtStunTime);
        isStunned = false;
    }

    protected override void HandleDeath()
    {
        base.HandleDeath();

        if (patrolRoutine != null)
            StopCoroutine(patrolRoutine);

        rb.velocity = Vector2.zero;
        rb.simulated = false;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;

        Destroy(gameObject, deathDestroyDelay);
    }

    public void PlayAttack()
    {
        if (IsDead) return;
        animator.SetTrigger("attack_1");
    }

    private void Attack()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            attackPoint.position,
            attackRadius,
            damageableLayer
        );

        foreach (Collider2D hit in hits)
        {
            HealthSystem hs = hit.GetComponent<HealthSystem>();
            if (hs != null)
                hs.ReceivedDamage(attackDamage);
        }
    }

    private void OnDrawGizmos()
    {
        if (attackPoint != null)
            Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }

    public void OnAttackFinished()//
    {//
        isAttacking = false;//
    }

}
