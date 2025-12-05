using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Configuración de la ronda")]
    public int livesPerRound = 2;          // Vidas por ronda
    public int microgamesPerRound = 3;     // Cuántos microjuegos por ronda

    [Header("Escenas intermedias")]
    public string preMicrogameSceneName = "PreMicro";
    public string postMicrogameSceneName = "PostMicro";
    public string roundWinSceneName = "RoundWin";
    public string roundLoseSceneName = "RoundLose";

    [Header("Escenas de microjuegos")]
    public List<string> microgameSceneNames = new List<string>
    {
        "loteria",
        "ruleta",
        "perroCereal"
    };

    // Estado interno extra
    private string nextMicrogameScene;
    private bool lastMicrogameWin;

    // Propiedades para que otras escenas lean info
    public int CurrentLives => currentLives;
    public int WinsThisRound => winsThisRound;
    public int GamesPlayedThisRound => gamesPlayedThisRound;
    public bool LastMicrogameWin => lastMicrogameWin;
    public string NextMicrogameScene => nextMicrogameScene; // útil para PreMicro

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

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainGame")
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

        PrepareNextMicrogame();
    }

    private void PrepareNextMicrogame()
    {
        // ¿Ya se jugaron los microjuegos necesarios o no hay vidas?
        if (gamesPlayedThisRound >= microgamesPerRound || currentLives <= 0)
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
        nextMicrogameScene = availableMicrogames[index];
        availableMicrogames.RemoveAt(index);

        Debug.Log($"Próximo microjuego: {nextMicrogameScene}");

        // En vez de cargar el microjuego directo, cargamos la escena previa
        SceneManager.LoadScene(preMicrogameSceneName);
    }

    public void StartSelectedMicrogame()
    {
        if (!string.IsNullOrEmpty(nextMicrogameScene))
        {
            Debug.Log($"Cargando microjuego real: {nextMicrogameScene}");
            SceneManager.LoadScene(nextMicrogameScene);
        }
        else
        {
            Debug.LogError("No hay microjuego pendiente para cargar.");
        }
    }

    // Esta función la llamarán los microjuegos al terminar
    public void OnMicrogameEnd(bool win)
    {
        gamesPlayedThisRound++;
        lastMicrogameWin = win;

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

        // Siempre, después de un microjuego, vamos a la escena de resumen
        SceneManager.LoadScene(postMicrogameSceneName);
    }

    public void ContinueAfterSummary()
    {
        // ¿Se acabaron las vidas o ya jugamos todos los microjuegos?
        if (currentLives <= 0 || gamesPlayedThisRound >= microgamesPerRound)
        {
            EndRound();
        }
        else
        {
            PrepareNextMicrogame();
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
            SceneManager.LoadScene(roundWinSceneName);
        }
        else
        {
            Debug.Log($"Ronda PERDIDA. Ganaste {winsThisRound} de {microgamesPerRound}.");
            SceneManager.LoadScene(roundLoseSceneName);
        }

        // El GameManager sigue vivo (DontDestroyOnLoad)
        // y se reusará si volvemos a MainGame.
    }
}
