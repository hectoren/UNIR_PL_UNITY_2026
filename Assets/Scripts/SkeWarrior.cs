using System.Collections;
using UnityEngine;

public class SkeWarrior : EnemyBase
{
    [Header("Patrol")]
    [SerializeField] private Transform[] wayPoints;
    [SerializeField] private float speedPatrol = 2f;

    [Header("Chase")]
    [SerializeField] private float chaseSpeedMultiplier = 4f;

    [Header("Combat")]
    [SerializeField] private float attackDamage = 10f;

    [Header("Hurt / Death")]
    [SerializeField] private float hurtStunTime = 0.3f;
    [SerializeField] private float deathDestroyDelay = 1.2f;

    private Vector3 currentDestination;
    private int currentIndex;

    private Coroutine patrolRoutine;
    private bool isStunned;
    private bool isChasing;

    private Transform playerTarget;
    private Vector3 originalScale;

    protected override void Awake()
    {
        base.Awake();

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
                yield return null;
                continue;
            }

            Vector3 targetPosition;

            if (isChasing && playerTarget != null)
            {
                targetPosition = new Vector3(
                    playerTarget.position.x,
                    transform.position.y,
                    transform.position.z
                );
            }
            else
            {
                targetPosition = currentDestination;
            }


            float speed = isChasing
                ? speedPatrol * chaseSpeedMultiplier
                : speedPatrol;

            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                speed * Time.deltaTime
            );

            FocusTarget(targetPosition);

            if (!isChasing && Vector3.Distance(transform.position, currentDestination) <= 0.05f)
            {
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

    private void FocusTarget(Vector3 target)
    {
        if (target.x > transform.position.x)
        {
            transform.localScale = new Vector3(
                Mathf.Abs(originalScale.x),
                originalScale.y,
                originalScale.z
            );
        }
        else
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

        // DETECCIÓN PARA PERSEGUIR
        if (other.CompareTag("DetectionPlayer"))
        {
            playerTarget = other.transform.root; // root = Player
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
        yield return new WaitForSeconds(hurtStunTime);
        isStunned = false;
    }

    protected override void HandleDeath()
    {
        base.HandleDeath();

        if (patrolRoutine != null)
            StopCoroutine(patrolRoutine);

        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;

        Destroy(gameObject, deathDestroyDelay);
    }
}
