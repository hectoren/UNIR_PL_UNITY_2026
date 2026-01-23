using UnityEngine;

public class DropOnDeath : MonoBehaviour
{
    [Header("Drop")]
    [SerializeField] private GameObject dropPrefab;
    [SerializeField] private Transform dropPoint;
    [SerializeField] private GameOverUI gameOverUI;


    private HealthSystem health;
    private bool dropped;

    private void Awake()
    {
        health = GetComponent<HealthSystem>();
    }

    private void OnEnable()
    {
        if (health != null)
            health.OnDeath += HandleDeath;
    }

    private void OnDisable()
    {
        if (health != null)
            health.OnDeath -= HandleDeath;
    }

    private void HandleDeath()
    {
        if (dropped) return;
        if (dropPrefab == null) return;

        dropped = true;

        Vector3 position =
            dropPoint != null ? dropPoint.position : transform.position;

        //Instantiate(dropPrefab, position, Quaternion.identity);
        var droppedItem = Instantiate(dropPrefab, position, Quaternion.identity);

        // Inyección de dependencia en runtime
        var endLevelEffect = droppedItem.GetComponent<EndLevelItemEffect>();
        if (endLevelEffect != null && gameOverUI != null)
        {
            endLevelEffect.SetGameOverUI(gameOverUI);
        }
    }
}
