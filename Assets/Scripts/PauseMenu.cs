using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "MainMenu";

    [Header("UI")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject controlsPanel;

    private bool isPaused;
    private bool showingControls;

    void Start()
    {
        pausePanel.SetActive(false);
        controlsPanel.SetActive(false);

        isPaused = false;
        showingControls = false;
    }

    void Update()
    {
        if (GameOverUI.IsGameOver)
            return;

        if (IsPausePressed())
        {
            HandlePauseInput();
        }
    }

    private bool IsPausePressed()
    {
        return (Keyboard.current != null && Keyboard.current.enterKey.wasPressedThisFrame)
            || (Gamepad.current != null && Gamepad.current.startButton.wasPressedThisFrame);
    }

    private void HandlePauseInput()
    {
        if (!isPaused)
        {
            Pause();
        }
        else if (showingControls)
        {
            BackToPause();
        }
        else
        {
            Resume();
        }
    }

    private void Pause()
    {
        isPaused = true;
        showingControls = false;

        Time.timeScale = 0f;
        pausePanel.SetActive(true);
        controlsPanel.SetActive(false);
    }

    public void Resume()
    {
        isPaused = false;
        showingControls = false;

        Time.timeScale = 1f;
        pausePanel.SetActive(false);
        controlsPanel.SetActive(false);
    }

    public void ShowControls()
    {
        showingControls = true;

        pausePanel.SetActive(false);
        controlsPanel.SetActive(true);
    }

    public void BackToPause()
    {
        showingControls = false;

        pausePanel.SetActive(true);
        controlsPanel.SetActive(false);
    }

    public void QuitGame()
    {
        /*#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif*/
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameSceneName);
    }
}
