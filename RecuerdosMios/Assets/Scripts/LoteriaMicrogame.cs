using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

[Serializable]
public class LoteriaCardSlot
{
    public string cardName;   // Ej: "La Luna", "El Catrín", etc.
    public Button button;     // Botón que está sobre esa carta
}

[Serializable]
public class LoteriaBoardConfig
{
    public string boardName;        // Opcional: "Tablero clásico"
    public Sprite boardSprite;      // Imagen del tablero 5x4
    [TextArea] public string description; // Solo para ustedes

    // Nombres de cartas en orden (debe tener la misma cantidad que cardSlots)
    public List<string> cardNames = new List<string>();
}

public class LoteriaMicrogame : MonoBehaviour
{
    [Header("Config microjuego")]
    [SerializeField] private float timeLimit = 5f;
    [SerializeField] private List<LoteriaCardSlot> cardSlots = new List<LoteriaCardSlot>();

    [Header("Tableros disponibles")]
    [SerializeField] private Image boardImage;  // Image del Canvas con el tablero
    [SerializeField] private List<LoteriaBoardConfig> boards = new List<LoteriaBoardConfig>();

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI instructionText;
    [SerializeField] private Image timerFill;   // Image con type = Filled
    [SerializeField] private TextMeshProUGUI resultText;

    [Header("Integración futura")]
    public UnityEvent<bool> onMicrogameEnd;    // true = win, false = lose

    private int targetIndex = -1;
    private float currentTime;
    private bool isFinished = false;

    private LoteriaBoardConfig currentBoard;

    private void Awake()
    {
        // Conectar cada botón con su callback
        for (int i = 0; i < cardSlots.Count; i++)
        {
            int index = i; // MUY importante para capturar bien el índice
            if (cardSlots[i].button != null)
            {
                cardSlots[i].button.onClick.AddListener(() => OnCardClicked(index));
            }
        }
    }

    private void Start()
    {
        SetupRandomBoard();
        StartMicrogame();
    }

    private void SetupRandomBoard()
    {
        if (boards == null || boards.Count == 0)
        {
            Debug.LogError("No hay tableros configurados en LoteriaMicrogame.");
            return;
        }

        // Elegir un tablero al azar
        currentBoard = boards[UnityEngine.Random.Range(0, boards.Count)];

        // Cambiar sprite del tablero
        if (boardImage != null && currentBoard.boardSprite != null)
        {
            boardImage.sprite = currentBoard.boardSprite;
        }

        // Verificar cantidad de nombres
        if (currentBoard.cardNames.Count != cardSlots.Count)
        {
            Debug.LogWarning($"El tablero '{currentBoard.boardName}' tiene {currentBoard.cardNames.Count} nombres, pero hay {cardSlots.Count} casillas. Revisa esto.");
        }

        // Asignar nombres a cada casilla según el tablero
        for (int i = 0; i < cardSlots.Count; i++)
        {
            if (i < currentBoard.cardNames.Count)
            {
                cardSlots[i].cardName = currentBoard.cardNames[i];
            }
        }
    }

    private void StartMicrogame()
    {
        isFinished = false;
        if (resultText != null) resultText.SetText("");

        // Tiempo
        currentTime = timeLimit;
        if (timerFill != null)
        {
            timerFill.fillAmount = 1f;
        }

        // Elegir carta objetivo al azar
        targetIndex = UnityEngine.Random.Range(0, cardSlots.Count);
        string targetName = cardSlots[targetIndex].cardName;

        if (instructionText != null)
        {
            instructionText.SetText($"¡ <b>{targetName}</b> !");
        }
    }

    private void Update()
    {
        if (isFinished) return;

        currentTime -= Time.deltaTime;

        if (timerFill != null)
        {
            timerFill.fillAmount = Mathf.Clamp01(currentTime / timeLimit);
        }

        if (currentTime <= 0f)
        {
            Lose();
        }
    }

    private void OnCardClicked(int index)
    {
        if (isFinished) return;

        if (index == targetIndex)
        {
            Win();
        }
        else
        {
            Lose();
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
