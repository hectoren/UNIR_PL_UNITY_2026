using System.Collections;
using UnityEngine;

public class SkeWarrior : EnemyBase
{
    [Header("Patrol")]
    [SerializeField] private Transform[] wayPoints;
    [SerializeField] private float speedPatrol = 2f;

    [Header("Combat")]
    [SerializeField] private float attackDamage = 10f;

    [Header("Hurt / Death")]
    [SerializeField] private float hurtStunTime = 0.3f;
    [SerializeField] private float deathDestroyDelay = 1.2f;

    private Vector3 currentDestination;
    private int currentIndex = 0;

    private Coroutine patrolRoutine;
    private bool isStunned;

    private Vector3 originalScale;

    protected override void Awake()
    {
        base.Awake();

        originalScale = transform.localScale;

        currentDestination = wayPoints[currentIndex].position;
        patrolRoutine = StartCoroutine(Patrol());
    }

    IEnumerator Patrol()
    {
        while (true)
        {
            while (transform.position != currentDestination)
            {
                if (IsDead)
                    yield break;

                if (isStunned)
                {
                    yield return null;
                    continue;
                }

                transform.position = Vector3.MoveTowards(
                    transform.position,
                    currentDestination,
                    speedPatrol * Time.deltaTime
                );

                yield return null;
            }

            DefineNewDestination();
        }
    }

    private void DefineNewDestination()
    {
        currentIndex++;
        if (currentIndex >= wayPoints.Length)
            currentIndex = 0;

        currentDestination = wayPoints[currentIndex].position;
        FocusDestination();
    }

    private void FocusDestination()
    {
        if (currentDestination.x > transform.position.x)
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

        if (other.CompareTag("DetectionPlayer"))
        {
            Debug.Log("Player Detectado!!!");
        }
        else if (other.CompareTag("PlayerHitBox"))
        {
            HealthSystem hs = other.GetComponent<HealthSystem>();
            if (hs != null)
                hs.ReceivedDamage(attackDamage);
        }
    }

    protected override void HandleDamaged()
    {
        base.HandleDamaged(); // dispara trigger "hurt"

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
        base.HandleDeath(); // fija isDead y animación Dead

        if (patrolRoutine != null)
            StopCoroutine(patrolRoutine);

        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;

        Destroy(gameObject, deathDestroyDelay);
    }
}
