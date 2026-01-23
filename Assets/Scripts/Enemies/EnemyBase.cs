using UnityEngine;

[RequireComponent(typeof(HealthSystem))]
[RequireComponent(typeof(Animator))]
public abstract class EnemyBase : MonoBehaviour
{
    [Header("Enemy Info")]
    [SerializeField] private string enemyName = "Enemy";

    protected HealthSystem health;
    protected Animator anim;

    protected bool isDead;

    public string EnemyName => enemyName;
    public bool IsDead => isDead;

    protected virtual void Awake()
    {
        health = GetComponent<HealthSystem>();
        anim = GetComponent<Animator>();
    }

    protected virtual void OnEnable()
    {
        health.OnDamaged += HandleDamaged;
        health.OnDeath += HandleDeath;
    }

    protected virtual void OnDisable()
    {
        health.OnDamaged -= HandleDamaged;
        health.OnDeath -= HandleDeath;
    }

    protected virtual void HandleDamaged()
    {
        if (isDead) return;
        anim.SetTrigger("hurt");
    }

    protected virtual void HandleDeath()
    {
        if (isDead) return;
        isDead = true;
        anim.SetBool("isDead", true);
    }
}
