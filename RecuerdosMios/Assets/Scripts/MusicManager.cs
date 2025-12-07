using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [Header("Audio")]
    public AudioSource audioSource;

    // Si quieres asignarlo desde el Inspector, puedes ignorar el Resources.Load de abajo
    public AudioClip musicClip;

    [Header("Escenas donde debe sonar la música")]
    // Lista ya inicializada con las escenas que mencionaste
    public List<string> allowedScenes = new List<string>
    {
        "PreMicro",
        "encuentraGato",
        "loteria",
        "MarcasCigarros",
        "perroCereal",
        "PostMicro",
        "ruleta"
    };

    private void Awake()
    {
        // Singleton: solo puede haber un MusicManager vivo
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        // Intentar cargar el clip si no está asignado
        if (musicClip == null)
        {
            // Opción 1 (recomendada): tener LoopMicrojuegos.wav en una carpeta llamada "Resources"
            // y cargarlo por código:
            musicClip = Resources.Load<AudioClip>("LoopMicrojuegos");
        }

        if (musicClip != null)
        {
            audioSource.clip = musicClip;
            audioSource.loop = true;
        }
        else
        {
            Debug.LogWarning("MusicManager: no se encontró el clip 'LoopMicrojuegos'. Asigna el AudioClip en el inspector o colócalo en Resources.");
        }

        // Suscribirse al evento de cambio de escena
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Revisar escena actual al iniciar
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        bool shouldPlay = allowedScenes.Contains(scene.name);

        if (shouldPlay)
        {
            if (!audioSource.isPlaying && audioSource.clip != null)
            {
                audioSource.Play();   // continúa donde iba, no se reinicia al cambiar de escena
            }
        }
        else
        {
            // Si la nueva escena no está en la lista, detenemos y destruimos el manager
            if (audioSource.isPlaying)
                audioSource.Stop();

            SceneManager.sceneLoaded -= OnSceneLoaded;
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Instance = null;
        }
    }
}
