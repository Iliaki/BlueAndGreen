using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Configuración de la ronda")]
    public int livesPerRound = 2;          // Vidas por ronda
    public int microgamesPerRound = 3;     // Cuántos microjuegos por ronda

    [Header("Escenas de microjuegos")]
    public List<string> microgameSceneNames = new List<string>
    {
        "MG_Loteria",
        "MG_Ruleta",
        "MG_SmashBar"
    };

    private int currentLives;
    private int winsThisRound;
    private int gamesPlayedThisRound;

    private List<string> availableMicrogames = new List<string>();

    private void Awake()
    {
        // Patrón Singleton sencillo
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        StartNewRound();
        Debug.Log("StartJuego");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (SceneManager.GetActiveScene().name == "MainGame")
        {
            StartNewRound();
        }
    }

    public void StartNewRound()
    {
        Debug.Log("=== Nueva ronda ===");

        currentLives = livesPerRound;
        winsThisRound = 0;
        gamesPlayedThisRound = 0;

        // Copia de la lista de microjuegos para no repetir
        availableMicrogames = new List<string>(microgameSceneNames);

        LoadNextMicrogame();
    }

    private void LoadNextMicrogame()
    {
        // ¿Ya se jugaron los microjuegos necesarios?
        if (gamesPlayedThisRound >= microgamesPerRound)
        {
            EndRound();
            return;
        }

        if (availableMicrogames.Count == 0)
        {
            Debug.LogWarning("No quedan microjuegos disponibles, terminando ronda.");
            EndRound();
            return;
        }

        // Elegir un microjuego aleatorio sin repetir
        int index = Random.Range(0, availableMicrogames.Count);
        string sceneName = availableMicrogames[index];
        availableMicrogames.RemoveAt(index);

        Debug.Log($"Cargando microjuego: {sceneName}");
        SceneManager.LoadScene(sceneName);
    }

    // Esta función la llamarán los microjuegos al terminar
    public void OnMicrogameEnd(bool win)
    {
        gamesPlayedThisRound++;

        if (win)
        {
            winsThisRound++;
            Debug.Log($"Microjuego ganado. Ganadas en esta ronda: {winsThisRound}");
        }
        else
        {
            currentLives--;
            Debug.Log($"Microjuego perdido. Vidas restantes: {currentLives}");
        }

        // ¿Se acabaron las vidas?
        if (currentLives <= 0)
        {
            Debug.Log("Se acabaron las vidas.");
            EndRound();
        }
        else if (gamesPlayedThisRound >= microgamesPerRound)
        {
            EndRound();
        }
        else
        {
            // Cargar el siguiente microjuego
            LoadNextMicrogame();
        }
    }

    private void EndRound()
    {
        // Para 3 microjuegos: necesitas al menos 2 ganados
        int minWinsToPass = Mathf.CeilToInt(microgamesPerRound / 2f);
        bool roundWon = winsThisRound >= minWinsToPass && currentLives > 0;

        if (roundWon)
        {
            Debug.Log($"Ronda GANADA. Ganaste {winsThisRound} de {microgamesPerRound}.");
            // Aquí luego: pasar a ronda 2, aumentar dificultad, etc.
        }
        else
        {
            Debug.Log($"Ronda PERDIDA. Ganaste {winsThisRound} de {microgamesPerRound}.");
        }

        // Por ahora: regresar al menú principal
        SceneManager.LoadScene("MainMenu");

        // Nota: el GameManager sigue vivo (DontDestroyOnLoad),
        // así que cuando vuelvas de nuevo a MainGame, no necesitas otro.
    }
}

