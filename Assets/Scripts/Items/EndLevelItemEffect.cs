using UnityEngine;

public class EndLevelItemEffect : MonoBehaviour, IItemEffect
{
    private GameOverUI gameOverUI;

    public void SetGameOverUI(GameOverUI ui)
    {
        gameOverUI = ui;
    }

    public void Apply(GameObject collector)
    {
        if (gameOverUI == null)
        {
            Debug.LogError("GameOverUI not assigned to EndLevelItemEffect.");
            return;
        }

        gameOverUI.ShowLevelCompleted();
    }
}
