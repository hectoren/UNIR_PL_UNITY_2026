using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private GameObject gameOverText;
    [SerializeField] private Volume gameOverVolume;

    public static bool IsGameOver { get; private set; }

    void Start()
    {
        IsGameOver = false;
        gameOverText.SetActive(false);

        if (gameOverVolume != null)
            gameOverVolume.weight = 0f;

        Player player = FindObjectOfType<Player>();
        if (player != null)
        {
            HealthSystem health = player.GetComponent<HealthSystem>();
            if (health != null)
                health.OnDeath += ShowGameOver;
        }
    }

    void Update()
    {
        if (!IsGameOver) return;

        if (Keyboard.current.anyKey.wasPressedThisFrame ||
            (Gamepad.current != null && Gamepad.current.allControls.Any(c => c.IsPressed())))
        {
            QuitGame();
        }
    }

    private void ShowGameOver()
    {
        if (IsGameOver) return;

        IsGameOver = true;

        gameOverText.SetActive(true);

        if (gameOverVolume != null)
            gameOverVolume.weight = 1f;

        Time.timeScale = 0f;
    }

    private void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
