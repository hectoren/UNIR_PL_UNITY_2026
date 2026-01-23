using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    private void Awake()
    {
        // 1) Asegura que el juego no quede pausado al entrar/re-entrar a la escena
        Time.timeScale = 1f;

        // 2) (Opcional) Limpia pausas raras de físicas si existieran
        Physics2D.simulationMode = SimulationMode2D.FixedUpdate;

        // Log de verificación
        Debug.Log("[GameInitializer] Time.timeScale reset to 1");
    }

    private void Start()
    {
        // Música de fondo del gameplay
        AudioManager.Instance.PlayMusic(AudioID.GameplayMusic);

        Debug.Log("[GameInitializer] Gameplay music started");
    }
}
