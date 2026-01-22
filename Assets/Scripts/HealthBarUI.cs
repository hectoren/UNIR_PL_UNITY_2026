using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBarUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image fillImage;
    [SerializeField] private TMP_Text nameText;

    [Header("Target")]
    [SerializeField] private HealthSystem healthSystem;

    private void Awake()
    {
        if (healthSystem == null)
            healthSystem = GetComponentInParent<HealthSystem>();
    }

    private void OnEnable()
    {
        if (healthSystem != null)
            healthSystem.OnHealthChanged += UpdateHealthBar;
    }

    private void OnDisable()
    {
        if (healthSystem != null)
            healthSystem.OnHealthChanged -= UpdateHealthBar;
    }

    private void Start()
    {
        if (healthSystem != null)
            UpdateHealthBar(healthSystem.Health, healthSystem.MaxHealth);
    }

    private void UpdateHealthBar(float current, float max)
    {
        if (fillImage != null)
            fillImage.fillAmount = current / max;
    }

    public void SetName(string characterName)
    {
        if (nameText != null)
            nameText.text = characterName;
    }
}
