using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;

    private float inputH;

    private HealthSystem healthSystem;
    private bool dead;
    private CinemachineVirtualCamera virtualCamera;

    [Header("Movement System")]
    [SerializeField] private Transform feet;
    [SerializeField] private float minX = -55f;
    [SerializeField] private float movementVelocity = 5f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float distanceDetectionGround = 0.2f;
    [SerializeField] private LayerMask jumpedLayer;

    [Header("Combat System")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRadius = 0.5f;
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private LayerMask damageableLayer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        healthSystem = GetComponent<HealthSystem>();
        virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();

        if (healthSystem != null)
        {
            healthSystem.OnDamaged += OnHurt;
            healthSystem.OnDeath += OnDeath;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        inputH = input.x;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (OnGround())
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            AudioManager.Instance.PlaySFX(AudioID.PlayerJump);
            anim.SetTrigger("jump");
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        anim.SetTrigger("attack_1");
    }

    void FixedUpdate()
    {
        Vector3 pos = transform.position;

        if (dead)
        {
        rb.velocity = Vector2.zero;
            return;
        }

        rb.velocity = new Vector2(inputH * movementVelocity, rb.velocity.y);

        anim.SetBool("running", inputH != 0);

        if (inputH > 0)
            transform.eulerAngles = Vector3.zero;
        else if (inputH < 0)
            transform.eulerAngles = new Vector3(0, 180, 0);

        if (pos.x < minX)
            pos.x = minX;

        transform.position = pos;
    }

    private bool OnGround()
    {
        return Physics2D.Raycast(
            feet.position,
            Vector2.down,
            distanceDetectionGround,
            jumpedLayer
        );
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

    private void OnDeath()
    {
        dead = true;

        rb.velocity = Vector2.zero;
        inputH = 0;

        anim.enabled = false;

        if (virtualCamera != null)
            virtualCamera.Follow = null;

        GameOverUI gameOver = FindObjectOfType<GameOverUI>();
        if (gameOver != null)
            gameOver.ShowGameOverFromPlayer();
    }

    private void OnHurt()
    {
        if (dead) return;
        anim.SetTrigger("hurt");
    }

    public void OnDeathAnimationFinished()
    {
        rb.simulated = false;

        GameOverUI gameOver = FindObjectOfType<GameOverUI>();
        if (gameOver != null)
            gameOver.ShowGameOverFromPlayer();
    }

    public void PlayPlayerAttackSound()
    {
        AudioManager.Instance.PlaySFX(AudioID.PlayerAttack);
    }

}
