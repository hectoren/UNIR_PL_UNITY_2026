using UnityEngine;

public class KnightBossDamageHitbox : MonoBehaviour
{
    [SerializeField] private KnightBoss owner;

    private void Reset()
    {
        owner = GetComponentInParent<KnightBoss>();
    }

    private void Awake()
    {
        if (owner == null)
            owner = GetComponentInParent<KnightBoss>();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (owner == null) return;
        owner.TryDealDamage(other);
    }
}
