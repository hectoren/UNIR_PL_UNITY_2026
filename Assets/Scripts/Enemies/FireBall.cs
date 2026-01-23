using UnityEngine;

public class FireBall : MonoBehaviour
{
    private Rigidbody2D rb;

    [Header("Movement")]
    [SerializeField] private float shotImpulse = 8f;

    [Header("Combat")]
    [SerializeField] private float damage = 10f;
    [SerializeField] private float lifeTime = 3f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.AddForce(transform.right * shotImpulse, ForceMode2D.Impulse);

        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("PlayerHitBox"))
        {
            HealthSystem health = other.GetComponentInParent<HealthSystem>();
            if (health != null)
            {
                health.ReceivedDamage(damage);
            }

            Destroy(gameObject);
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject);
        }
    }
}
