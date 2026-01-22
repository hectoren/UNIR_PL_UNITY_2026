using System;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] private float health = 100f;
    private float maxHealth;
    public bool isDead = false;

    // EVENTOS
    public event Action OnDamaged;
    public event Action OnDeath;
    public event Action<float, float> OnHealthChanged;

    // PROPIEDADES (solo lectura)
    public float Health => health;
    public float MaxHealth => maxHealth;

    private void Awake()
    {
        maxHealth = health;
    }

    private void Start()
    {
        // Inicializa la UI al comenzar
        OnHealthChanged?.Invoke(health, maxHealth);
    }

    public void ReceivedDamage(float damageReceived)
    {
        if (isDead) return;

        health -= damageReceived;
        health = Mathf.Clamp(health, 0f, maxHealth);

        OnHealthChanged?.Invoke(health, maxHealth);

        if (health > 0f)
        {
            OnDamaged?.Invoke();
        }
        else
        {
            Kill();
        }
    }

    public void Kill()
    {
        if (isDead) return;

        isDead = true;
        health = 0f;

        OnHealthChanged?.Invoke(health, maxHealth);
        OnDeath?.Invoke();
    }
}
