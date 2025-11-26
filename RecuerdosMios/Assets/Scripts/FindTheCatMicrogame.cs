using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

[Serializable]
public class CatSceneConfig
{
    public string sceneName;     // Opcional: "Patio", "Sala", etc.
    public GameObject root;      // GameObject padre: CatScene_X
    public Button catButton;     // Botón invisible sobre el gato
}

public class FindTheCatMicrogame : MonoBehaviour
{
    [Header("Config microjuego")]
    [SerializeField] private float timeLimit = 5f;

    [Header("Escenas de gato")]
    [SerializeField] private List<CatSceneConfig> catScenes = new List<CatSceneConfig>();

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI instructionText;
    [SerializeField] private Image timerFill;       // Image (Filled)
    [SerializeField] private TextMeshProUGUI resultText;

    [Header("Integración futura")]
    public UnityEvent<bool> onMicrogameEnd;         // true = win, false = lose

    private float currentTime;
    private bool isFinished = false;
    private CatSceneConfig currentScene;

    private void Start()
    {
        SetupScenes();
        SetupRandomScene();
        StartMicrogame();
    }

    private void SetupScenes()
    {
        // Asegurarnos de desactivar todas las escenas al inicio
        foreach (var scene in catScenes)
        {
            if (scene.root != null)
            {
                scene.root.SetActive(false);
            }

            if (scene.catButton != null)
            {
                // Quitamos listeners previos por si acaso
                scene.catButton.onClick.RemoveAllListeners();
            }
        }
    }

    private void SetupRandomScene()
    {
        if (catScenes == null || catScenes.Count == 0)
        {
            Debug.LogError("No hay escenas de gato configuradas en FindTheCatMicrogame.");
            return;
        }

        // Elegir una escena al azar
        int index = UnityEngine.Random.Range(0, catScenes.Count);
        currentScene = catScenes[index];

        // Activar solo esa escena
        if (currentScene.root != null)
        {
            currentScene.root.SetActive(true);
        }

        // Asignar callback al botón del gato
        if (currentScene.catButton != null)
        {
            currentScene.catButton.onClick.AddListener(OnCatFound);
        }

        Debug.Log("Escena de gato seleccionada: " + currentScene.sceneName);
    }

    private void StartMicrogame()
    {
        isFinished = false;

        currentTime = timeLimit;
        if (timerFill != null)
        {
            timerFill.fillAmount = 1f;
        }

        if (resultText != null)
        {
            resultText.SetText("");
        }

        if (instructionText != null)
        {
            instructionText.SetText("Encuentra al gato");
        }
    }

    private void Update()
    {
        if (isFinished) return;

        // Actualizar temporizador
        currentTime -= Time.deltaTime;

        if (timerFill != null)
        {
            timerFill.fillAmount = Mathf.Clamp01(currentTime / timeLimit);
        }

        // Si se acaba el tiempo y no encontró al gato → pierde
        if (currentTime <= 0f)
        {
            Lose();
        }
    }

    private void OnCatFound()
    {
        if (isFinished) return;

        Win();
    }


    private void Win()
    {
        if (isFinished) return;
        isFinished = true;

        // UI / mensajes
        // ...

        // Avisar al GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnMicrogameEnd(true);
        }

        // (Opcional) seguir usando el UnityEvent si quieres
        onMicrogameEnd?.Invoke(true);
    }


  
    private void Lose()
    {
        if (isFinished) return;
        isFinished = true;

        // UI / mensajes
        // ...

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnMicrogameEnd(false);
        }

        onMicrogameEnd?.Invoke(false);
    }

}



