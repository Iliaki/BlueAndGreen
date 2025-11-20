using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

[Serializable]
public class RouletteBoardConfig
{
    public string boardName;          // Ej: "Ruleta fácil", "Ruleta difícil"
    public Sprite wheelSprite;        // Imagen de la ruleta
    public int segments = 6;          // Número de sectores (6 en tu caso)

    [Tooltip("Índices de los sectores que son derrota (0 a segments-1).")]
    public List<int> losingSectors = new List<int>();

    [Tooltip("Ajuste en grados para alinear el sector 0 con el puntero.")]
    public float angleOffset = 0f;
}

public class RussianRouletteMicrogame : MonoBehaviour
{
    [Header("Config microjuego")]
    [SerializeField] private float timeLimit = 5f;
    [SerializeField] private float spinSpeed = 180f;  // grados por segundo

    [Header("Ruleta")]
    [SerializeField] private Image wheelImage;             // UI Image de la ruleta
    [SerializeField] private List<RouletteBoardConfig> boards = new List<RouletteBoardConfig>();

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI instructionText;
    [SerializeField] private Image timerFill;              // Image con Type = Filled
    [SerializeField] private TextMeshProUGUI resultText;

    [Header("Integración futura")]
    public UnityEvent<bool> onMicrogameEnd;                // true = win, false = lose

    private RouletteBoardConfig currentBoard;
    private float currentTime;
    private bool isFinished = false;
    private bool isSpinning = true;

    private void Start()
    {
        SetupRandomBoard();
        StartMicrogame();
    }

    private void SetupRandomBoard()
    {
        if (boards == null || boards.Count == 0)
        {
            Debug.LogError("No hay tableros de ruleta configurados en RussianRouletteMicrogame.");
            return;
        }

        int index = UnityEngine.Random.Range(0, boards.Count);
        currentBoard = boards[index];

        if (wheelImage != null && currentBoard.wheelSprite != null)
        {
            wheelImage.sprite = currentBoard.wheelSprite;
        }

        // Asegurarnos de que tenga al menos 1 sector de derrota
        if (currentBoard.losingSectors == null || currentBoard.losingSectors.Count == 0)
        {
            Debug.LogWarning($"El tablero '{currentBoard.boardName}' no tiene sectores de derrota configurados.");
        }

        Debug.Log("Ruleta seleccionada: " + currentBoard.boardName);
    }

    private void StartMicrogame()
    {
        isFinished = false;
        isSpinning = true;

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
            instructionText.SetText("Detén la ruleta en una casilla segura (Barra espaciadora)");
        }

        // Reiniciar rotación de la ruleta
        if (wheelImage != null)
        {
            wheelImage.rectTransform.rotation = Quaternion.identity;
        }
    }

    private void Update()
    {
        if (isFinished) return;

        // 1) Girar la ruleta mientras esté activa
        if (isSpinning && wheelImage != null)
        {
            wheelImage.rectTransform.Rotate(0f, 0f, spinSpeed * Time.deltaTime);
        }

        // 2) Manejar entrada de usuario (barra espaciadora)
        if (isSpinning && Input.GetKeyDown(KeyCode.Space))
        {
            isSpinning = false;
            EvaluateResult();
        }

        // 3) Actualizar el temporizador
        currentTime -= Time.deltaTime;
        if (timerFill != null)
        {
            timerFill.fillAmount = Mathf.Clamp01(currentTime / timeLimit);
        }

        // Si se acaba el tiempo sin haber detenido la ruleta -> pierde
        if (currentTime <= 0f && !isFinished)
        {
            Lose();
        }
    }

    private void EvaluateResult()
    {
        if (wheelImage == null || currentBoard == null) return;

        // Obtener ángulo Z de la ruleta
        float zRotation = wheelImage.rectTransform.eulerAngles.z;

        // Ajustar con offset para alinear el sector 0 con el puntero
        float adjustedAngle = zRotation + currentBoard.angleOffset;

        // Normalizar ángulo entre 0 y 360
        adjustedAngle = (adjustedAngle % 360f + 360f) % 360f;

        // Tamaño angular de cada sector
        float sectorAngle = 360f / currentBoard.segments;

        // Índice de sector en el que cayó
        int sectorIndex = Mathf.FloorToInt(adjustedAngle / sectorAngle);
        sectorIndex = Mathf.Clamp(sectorIndex, 0, currentBoard.segments - 1);

        Debug.Log($"Ruleta detenida. zRot={zRotation:F1}, angleAdj={adjustedAngle:F1}, sector={sectorIndex}");

        // Revisar si es sector de derrota
        if (currentBoard.losingSectors.Contains(sectorIndex))
        {
            Lose();
        }
        else
        {
            Win();
        }
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
