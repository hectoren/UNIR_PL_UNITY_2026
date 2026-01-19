using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "SampleScene";

    public void OnNewGame()
    {
        Debug.Log("CLICK NEW GAME");
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameSceneName);
    }

    public void OnQuitGame()
    {
        Debug.Log("CLICK QUIT GAME");
        Application.Quit();

        // Solo para pruebas en el editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
