using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Wizard : EnemyBase
{
    [SerializeField] private GameObject ball;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float attackTime = 2f;

    private Transform player;
    private Rigidbody2D rb;

    private Coroutine attackCoroutine;
    private bool playerDetected;

    private bool faceRight = true;

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void Start()
    {
        GameObject playerHitBox = GameObject.FindGameObjectWithTag("PlayerHitBox");
        if (playerHitBox != null)
            player = playerHitBox.transform;
    }

    private void Update()
    {
        if (isDead) return;

        if (playerDetected && player != null)
            FacePlayer();
    }

    private IEnumerator AttackRoutine()
    {
        while (playerDetected && !isDead)
        {
            anim.SetTrigger("fireball");

            float t = 0f;
            while (t < attackTime)
            {
                if (!playerDetected || isDead) yield break;
                t += Time.deltaTime;
                yield return null;
            }
        }

        attackCoroutine = null;
    }

    private void ThrowBall()
    {
        if (!playerDetected || isDead) return;

        Quaternion rot = faceRight ? Quaternion.Euler(0f, 0f, 0f)
                                   : Quaternion.Euler(0f, 0f, 180f);

        Instantiate(ball, spawnPoint.position, rot);
    }

    private void FacePlayer()
    {
        faceRight = (player.position.x >= transform.position.x);

        transform.localScale = faceRight ? Vector3.one : new Vector3(-1f, 1f, 1f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("PlayerHitBox")) return;

        playerDetected = true;

        if (attackCoroutine == null)
            attackCoroutine = StartCoroutine(AttackRoutine());
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("PlayerHitBox")) return;

        playerDetected = false;

        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }

        anim.ResetTrigger("fireball");
    }

    protected override void HandleDeath()
    {
        base.HandleDeath();

        if (attackCoroutine != null)
            StopCoroutine(attackCoroutine);

        Destroy(gameObject, 1.2f);
    }

    public void PlayFireballCastSound()
    {
        AudioManager.Instance.PlaySFX(AudioID.FireballCast);
    }

}
