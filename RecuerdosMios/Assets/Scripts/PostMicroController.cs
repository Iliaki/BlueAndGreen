using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PostMicroController : MonoBehaviour
{
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI livesText;
    public TextMeshProUGUI progressText;

    [Header("Timer visual")]
    public Image timerFill;       // Image (Filled)
    public float postDuration = 2f;

    private float currentTime;
    private bool advanced = false;

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
                ? "Ganaste este microjuego"
                : "Perdiste este microjuego");
        }

        if (livesText != null)
        {
            livesText.SetText("Vidas restantes: " + gm.CurrentLives);
        }

        if (progressText != null)
        {
            progressText.SetText("Juego " + gm.GamesPlayedThisRound + " de " + gm.microgamesPerRound);
        }

        // Timer
        currentTime = postDuration;
        if (timerFill != null)
        {
            timerFill.type = Image.Type.Filled;
            timerFill.fillAmount = 1f;
        }
    }

    void Update()
    {
        if (advanced) return;

        currentTime -= Time.deltaTime;

        if (timerFill != null && postDuration > 0f)
        {
            float t = Mathf.Clamp01(currentTime / postDuration);
            timerFill.fillAmount = t;
        }

        if (currentTime <= 0f)
        {
            AdvanceAfterSummary();
        }
    }

    void AdvanceAfterSummary()
    {
        if (advanced) return;
        advanced = true;

        var gm = GameManager.Instance;
        if (gm == null)
        {
            Debug.LogError("PostMicroController: No hay GameManager.Instance al avanzar");
            return;
        }

        Debug.Log("PostMicroController: tiempo agotado, continuar ronda");
        gm.ContinueAfterSummary();
    }
}
