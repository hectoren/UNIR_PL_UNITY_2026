using UnityEngine;

public class SkeWarriorDamageHitbox : MonoBehaviour
{
    [SerializeField] private SkeWarrior owner;

    private void Reset()
    {
        owner = GetComponentInParent<SkeWarrior>();
    }

    private void Awake()
    {
        if (owner == null)
            owner = GetComponentInParent<SkeWarrior>();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (owner == null) return;
        owner.TryDealDamage(other);
    }
}
