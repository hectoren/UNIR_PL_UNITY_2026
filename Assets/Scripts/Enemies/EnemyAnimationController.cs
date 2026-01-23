using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(HealthSystem))]
public class EnemyAnimationController : MonoBehaviour
{
    [SerializeField] private float hurtStunTime = 0.3f;
    private Animator anim;
    private HealthSystem health;
    private bool deadHandled;
    private bool isStunned;
    private Rigidbody2D rb;


    void Awake()
    {
        anim = GetComponent<Animator>();
        health = GetComponent<HealthSystem>();
        rb = GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        health.OnDamaged += OnHurt;
        health.OnDeath += OnDeath;
    }

    void OnDisable()
    {
        health.OnDamaged -= OnHurt;
        health.OnDeath -= OnDeath;
    }

    private void OnHurt()
    {
        if (health.isDead || isStunned) return;
        anim.SetTrigger("hurt");
        StartCoroutine(HurtStun());
    }

    private void OnDeath()
    {
        if (deadHandled) return;
        deadHandled = true;

        anim.SetBool("isDead", true);

        // Desactivar colisiones y movimiento
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.simulated = false;
        }

        // Destruir tras la animación Dead
        Destroy(gameObject, GetDeathClipLength());
    }

    private float GetDeathClipLength()
    {
        // Valor por defecto seguro
        float defaultTime = 1f;

        AnimatorStateInfo st = anim.GetCurrentAnimatorStateInfo(0);
        if (st.length > 0f)
            return st.length;

        return defaultTime;
    }

    private IEnumerator HurtStun()
    {
        isStunned = true;

        if (rb != null)
            rb.velocity = Vector2.zero;

        yield return new WaitForSeconds(hurtStunTime);

        isStunned = false;
    }

}
