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
        rb.gravityScale = 0f;
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

            if (isChasing && playerTarget != null)
            {
                direction = new Vector2(
                    playerTarget.position.x - transform.position.x,
                    0f
                ).normalized;

                speed = speedPatrol * chaseSpeedMultiplier;
            }
            else
            {
                direction = new Vector2(
                    currentDestination.x - transform.position.x,
                    0f
                ).normalized;

                speed = speedPatrol;
            }

            rb.velocity = new Vector2(direction.x * speed, rb.velocity.y);

            FocusDirection(direction.x);

            // Llegó al punto de patrulla
            if (!isChasing && Mathf.Abs(transform.position.x - currentDestination.x) <= 0.05f)
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

        // DETECCIÓN DEL PLAYER
        if (other.CompareTag("DetectionPlayer"))
        {
            playerTarget = other.transform.root;
            isChasing = true;
        }

        // DAÑO AL PLAYER
        if (other.CompareTag("PlayerHitBox"))
        {
            HealthSystem hs = other.GetComponent<HealthSystem>();
            if (hs != null)
                hs.ReceivedDamage(attackDamage);
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
}
