using UnityEngine;
using TMPro;

public class PostMicroController : MonoBehaviour
{
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI livesText;
    public TextMeshProUGUI progressText; // opcional

    void Start()
    {
        var gm = GameManager.Instance;
        if (gm == null)
        {
            Debug.LogError("PostMicroController: No hay GameManager.Instance");
            return;
        }

        if (resultText != null)
        {
            resultText.SetText(gm.LastMicrogameWin
                ? "<color=green>¡Ganaste este microjuego!</color>"
                : "<color=red>Perdiste este microjuego...</color>");
        }

        if (livesText != null)
        {
            livesText.SetText($"Vidas restantes: {gm.CurrentLives}");
        }

        if (progressText != null)
        {
            progressText.SetText($"Juego {gm.GamesPlayedThisRound} de {gm.microgamesPerRound}");
        }
    }

    public void OnContinueButton()
    {
        var gm = GameManager.Instance;
        if (gm == null)
        {
            Debug.LogError("PostMicroController: No hay GameManager.Instance al presionar continuar");
            return;
        }

        Debug.Log("PostMicroController: Continuar después del resumen");
        gm.ContinueAfterSummary();
    }
}
